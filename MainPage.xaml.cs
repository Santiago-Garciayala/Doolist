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

            categories.Add(new Category("Test"));
            UpdateCategoryDisplays();
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
                    UpdateCategoryDisplays();
                    break;
                case 1:
                    currentCategory.AddList(new TodoList());
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

        void UpdateCategoryDisplays()
        {
            bool onlyPush = true;

            for(int i = 0; i < categories.Count - 1 || categories.Count == 0; ++i) {
                if (categories[i] != ((CategoryDisplay)ContentCell.Children[i]).category)
                {
                    onlyPush = false; 
                    break;
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
    }

}
