using Mopups.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Doolist
{

    /*
     TODO:
    -implement preview of notes in mode 1
    -implement DisplayCategorySettings
    -implement DisplayListSettings
    -implement pinning DONE
    -implement importance button DONE
    -implement persistence** DONE
    -implement undo and redo* DONE
    -implement search bar in mode 0

    -fix UpdateBulletPointDisplays not actually updating in time when on a note loaded from user data*
    -fix plus button position
    -fix template buttons not showing up on Android - I straight up cannot fix this, not in time anyways. Must be a MAUI issue
    -fix list count in catergory only displaying 1 (this used to work i didnt change anything???)

    after settings implemented: 
    -implement sorting of elements
    -implement color customization
    -implement text options in mode 2
     */
    public partial class MainPage : ContentPage
    {
        //mode determines what type of page is displayed cuz i didnt wanna figure out how to navigate with AppShell
        //0 for categories, 1 for lists, 2 for specific list 
        private int mode = 0;
        public ObservableCollection<Category> categories;
        public Category currentCategory;
        public TodoList currentList;
        public int currentListIndex;
        public List<TodoList> UndoBuffer = new List<TodoList>();
        public int UndoCounter = 0;
        //this is done so i dont have to pass a reference to mainPage to the TodoList constructor because that breaks everything for some reason
        public static MainPage MainPageInstance { get; private set; }



        public MainPage()
        {
            MainPageInstance = this;
            InitializeComponent();
            LoadContent();

            this.LayoutChanged += OnWindowChanged;
            AddButton.Pressed += OnAddButtonPressed;
            BackButton.Pressed += OnBackButtonPressed;
            SettingsButton.Pressed += DisplaySettings;
            categories.CollectionChanged += (s, e) => { UpdateDisplays(false); };

            SwitchToCategoriesMode();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ResizeTopBar();
            ResizeAddButton();
            ResizeContentScrollView();
        }

        void OnWindowChanged(object sender, EventArgs e)
        {
            ResizeTopBar();
            ResizeAddButton();
            ResizeContentScrollView();
        }

        void ResizeTopBar()
        {
            double height = this.Height;
            double width = this.Width;

            foreach (ImageButton child in TopBar.Children)
            {
                    child.HeightRequest = height * .05;
                    child.WidthRequest = height * .05;

            }
        }

        void ResizeAddButton()
        {
            double height = this.Height;
            double width = this.Width;

            ContentGridFillerBox.HeightRequest = height * .05;
            ContentGridFillerBox.WidthRequest = height * .05;

            AddButton.HeightRequest = height * .05;
            AddButton.WidthRequest = height * .05;


        }

        void ResizeContentScrollView()
        {
            double height = this.Height;
            double width = this.Width;

            ContentScrollView.HeightRequest = height * .95;
            ContentScrollView.WidthRequest = width;
        }

        async void OnAddButtonPressed(object sender, EventArgs e)
        {
            switch (mode)
            {
                case 0:
                    string name = await DisplayPromptAsync("", "Enter a name for your category");
                    if (name == null) { name = "Category"; };
                    Category cat = new Category(name);
                    categories.Add(cat);
                    SaveContent();
                    break;
                case 1:
                    TodoList list = new TodoList();
                    currentList = list;
                    currentListIndex = currentCategory.lists.IndexOf(currentList);
                    currentCategory.lists.Add(list);
                    SwitchToBulletPointsMode(list);
                    SaveContent();
                    break;
                default:
                    DisplayAlert("Alert", "You shouldnt be able to see this button", "OK");
                    break;
            }

        }

        void OnBackButtonPressed(object sender, EventArgs e)
        {
            SaveContent();

            switch (mode)
            {
                case 2:
                    SwitchToNotesMode(currentCategory);
                    break;
                case 1:
                    SwitchToCategoriesMode();
                    break;
                default:
                    DisplayAlert("Alert", "You shouldnt be able to see this button", "OK");
                    break;
            }
        }
        public void DeleteBulletPoint(object sender, EventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            BulletPointDisplay parent = btn.Parent as BulletPointDisplay;
            currentList.bulletPoints.Remove(parent.source);

            SaveContent();
            AddCurrentStateToUndoBuffer();
        }

        public void OnPinButtonClicked(object sender, EventArgs e)
        {
            ImageButton pinBtn = (ImageButton)sender;
            ContentCellDisplay parent = (ContentCellDisplay)pinBtn.Parent;
            ListableElement source = parent.source;

            source.IsPinned = source.IsPinned ? false : true;
            pinBtn.Opacity = source.IsPinned ? 1 : 0.1;

            SortByPinned();

            SaveContent();
            if(mode == 2)
                AddCurrentStateToUndoBuffer();
        }

        public void ResizeTemplateButton(object sender, EventArgs e)
        {
            VisualElement btn = (VisualElement)sender;

            double height = ((ContentCellDisplay)btn.Parent).Height;
            double width = ((ContentCellDisplay)btn.Parent).Width;

            btn.HeightRequest = height * .04;
            btn.WidthRequest = height * .04;
        }

        void ResizeTemplateButtons(ContentCellDisplay display)
        {
            foreach (var child in display.Children)
            {
                if(child.GetType() == typeof(ImageButton) || child.GetType() == typeof(Button))
                {
                    ResizeTemplateButton(child, new EventArgs());
                }
            }
        }

        public void DisplaySettings(object sender, EventArgs e)
        {
            //TODO
        }
        public void DisplayCategorySettings(object sender, EventArgs e)
        {
            ImageButton imgBtn = (ImageButton)sender;
            CategoryDisplay parent = (CategoryDisplay)imgBtn.Parent;
            Category src = parent.source;

            MopupService.Instance.PushAsync(new CategorySettingsPopup(src, parent));
        }

        public void DisplayListSettings(object sender, EventArgs e)
        {
            //TODO
        }

        public void UpdateCategoryDisplays(bool onlyPush)
        {
            //this came before UpdateDisplays was abstracted to all 3 modes and is still used as a failsafe in UpdateDisplays
            //this comment came before i regretted doing that

            /*
            bool onlyPush = true; //true if all (except the last) elements in categories list match the ones being displayed 

            if(categories.Count > ContentCell.Children.Count - 1)
                onlyPush = false;

            if(categories.Count == 0)
                onlyPush = false;

            for(int i = 0; i < categories.Count - 1 && onlyPush; ++i) {
                if (categories[i] != ((CategoryDisplay)ContentCell.Children[i]).source)
                {
                    onlyPush = false; 
                }
            }
            */

            if (onlyPush) 
            {
                CategoryDisplay display = new CategoryDisplay(categories.Last(), this);
                ContentCell.Add(display);
                ResizeTemplateButtons(display);
            }
            else
            {
                ContentCell.Clear();
                foreach (Category category in categories)
                {
                    CategoryDisplay display = new CategoryDisplay(category, this);
                    ContentCell.Add(display);
                    ResizeTemplateButtons(display);
                }
            }

        }

        public void UpdateNoteDisplays(bool onlyPush)
        {
            if (onlyPush)
            {
                TodoListDisplay display = new TodoListDisplay(currentCategory.lists.Last(), this);
                ContentCell.Add(display);
                ResizeTemplateButtons(display);
            }
            else
            {
                ContentCell.Clear();
                foreach (TodoList list in currentCategory.lists)
                {
                    TodoListDisplay display = new TodoListDisplay(list, this);
                    ContentCell.Add(display);
                    ResizeTemplateButtons(display);
                }
            }

        }

        public void UpdateBulletPointDisplays(bool onlyPush)
        {
            if (onlyPush)
            {
                BulletPointDisplay display = new BulletPointDisplay(currentList.bulletPoints.Last(), this);
                ContentCell.Add(display);
                ResizeTemplateButtons(display);
            }
            else
            {
                ContentCell.Clear();
                ContentCell.Add(CreateTitleEditor());
                foreach (BulletPoint bp in currentList.bulletPoints)
                {
                    BulletPointDisplay display = new BulletPointDisplay(bp, this);
                    ContentCell.Add(display);
                    ResizeTemplateButtons(display);
                }
            }

        }

        public void UpdateDisplays(bool onlyPush)
        {
            switch (mode)
            {
                case 0:
                    UpdateCategoryDisplays(onlyPush);
                    break;
                case 1:
                    UpdateNoteDisplays(onlyPush);
                    break;
                case 2:
                    UpdateBulletPointDisplays(onlyPush);
                    break;
            }
        }

        /* I regretted doing this
        void UpdateDisplays(bool onlyPush)
        {
            //kind of ugly fucky wucky method with a lot of type shenanigans but doing it this way made it so i could abstract 3 methods into 1 ¯\_(ツ)_/¯

            Type[] displayTypes = { typeof(CategoryDisplay), typeof(TodoListDisplay), typeof(BulletPointDisplay) };
            object[] collections = { categories, currentCategory?.lists, currentList?.bulletPoints }; 
            dynamic currentCollection = collections[mode]; //afaik this is unsafe but the alternative was *extremely* ugly and i also couldnt get it to work 

            if (currentCollection == null)
            {
                SwitchToCategoriesMode();
                return;
            }

            if (onlyPush)
            {
                dynamic display = Activator.CreateInstance(displayTypes[mode], currentCollection.Last(), this);
                ContentCell.Add(display);
                ResizeTemplateButtons(display);
            }
            else
            {
                ContentCell.Clear();
                if(mode == 2) { ContentCell.Add(CreateTitleEditor()); }
                foreach (object item in currentCollection)
                {
                    dynamic display = Activator.CreateInstance(displayTypes[mode], item, this);
                    ContentCell.Add(display);
                    ResizeTemplateButtons(display);
                }
            }
        }
        */

        public void SwitchToCategoriesMode()
        {
            mode = 0;

            SaveContent();

            BackButton.IsEnabled = false;
            BackButton.IsVisible = false;
            RedoButton.IsEnabled = false;
            RedoButton.IsVisible = false;
            UndoButton.IsEnabled = false;
            UndoButton.IsVisible = false;
            AddButton.IsEnabled = true;
            AddButton.IsVisible = true;

            ContentCell.Clear();
            UpdateCategoryDisplays(false); 

            //add more stuff as more functionality comes along
        }

        public void SwitchToNotesMode(Category category)
        {
            mode = 1;
            currentCategory = category;
            SaveContent();

            BackButton.IsEnabled = true;
            BackButton.IsVisible = true;
            RedoButton.IsEnabled = false;
            RedoButton.IsVisible = false;
            UndoButton.IsEnabled = false;
            UndoButton.IsVisible = false;
            AddButton.IsEnabled = true;
            AddButton.IsVisible = true;

            ContentCell.Clear();
            UpdateNoteDisplays(false);
        }

        public void SwitchToBulletPointsMode(TodoList todoList)
        {
            mode = 2;
            currentList = todoList;
            currentListIndex = currentCategory.lists.IndexOf(currentList);
            UndoBuffer.Clear();
            UndoBuffer.Add(currentList.Clone());
            UndoCounter = 0;
            SaveContent();

            BackButton.IsEnabled = true;
            BackButton.IsVisible = true;
            RedoButton.IsEnabled = true;
            RedoButton.IsVisible = true;
            UndoButton.IsEnabled = true;
            UndoButton.IsVisible = true;
            AddButton.IsEnabled = false;
            AddButton.IsVisible = false;

            ContentCell.Clear();
            ContentCell.Add(CreateTitleEditor());
            UpdateBulletPointDisplays(false);
        }

        Editor CreateTitleEditor()
        {
            Editor editor = new Editor
            {
                FontSize = 60,
                Placeholder = "Title",
                Text = currentList.Title,
                BackgroundColor = Color.FromArgb("00000000")
            };

            editor.TextChanged += OnTitleEditorTextChanged;
            editor.Completed += OnTitleEditorCompleted;

            return editor;
        }

        void OnTitleEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            Editor editor = (Editor)sender;
            string output = e.NewTextValue.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            
            if(output != e.NewTextValue)
            {
                editor.Text = output;
                currentList.bulletPoints.Insert(0, new BulletPoint());
            }

            currentList.Title = output;
        }

        void OnTitleEditorCompleted(object sender, EventArgs e) {
            SaveContent();
            UndoBuffer.Add(currentList.Clone());
            ++UndoCounter;
        }

        public void OnBPEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            Editor editor = (Editor)sender;
            BulletPoint point = ((BulletPointDisplay)editor.Parent).source;

            char[] delimiters = { '\n', '\r', '\t' };
            string[] output = e.NewTextValue.Split(delimiters);

            point.Text = output[0];

            //if output length > 1 creates a new BPDisplay for every delimiter char entered (normally will only be one unless they paste smth with more than 1)
            for (int i = 1; i < output.Length; i++) {
                currentList.bulletPoints.Insert(currentList.bulletPoints.IndexOf(point) + i, new BulletPoint { Text = output[i] });
            }

            
            //clear any possible redos beyond the current state from UndoBuffer
            while(UndoCounter + 1 < UndoBuffer.Count)
            {
                UndoBuffer.RemoveAt(UndoCounter + 1);
            }
            
        }

        public void OnBPEditorCompleted(object sender, EventArgs e)
        {
            SaveContent();
            AddCurrentStateToUndoBuffer();
        }

        //TODO: pass in arg so onlyPush can be true if it just adds one
        public void OnBulletPointsCollectionChanged(object sender, EventArgs e)
        {
            UpdateDisplays(false);
        }

        public void SaveContent() 
        {
            JsonSerializerOptions options = new JsonSerializerOptions { 
                NumberHandling =  JsonNumberHandling.AllowNamedFloatingPointLiterals, //let it be known that IntelliJ suggested this and it fixed it
            };
            string data = JsonSerializer.Serialize(categories, options);
            string path = Path.Combine(FileSystem.Current.AppDataDirectory, "content.json");

            try
            {
                using (StreamWriter writer = new StreamWriter(path)) { 
                    writer.Write(data); 
                }
            }catch(Exception e)
            {
                DisplayAlert("Failed to save content", e.Message, "OK");
            }
            
        }

        private void LoadContent() 
        {
            string path = Path.Combine(FileSystem.Current.AppDataDirectory, "content.json");
            //Debug.WriteLine(path);

            try
            {
                if (File.Exists(path)) {
                    using(StreamReader reader = new StreamReader(path))
                    {
                        string contentJson = reader.ReadToEnd();
                        //Debug.Write(contentJson);
                        categories = JsonSerializer.Deserialize<ObservableCollection<Category>>(contentJson);
                    }
                }
                else
                {
                    categories = new ObservableCollection<Category>();
                    return;
                }
            }catch(Exception e)
            {
                categories = new ObservableCollection<Category>();
                DisplayAlert("Failed to load content due to " + e.GetType(), e.Message, "OK");
            }
        }

        private void UndoButton_Clicked(object sender, EventArgs e) //TODO: make it so this keeps pin button state
        {
            if((UndoCounter - 1) >= 0)
            {
                UndoCounter--; //important to have this at the beginning
                currentList = UndoBuffer[UndoCounter].Clone();
                currentCategory.lists[currentListIndex] = currentList; //this is important, makes it so everything is saved after undoing
                UpdateBulletPointDisplays(false);
                SaveContent();
            }
        }

        private void RedoButton_Clicked(object sender, EventArgs e)
        {
            if ((UndoCounter + 1) < UndoBuffer.Count)
            {
                UndoCounter++; //important to have this at the beginning
                currentList = UndoBuffer[UndoCounter].Clone();
                currentCategory.lists[currentListIndex] = currentList;
                UpdateBulletPointDisplays(false);
                SaveContent();
            }
        }

        public void AddCurrentStateToUndoBuffer()
        {
            UndoBuffer.Add(currentList.Clone());
            ++UndoCounter;
        }

        void SortByPinned()
        {
            int pinnedAtTopEndIndex = 0;
            dynamic[] collections = { categories, currentCategory?.lists, currentList?.bulletPoints };

            foreach (ListableElement item in collections[mode])
            {
                if (!item.IsPinned)
                    break;
                ++pinnedAtTopEndIndex;
            }

            for (int i = pinnedAtTopEndIndex; i < collections[mode].Count; ++i)
            {
                if (collections[mode][i].IsPinned)
                {
                    collections[mode].Move(i, pinnedAtTopEndIndex); //this method is specific to ObservableCollection so im in luck
                    ++pinnedAtTopEndIndex;
                }
            }

        }
    }

}
