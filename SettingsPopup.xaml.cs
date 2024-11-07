

using Mopups.Services;

namespace Doolist;

public partial class SettingsPopup
{
    public SettingsPopup(double X, double Y)
    {
        InitializeComponent();

        stack.WidthRequest = MainPage.MainPageInstance.Width * .16;
        
        TapGestureRecognizer TGR = new TapGestureRecognizer();
        TGR.Tapped += (s, e) => { MopupService.Instance.PopAsync(); };
        absLayout.GestureRecognizers.Add(TGR);

        AbsoluteLayout.SetLayoutBounds(stack, new Rect(X - stack.Width, Y - stack.Height, stack.Width, stack.Height));
    }

    //this does NOT handle event handlers associated with a button, do that separately
    protected Button CreateButton(string name)
    {
        Button btn = new Button
        {
            Text = name,
            TextColor = Color.FromArgb("#FF000000"),
            BackgroundColor = Color.FromArgb("#FF888888"),
            Padding = new Thickness(50, 0, 50, 0),
            Margin = 1,
            FontSize = 12,
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            BorderWidth = 3,
            BorderColor = Color.FromArgb("#FF000000")
        };

        stack.Add(btn);
        return btn;
    }
}