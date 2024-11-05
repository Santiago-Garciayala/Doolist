namespace Doolist;

public partial class ImportancePopup : Mopups.Pages.PopupPage
{
	public BulletPoint bp { get; set; }

	public ImportancePopup(BulletPoint point)
	{
		InitializeComponent();

		bp = point;
		importanceSlider.Value = bp.Importance;
        importanceLabel.Text = bp.Importance.ToString();
    }


    private void importanceSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
		Slider slider = (Slider)sender;

		bp.Importance = (int)Math.Round(slider.Value); //why is it null???????? FIX
		importanceLabel.Text = bp.Importance.ToString();
    }
}