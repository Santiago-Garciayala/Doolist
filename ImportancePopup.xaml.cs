namespace Doolist;

public partial class ImportancePopup : Mopups.Pages.PopupPage
{
	BulletPoint bp { get; set; }
	bool initialized { get; set; } = false;

	public ImportancePopup(BulletPoint point)
	{
		InitializeComponent();

		bp = point;
		importanceSlider.Value = bp.Importance;
        importanceLabel.Text = bp.Importance.ToString();

		this.BackgroundClicked += (s, e) => { MainPage.MainPageInstance.UpdateBulletPointDisplays(false); };

        initialized = true;
    }


    private void importanceSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
		if (initialized)
		{
			Slider slider = (Slider)sender;

			bp.Importance = (int)Math.Round(slider.Value);
			importanceLabel.Text = bp.Importance.ToString();
		}
    }
}