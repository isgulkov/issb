using System.Windows;
using System.Windows.Controls;

namespace issb
{
    /// <summary>
    /// Визуальный элемент, презначение которого в том, чтобы к нему применялся стиль, описанный на XAML, отображающий на нем элементы управления изменения размера и вращения элемента раскадровки (<see cref="ResizeThumb"/> и <see cref="RotateThumb"/>)
    /// </summary>
    public class ResizeRotateChrome : Control
    {
        /// <summary>
        /// Устанавливает такое значение свойства DefaultStyleKey данного класса, чтобы к его объектам применялись соответствующие стили
        /// </summary>
        static ResizeRotateChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeRotateChrome), new FrameworkPropertyMetadata(typeof(ResizeRotateChrome)));
        }
    }
}
