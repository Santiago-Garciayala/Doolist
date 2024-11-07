
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doolist
{
    public class CategorySettingsPopup : SettingsPopup
    {
        Category category { get; set; }
        public CategorySettingsPopup(Category cat, VisualElement parentOfSender) : base(parentOfSender)
        {
            category = cat;

            Button RenameButton = CreateButton("Rename Category");
            RenameButton.Clicked += OnRenameButtonClicked;

            Button SortAlphabeticallyButton = CreateButton("Sort Alphabetically");
            SortAlphabeticallyButton.Clicked += OnSortAlphabeticallyButtonClicked;
        }

        async void OnRenameButtonClicked(object sender, EventArgs e) {
            string name = await DisplayPromptAsync("", "Enter a name for your category");
            if (name == null) { name = "Category"; };
            category.title = name;
            MainPage.MainPageInstance.UpdateCategoryDisplays(false);
            MainPage.MainPageInstance.SaveContent();
        }

        void OnSortAlphabeticallyButtonClicked(object sender, EventArgs e)
        {
            //i honestly cant be bothered to implement a sorting algorithm because we havent been taught any and im tired of this project
        }
    }
}
