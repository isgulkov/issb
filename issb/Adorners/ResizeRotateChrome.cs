using System.Windows;
using System.Windows.Controls;

namespace issb
{
    public class ResizeRotateChrome : Control
    {
        static ResizeRotateChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeRotateChrome), new FrameworkPropertyMetadata(typeof(ResizeRotateChrome)));
        }
    }
}
