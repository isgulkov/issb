using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace issb
{
    /// <summary>
    /// Визуальный декоратор (adorner), отображающий элементы управления, связанные с изменением размера и вращением элемента раскадровки
    /// </summary>
    public class ResizeRotateAdorner : Adorner
    {
        /// <summary>
        /// Коллекция дочерних элементов визуального декоратора
        /// </summary>
        private VisualCollection Visuals;

        /// <summary>
        /// Единственный дочерний элемент данного визуального декоратора, на котором с помощью соответствующего стиля отображаются элементы управления 
        /// </summary>
        private ResizeRotateChrome Chrome;

        /// <summary>
        /// Возвращает число дочерних элементов данного визуального декоратора (необходимо для реализации визуального декоратора в WPF)
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return Visuals.Count;
            }
        }

        /// <summary>
        /// Создает визуальный декоратор для данного элемента раскадровки
        /// </summary>
        /// <param name="storyboardItem">Элемент раскадровки, для которого предполагается создать визуальный декоратор</param>
        public ResizeRotateAdorner(ContentControl storyboardItem) : base(storyboardItem)
        {
            SnapsToDevicePixels = true;
            Chrome = new ResizeRotateChrome();
            Chrome.DataContext = storyboardItem;
            Visuals = new VisualCollection(this);
            Visuals.Add(Chrome);
        }


        /// <summary>
        /// Перестраивает в пространстве дочерние элементы данного визуального декоратора при перестроении его его родительскими элементом (необходимо для реализации визуального декоратора в WPF)
        /// </summary>
        /// <param name="arrangeBounds">Границы расположения элемента</param>
        /// <returns>Границы расположения элемента</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        /// <summary>
        /// Возвращает дочерний элемент с переданным индексом (необходимо для реализации визуального декоратора в WPF)
        /// </summary>
        /// <param name="index">Индекс дочернего элемента</param>
        /// <returns>Дочерний элемент с переданным индексом</returns>
        protected override Visual GetVisualChild(int index)
        {
            return Visuals[index];
        }
    }
}
