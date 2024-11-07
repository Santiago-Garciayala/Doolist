using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Doolist
{
    internal class ListSettingsPopup : SettingsPopup
    {
        public TodoList list { get; set; }

        public ListSettingsPopup(TodoList l, VisualElement parentOfSender) : base(parentOfSender)
        {
            this.list = l;

            Button SortByImportanceButton = CreateButton("Sort by Importance");
            SortByImportanceButton.Clicked += OnSortByImportanceButtonClicked;

            Button SortAlphabeticallyButton = CreateButton("Sort Alphabetically");
            SortAlphabeticallyButton.Clicked += OnSortAlphabeticallyButtonClicked;
        }

        void OnSortByImportanceButtonClicked(object sender, EventArgs e)
        {
            list.bulletPoints = SortByImportance(list.bulletPoints);
            MainPage.MainPageInstance.UpdateDisplays(false);
            MainPage.MainPageInstance.SaveContent();
            if (MainPage.MainPageInstance.mode == 2)
                MainPage.MainPageInstance.AddCurrentStateToUndoBuffer();
        }

        ObservableCollection<BulletPoint> SortByImportance(ObservableCollection<BulletPoint> bulletPoints)
        {
            if (bulletPoints[0].IsPinned != bulletPoints.Last().IsPinned) {
                ObservableCollection<BulletPoint> pinnedList = new ObservableCollection<BulletPoint>();
                ObservableCollection<BulletPoint> unpinnedList = new ObservableCollection<BulletPoint>();
                int i = 0;
                while (bulletPoints[i].IsPinned) { 
                    pinnedList.Add(bulletPoints[i]);
                    ++i;
                }
                while(i < bulletPoints.Count)
                {
                    unpinnedList.Add(bulletPoints[i]);
                    ++i;
                }
                return new ObservableCollection<BulletPoint>(SortByImportance(pinnedList).Concat(SortByImportance(unpinnedList))); //ugly line but idc i wanted to use recursion
            }

            //bubblesort 
            BulletPoint temp;

            for (int i = 0; i < bulletPoints.Count; i++)
            {
                for (int j = 0; j < bulletPoints.Count - 1; j++)
                {
                    if (bulletPoints[j].Importance < bulletPoints[j + 1].Importance)
                    {
                        temp = bulletPoints[j + 1];
                        bulletPoints[j + 1] = bulletPoints[j];
                        bulletPoints[j] = temp;
                    }
                }
            }

            return bulletPoints;

        }

        void OnSortAlphabeticallyButtonClicked(object sender, EventArgs e)
        {
            list.bulletPoints = SortAlphabetically(list.bulletPoints);
            MainPage.MainPageInstance.UpdateDisplays(false);
            MainPage.MainPageInstance.SaveContent();
            if(MainPage.MainPageInstance.mode == 2)
                MainPage.MainPageInstance.AddCurrentStateToUndoBuffer();
        }

        ObservableCollection<BulletPoint> SortAlphabetically(ObservableCollection<BulletPoint> bulletPoints)
        {
            if (bulletPoints[0].IsPinned != bulletPoints.Last().IsPinned)
            {
                ObservableCollection<BulletPoint> pinnedList = new ObservableCollection<BulletPoint>();
                ObservableCollection<BulletPoint> unpinnedList = new ObservableCollection<BulletPoint>();
                int i = 0;
                while (bulletPoints[i].IsPinned)
                {
                    pinnedList.Add(bulletPoints[i]);
                    ++i;
                }
                while (i < bulletPoints.Count)
                {
                    unpinnedList.Add(bulletPoints[i]);
                    ++i;
                }
                return new ObservableCollection<BulletPoint>(SortAlphabetically(pinnedList).Concat(SortAlphabetically(unpinnedList))); //ugly line but idc i wanted to use recursion
            }

            //bubblesort 
            BulletPoint temp;

            for (int i = 0; i < bulletPoints.Count; i++)
            {
                for (int j = 0; j < bulletPoints.Count - 1; j++)
                {
                    if (bulletPoints[j].Text.CompareTo(bulletPoints[j + 1].Text) > 0)
                    {
                        temp = bulletPoints[j + 1];
                        bulletPoints[j + 1] = bulletPoints[j];
                        bulletPoints[j] = temp;
                    }
                }
            }

            return bulletPoints;
        }
    }
}
