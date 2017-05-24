using System.Windows;
using System.Windows.Controls;

namespace issb
{
    /// <summary>
    /// Представляет собой панель инструментов, содержащую элементы, которые можно отсюда перетаскивать на рабочий холст, добавляя туда тем самым элементы раскадровки и изображения-фоны
    /// </summary>
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
