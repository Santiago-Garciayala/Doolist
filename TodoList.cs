using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doolist
{
    public class TodoList : ListableElement
    {
        public ObservableCollection<BulletPoint> bulletPoints { get; set; } = new ObservableCollection<BulletPoint>();
        public string Title { get; set; } = "";
        public bool IsPinned { get; set; } = false;

        public TodoList()
        {
            bulletPoints.Add(new BulletPoint());
            bulletPoints.CollectionChanged += MainPage.MainPageInstance.OnBulletPointsCollectionChanged;
        }

        public TodoList Clone()
        {
            TodoList clone = new TodoList();
            clone.bulletPoints.Clear();

            foreach (BulletPoint point in bulletPoints)
            {
                clone.bulletPoints.Add(point.Clone());
            }

            clone.Title = Title;
            clone.IsPinned = IsPinned;

            return clone;
        }
    }
}
