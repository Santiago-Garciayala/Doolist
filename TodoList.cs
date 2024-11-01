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
        public ObservableCollection<BulletPoint> bulletPoints { get; } = new ObservableCollection<BulletPoint>();
        public string Title { get; set; } = "";
        public bool IsPinned { get; set; } = false;

        public TodoList()
        {
            bulletPoints.Add(new BulletPoint());
        }
    }
}
