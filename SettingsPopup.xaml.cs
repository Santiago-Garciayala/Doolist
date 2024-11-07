

using Mopups.Services;

namespace Doolist;

public partial class SettingsPopup
{
    public SettingsPopup(VisualElement parentOfSender)
    {
        InitializeComponent();

        stack.WidthRequest = MainPage.MainPageInstance.Width * .16;
        
        TapGestureRecognizer TGR = new TapGestureRecognizer();
        TGR.Tapped += (s, e) => { MopupService.Instance.PopAsync(); };
        absLayout.GestureRecognizers.Add(TGR);

        //magic values that probably only work for windows, but the buttons are not being rendered on android anyways
        //had to use the parent and not the sender itself cuz otherwise it wasnt changing the position at all
        double X = parentOfSender.X + parentOfSender.Width * .84; 
        double Y = parentOfSender.Y + parentOfSender.Height * 1.7;
        AbsoluteLayout.SetLayoutBounds(stack, new Rect(X, Y, stack.Width, stack.Height));

        
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
            Margin = 0,
            FontSize = 12,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            BorderWidth = 2,
            BorderColor = Color.FromArgb("#FF000000"),
            CornerRadius = 0
        };

        stack.Add(btn);
        return btn;
    }
}