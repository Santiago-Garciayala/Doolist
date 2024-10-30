using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doolist
{
    public class Category
    {
        private ObservableCollection<TodoList> lists { get; } = new ObservableCollection<TodoList>();
        public string title { get; set; } = "Title";
        public bool IsPinned { get; set; } = false;
        public string CountDisplay { get; set; }


        public Category(string title) { 
            this.title = title;
            CountDisplay = lists.Count + " notes";
            lists.CollectionChanged += (s, e) => { CountDisplay = lists.Count + " notes"; };
        }

        public void AddList(TodoList list) { 
            lists.Add(list);
        }

        public void RemoveList(TodoList list) { 
            lists.Remove(list);
        }

    }
}
