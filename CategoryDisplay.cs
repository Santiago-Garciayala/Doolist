
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doolist
{
    internal class CategoryDisplay : ContentCellDisplay
    {
        public MainPage mainPage { get; set; }

        public CategoryDisplay(Category src, MainPage mainP) {
            source = src;
            mainPage = mainP;

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
                Text = source.title,
                FontAttributes = FontAttributes.Bold,
                FontSize = 40
            };
            this.Add(titleLabel, 0, 0);

            Label countLabel = new Label { Text = source.CountDisplay };
            this.Add(countLabel, 0, 1);

            ImageButton pinned = new ImageButton
            {
                Source = "pin.png",
                Opacity = source.IsPinned ? 1 : 0.1,
                Padding = 5,
                BackgroundColor = Color.FromArgb("#00000000"),
                HorizontalOptions = LayoutOptions.Start
            };
            pinned.Loaded += mainPage.ResizeTemplateButton;
            pinned.Clicked += mainPage.OnPinButtonClicked;
            this.Add(pinned, 2, 0);

            ImageButton settingsBtn = new ImageButton();
            settingsBtn.Source = "threedots.png";
            settingsBtn.Padding = 5;
            settingsBtn.BackgroundColor = Color.FromArgb("#00000000");
            settingsBtn.HorizontalOptions = LayoutOptions.Start;

            settingsBtn.Loaded += mainPage.ResizeTemplateButton;
            settingsBtn.Pressed += mainPage.DisplayCategorySettings;
            this.Add(settingsBtn, 2, 1);
            
            TapGestureRecognizer TGR = new TapGestureRecognizer();
            TGR.Tapped += (s, e) => { mainPage.SwitchToNotesMode(source); };
            this.GestureRecognizers.Add(TGR);

            
        }


    }
}
