using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace issb
{
    /// <summary>
    /// Элемент-декоратор, добавляющий элементу раскадровки поддержку изменения размера и вращения
    /// </summary>
    public class ResizeRotateDecorator : Control
    {
        /// <summary>
        /// Визуальный декоратор (adorner), представляющий собой элементы управления для изменения его размера и вращения элемента раскадровки
        /// </summary>
        private Adorner Adorner;

        /// <summary>
        /// Отображать ли визуальный декоратор с элементами управления
        /// </summary>
        public bool ShowDecorator
        {
            get { return (bool)GetValue(ShowDecoratorProperty); }
            set { SetValue(ShowDecoratorProperty, value); }
        }

        public static readonly DependencyProperty ShowDecoratorProperty =
            DependencyProperty.Register(
                "ShowDecorator",
                typeof(bool), typeof(ResizeRotateDecorator),
                new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowDecoratorProperty_Changed))
                );

        public ResizeRotateDecorator()
        {
            Unloaded += new RoutedEventHandler(DesignerItemDecorator_Unloaded);
        }

        /// <summary>
        /// Скрыть визуальный декоратор
        /// </summary>
        private void HideAdorner()
        {
            if(Adorner != null) {
                Adorner.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Отобразить визуальный декоратор
        /// </summary>
        private void ShowAdorner()
        {
            if(Adorner == null) {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

                if(adornerLayer != null) {
                    ContentControl designerItem = DataContext as ContentControl;
                    Canvas canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;
                    Adorner = new ResizeRotateAdorner(designerItem);
                    adornerLayer.Add(Adorner);

                    if(ShowDecorator) {
                        Adorner.Visibility = Visibility.Visible;
                    }
                    else {
                        Adorner.Visibility = Visibility.Hidden;
                    }
                }
            }
            else {
                Adorner.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Скрывает и удаляет из памяти визуальный декоратор при выгрузке данного элемента-декоратора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void DesignerItemDecorator_Unloaded(object sender, RoutedEventArgs eventArgs)
        {
            if(Adorner != null) {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if(adornerLayer != null) {
                    adornerLayer.Remove(Adorner);
                }

                Adorner = null;
            }
        }

        /// <summary>
        /// Скрывает или отображает визуальный декоратор с элементами управления при изменении свойства <see cref="ShowDecorator"/> данного элемента
        /// </summary>
        /// <param name="d"></param>
        /// <param name="eventArgs"></param>
        private static void ShowDecoratorProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
        {
            ResizeRotateDecorator decorator = (ResizeRotateDecorator)d;
            bool showDecorator = (bool)eventArgs.NewValue;

            if(showDecorator) {
                decorator.ShowAdorner();
            }
            else {
                decorator.HideAdorner();
            }
        }
    }
}
