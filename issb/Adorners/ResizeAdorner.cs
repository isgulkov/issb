using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace issb
{
    public class ResizeAdorner : Adorner
    {
        private VisualCollection Visuals;
        private ResizeChrome Chrome;

        protected override int VisualChildrenCount
        {
            get
            {
                return Visuals.Count;
            }
        }

        public ResizeAdorner(ContentControl designerItem) : base(designerItem)
        {
            SnapsToDevicePixels = true;
            Chrome = new ResizeChrome();
            Chrome.DataContext = designerItem;
            Visuals = new VisualCollection(this);
            Visuals.Add(Chrome);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return Visuals[index];
        }
    }
}
