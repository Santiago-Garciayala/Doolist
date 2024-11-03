using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doolist
{
    public class BulletPoint : ListableElement
    {
        public string Text { get; set; } = "";
        public int Importance { get; set; } = 1;
        public bool IsDone { get; set; } = false;

        public BulletPoint Clone()
        {
            BulletPoint clone = new BulletPoint();
            clone.Text = Text;
            clone.Importance = Importance;
            clone.IsDone = IsDone;

            return clone;
        }
    }
}
