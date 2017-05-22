using System.Windows;
using System.Windows.Controls;

namespace issb
{
    public class Toolbox : ItemsControl
    {
        private Size _DefaultItemSize = new Size(50, 50);

        public Size DefaultItemSize
        {
            get { return _DefaultItemSize; }
            set { _DefaultItemSize = value; }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolboxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ToolboxItem);
        }
    }
}
