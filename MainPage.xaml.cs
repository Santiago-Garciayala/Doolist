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
        }

        void OnWindowChanged(object sender, EventArgs e)
        {
            ResizeTopBar();
            ResizeAddButton();
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

        async void OnAddButtonPressed(object sender, EventArgs e)
        {
            switch (mode)
            {
                case 0:
                    string name = await DisplayPromptAsync("", "Enter a name for your category");
                    if (name == null) { name = "Category"; };
                    categories.Add(new Category(name));
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

            btn.HeightRequest = height * .075;
            btn.WidthRequest = height * .075;
        }

    }

}
