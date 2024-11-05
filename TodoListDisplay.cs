
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doolist
{
    internal class TodoListDisplay : ContentCellDisplay
    {
        public MainPage mainPage { get; set; }

        public TodoListDisplay(TodoList src, MainPage mainP)
        {
            this.source = src;
            this.mainPage = mainP;

            ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                };

            RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                };
            //ColumnSpacing = 3,
            //RowSpacing = 3,

            //the compiler was complaining about an ambiguous reference to Colors so had to use hex
            BackgroundColor = Color.FromArgb("#FFF0F8FF"); //AliceBlue
            Margin = new Thickness(10, 10, 10, 0);


            Label titleLabel = new Label
            {
                Text = source.Title,
                FontAttributes = FontAttributes.Bold,
                FontSize = 40
            };
            this.Add(titleLabel, 0, 0);

            //TODO: Create preview for bullet points
            Label previewLabel = new Label { Text = "Preview" };
            this.Add(previewLabel, 0, 1);

            ImageButton pinned = new ImageButton
            {
                Source = "pin.png",
                Opacity = source.IsPinned ? 1 : 0.1,
                Padding = 5,
                BackgroundColor = Color.FromArgb("#00000000")
            };
            pinned.Loaded += mainPage.ResizeTemplateButton;
            pinned.Clicked += mainPage.OnPinButtonClicked;
            this.Add(pinned, 2, 0);

            ImageButton settingsBtn = new ImageButton
            {
                Source = "threedots.png",
                Padding = 5,
                BackgroundColor = Color.FromArgb("#00000000")
            };
            settingsBtn.Loaded += mainPage.ResizeTemplateButton;
            settingsBtn.Pressed += mainPage.DisplayListSettings;
            this.Add(settingsBtn, 2, 1);

            TapGestureRecognizer TGR = new TapGestureRecognizer();
            TGR.Tapped += (s, e) => { mainPage.SwitchToBulletPointsMode(source); };
            this.GestureRecognizers.Add(TGR);
        }
    }
}
