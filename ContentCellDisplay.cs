using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doolist
{
    public class ContentCellDisplay : Grid
    {
        public dynamic source { get; set; }

        public ContentCellDisplay()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            WidthRequest = MainPage.MainPageInstance.Width;
        }
    }
}
