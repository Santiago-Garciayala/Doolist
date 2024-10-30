using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doolist
{
    public class TodoList
    {
        private ObservableCollection<BulletPoint> bulletPoints { get; } = new ObservableCollection<BulletPoint>();
        public string Title { get; set; } = "Title";
        public bool IsPinned { get; set; } = false;

        public void AddBulletPoint(BulletPoint point)
        {
            bulletPoints.Add(point);
        }

        public void RemoveBulletPoint(BulletPoint point) { 
            bulletPoints.Remove(point);
        }
    }
}
