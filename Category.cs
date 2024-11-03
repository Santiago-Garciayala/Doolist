using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doolist
{
    public class Category : ListableElement
    {
        public ObservableCollection<TodoList> lists { get; set; } = new ObservableCollection<TodoList>();
        public string title { get; set; } = "Title";
        public bool IsPinned { get; set; } = false;
        public string CountDisplay { get; set; }


        public Category(string title) { 
            this.title = title;
            CountDisplay = lists.Count + " note(s)";
            lists.CollectionChanged += (s, e) => { CountDisplay = lists.Count + " note(s)"; };
        }

    }
}
