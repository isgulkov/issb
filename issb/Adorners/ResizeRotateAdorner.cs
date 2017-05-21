using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace issb
{
    public class ResizeRotateAdorner : Adorner
    {
        private VisualCollection Visuals;
        private ResizeRotateChrome Chrome;

        protected override int VisualChildrenCount
        {
            get
            {
                return Visuals.Count;
            }
        }

        public ResizeRotateAdorner(ContentControl designerItem) : base(designerItem)
        {
            SnapsToDevicePixels = true;
            Chrome = new ResizeRotateChrome();
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
