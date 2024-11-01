using System.Collections.ObjectModel;

namespace Doolist
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Category> categories = new ObservableCollection<Category>();
        public Category currentCategory;
        public TodoList currentList;

        //determines what type of page is displayed cuz i didnt wanna figure out how to navigate with AppShell
        //0 for categories, 1 for lists, 2 for specific list 
        private int mode = 0; 


        public MainPage()
        {
            InitializeComponent();

            this.LayoutChanged += OnWindowChanged;
            AddButton.Pressed += OnAddButtonPressed;
            BackButton.Pressed += OnBackButtonPressed;

            //I didnt really need to use an ObservableCollection here since im not using a CollectionView anymore but since its there i might as well do this
            categories.CollectionChanged += (s, e) => { UpdateDisplays(false); };

            categories.Add(new Category("Test"));
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
                    break;
                case 1:
                    TodoList list = new TodoList();
                    list.bulletPoints.CollectionChanged += OnBulletPointsCollectionChanged;
                    currentList = list;
                    currentCategory.lists.Add(list);
                    SwitchToBulletPointsMode(list);
                    break;
                default:
                    DisplayAlert("Alert", "You shouldnt be able to see this button", "OK");
                    break;
            }

        }

        void OnBackButtonPressed(object sender, EventArgs e)
        {
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

        public void ResizeTemplateButton(object sender, EventArgs e)
        {
            VisualElement btn = (VisualElement)sender;

            double height = ((Grid)btn.Parent).Height;
            double width = ((Grid)btn.Parent).Width;

            btn.HeightRequest = height * .04;
            btn.WidthRequest = height * .04;
        }

        void ResizeTemplateButtons(Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if(child.GetType() == typeof(ImageButton) || child.GetType() == typeof(Button))
                {
                    ResizeTemplateButton(child, new EventArgs());
                }
            }
        }
        public void DisplayCategorySettings(object sender, EventArgs e)
        {
            //TODO
        }

        public void DisplayListSettings(object sender, EventArgs e)
        {
            //TODO
        }

        void UpdateCategoryDisplays()
        {
            //this came before UpdateDisplays was abstracted to all 3 modes and is still used as a failsafe in UpdateDisplays

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

        public void SwitchToCategoriesMode()
        {
            mode = 0;

            BackButton.IsEnabled = false;
            BackButton.IsVisible = false;
            RedoButton.IsEnabled = false;
            RedoButton.IsVisible = false;
            UndoButton.IsEnabled = false;
            UndoButton.IsVisible = false;
            AddButton.IsEnabled = true;
            AddButton.IsVisible = true;

            ContentCell.Clear();
            UpdateCategoryDisplays(); //still uses UpdateCategoryDisplays instead of UpdateDisplays in case the latter fails

            //add more stuff as more functionality comes along
        }

        public void SwitchToNotesMode(Category category)
        {
            mode = 1;
            currentCategory = category;

            BackButton.IsEnabled = true;
            BackButton.IsVisible = true;
            RedoButton.IsEnabled = false;
            RedoButton.IsVisible = false;
            UndoButton.IsEnabled = false;
            UndoButton.IsVisible = false;
            AddButton.IsEnabled = true;
            AddButton.IsVisible = true;

            ContentCell.Clear();
            UpdateDisplays(false);
        }

        public void SwitchToBulletPointsMode(TodoList todoList)
        {
            mode = 2;
            currentList = todoList;

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
            UpdateDisplays(false);

            //TODO
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

        public void OnBPEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            Editor editor = (Editor)sender;
            BulletPoint point = ((BulletPointDisplay)editor.Parent).source;

            char[] delimiters = { '\n', '\r', '\t' };
            string[] output = e.NewTextValue.Split(delimiters);

            point.Text = output[0];

            //if output length > 1 creates a new BPDisplay for every delimiter char entered (normally will only be one unless the paste smth with more than 1)
            for (int i = 1; i < output.Length; i++) {
                currentList.bulletPoints.Insert(currentList.bulletPoints.IndexOf(point) + i, new BulletPoint { Text = output[i] });
            }
        }

        //TODO: fix when deleting bp when length > 1
        public void DeleteBulletPoint(object sender, EventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            BulletPointDisplay parent = btn.Parent as BulletPointDisplay;
            currentList.bulletPoints.Remove(parent.source);
        }

        void OnBulletPointsCollectionChanged(object sender, EventArgs e)
        {
            UpdateDisplays(false);
        }
    }

}
