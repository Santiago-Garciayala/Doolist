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

            BindingContext = this;
            categories.Add(new Category("Test"));
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
                    Grid display = CreateCategoryDisplay(cat);
                    categories.Add(cat);
                    ContentCell.Add(display);
                    ResizeTemplateButtons(display);
                    break;
                case 1:
                    currentCategory.AddList(new TodoList());
                    break;
                default:
                    DisplayAlert("Alert", "You shouldnt be able to see this button", "OK");
                    break;
            }
        }

        void ResizeTemplateButton(object sender, EventArgs e)
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
        private void DisplayCategorySettings(object sender, EventArgs e)
        {
            //TODO
        }

        private Grid CreateCategoryDisplay(Category category)
        {
            Grid display = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Auto)},
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Auto)}
                },
                RowDefinitions =
                {
                    new RowDefinition{ Height = new GridLength (1, GridUnitType.Auto)},
                    new RowDefinition{ Height = new GridLength (1, GridUnitType.Auto)}
                },
                //ColumnSpacing = 3,
                //RowSpacing = 3,
                BackgroundColor = Colors.AliceBlue,
                Margin = new Thickness(10, 10, 10, 0)
            };

            Label titleLabel = new Label
            {
                Text = category.title,
                FontAttributes = FontAttributes.Bold,
                FontSize = 40
            };
            display.Add(titleLabel, 0, 0);

            Label countLabel = new Label { Text = category.CountDisplay };
            display.Add(countLabel, 0, 1);

            ImageButton pinned = new ImageButton
            {
                Source = "pin.png",
                Padding = 5,
                BackgroundColor = Colors.Transparent
            };
            pinned.Loaded += ResizeTemplateButton;
            display.Add(pinned, 2, 0);
            //ResizeTemplateButton(pinned, new EventArgs());

            ImageButton settingsBtn = new ImageButton
            {
                Source = "threedots.png",
                Padding = 5,
                BackgroundColor = Colors.Transparent
            };
            settingsBtn.Loaded += ResizeTemplateButton;
            settingsBtn.Pressed += DisplayCategorySettings;
            display.Add(settingsBtn, 2, 1);
            //ResizeTemplateButton(settingsBtn, new EventArgs());

            return display;
        }
    }

}
