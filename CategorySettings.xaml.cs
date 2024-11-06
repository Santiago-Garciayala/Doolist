

namespace Doolist;

public partial class CategorySettings 
{
	public CategorySettings()
	{
		InitializeComponent();

		stack.WidthRequest = MainPage.MainPageInstance.Width * .2;

		Label SortAlphabeticallyButton = CreateFakeButton("Sort Alphabetically");
		((TapGestureRecognizer)SortAlphabeticallyButton.GestureRecognizers[0]).Tapped += OnSortAlphabeticallyButtonClicked;
        stack.Add(SortAlphabeticallyButton);
    }

	//this does NOT handle event handlers associated with a button, do that separately
	//its a fake button cuz for some reason Buttons and ImageButtons dont work on android?
	//it STILL doesnt work on android - FIX
	Label CreateFakeButton(string name)
	{
		Label btn = new Label
		{
			Text = name,
			TextColor = Color.FromArgb("#FF000000"),
            BackgroundColor = Color.FromArgb("#FF888888"),
			Padding = new Thickness(50, 0, 50, 0),
			Margin = 3,
			FontSize = 12,
			HorizontalOptions = LayoutOptions.CenterAndExpand
		};

        TapGestureRecognizer TGR = new TapGestureRecognizer();
		btn.GestureRecognizers.Add(TGR);

        return btn;
	}

	void OnSortAlphabeticallyButtonClicked(object sender, EventArgs e) {
		//TODO
		DisplayAlert("aaaa", "alert", "d");
	}
}