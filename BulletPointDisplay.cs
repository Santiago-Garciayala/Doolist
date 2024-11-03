﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doolist
{
    internal class BulletPointDisplay : ContentCellDisplay
    {
        public BulletPoint source {  get; set; }
        public MainPage mainPage { get; set; }

        public BulletPointDisplay(BulletPoint source, MainPage mainPage)
        {
            this.source = source;
            this.mainPage = mainPage;

            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Auto) },
                new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Auto) },
                new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Auto) }
            };

            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition{Height = new GridLength (1, GridUnitType.Auto) }
            };

            BackgroundColor = Color.FromArgb("#FFF0F8FF"); //AliceBlue

            //resize this later
            CheckBox checkBox = new CheckBox { IsChecked = source.IsDone };
            checkBox.CheckedChanged += CheckBoxCheckedChanged;
            this.Add(checkBox, 0, 0);

            Editor editor = new Editor
            {
                BackgroundColor = Color.FromArgb("#00000000"),
                Placeholder = "",
                Text = source.Text
            };
            editor.TextChanged += mainPage.OnBPEditorTextChanged;
            editor.Completed += (s, e) => { mainPage.SaveContent(); };
            this.Add(editor, 1, 0);

            ImageButton trash = new ImageButton
            {
                Source = "trash.png",
                Padding = 5,
                BackgroundColor = Color.FromArgb("#00000000"),
            };
            trash.Loaded += mainPage.ResizeTemplateButton;
            trash.Pressed += mainPage.DeleteBulletPoint;
            this.Add(trash, 2, 0);

            Button importance = new Button
            {
                Text = source.Importance.ToString(),
                Padding = 5,
                BackgroundColor = Color.FromArgb("#00000000"),
                TextColor = Color.FromArgb("#FF000000"),
                FontAttributes = FontAttributes.Bold,
                BorderColor = Color.FromArgb("#FF000000"),
                BorderWidth = 3,
                CornerRadius = 30
            };
            importance.Loaded += mainPage.ResizeTemplateButton;
            importance.Pressed += DisplayImportanceMenu;
            this.Add(importance, 3, 0);
        }

        private void CheckBoxCheckedChanged(object? sender, CheckedChangedEventArgs e)
        {
            source.IsDone = ((CheckBox)sender).IsChecked;
        }

        private void DisplayImportanceMenu(object? sender, EventArgs e)
        {
            //TODO
        }
    }
}
