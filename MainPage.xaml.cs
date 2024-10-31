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
            categories.CollectionChanged += (s, e) => { UpdateCategoryDisplays(); };

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
            ImageButton btn = (ImageButton)sender;

            double height = ((Grid)btn.Parent).Height;
            double width = ((Grid)btn.Parent).Width;

            btn.HeightRequest = height * .04;
            btn.WidthRequest = height * .04;
        }

        void ResizeTemplateButtons(Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if(child.GetType() == typeof(ImageButton))
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
            bool onlyPush = true; //true if all (except the last) elements in categories list match the ones being displayed 

            if(categories.Count > ContentCell.Children.Count - 1)
                onlyPush = false;

            if(categories.Count == 0)
                onlyPush = false;

            for(int i = 0; i < categories.Count - 1 && onlyPush; ++i) {
                if (categories[i] != ((CategoryDisplay)ContentCell.Children[i]).category)
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

        void UpdateTodoListDisplays()
        {
            bool onlyPush = true; //true if all (except the last) elements in currentCategory.lists list match the ones being displayed 

            if (currentCategory.lists.Count > ContentCell.Children.Count - 1)
                onlyPush = false;

            if (currentCategory.lists.Count == 0)
                onlyPush = false;

            for (int i = 0; i < currentCategory.lists.Count - 1 && onlyPush; ++i)
            {
                if (currentCategory.lists[i] != ((TodoListDisplay)ContentCell.Children[i]).list)
                {
                    onlyPush = false;
                }
            }

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

        void UpdateBulletPointDisplays()
        {
            //TODO
            //make it so it makes a new bullet point when the return key is pressed
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
            UpdateCategoryDisplays();

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
            UpdateTodoListDisplays();
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
            UpdateBulletPointDisplays();

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
            }

            currentList.Title = output;
        }

        public void DeleteBulletPoint(object sender, EventArgs e)
        {
            //TODO
        }

    }

}
