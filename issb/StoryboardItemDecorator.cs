using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace issb
{
    public class StoryboardItemDecorator : Control
    {
        private Adorner Adorner;

        public bool ShowDecorator
        {
            get { return (bool)GetValue(ShowDecoratorProperty); }
            set { SetValue(ShowDecoratorProperty, value); }
        }

        public static readonly DependencyProperty ShowDecoratorProperty =
            DependencyProperty.Register(
                "ShowDecorator",
                typeof(bool), typeof(StoryboardItemDecorator),
                new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowDecoratorProperty_Changed))
                );

        public StoryboardItemDecorator()
        {
            Unloaded += new RoutedEventHandler(this.DesignerItemDecorator_Unloaded);
        }

        private void HideAdorner()
        {
            if(Adorner != null) {
                Adorner.Visibility = Visibility.Hidden;
            }
        }

        private void ShowAdorner()
        {
            if(Adorner == null) {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

                if(adornerLayer != null) {
                    ContentControl designerItem = DataContext as ContentControl;
                    Canvas canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;
                    Adorner = new ResizeAdorner(designerItem);
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

        private void DesignerItemDecorator_Unloaded(object sender, RoutedEventArgs e)
        {
            if(Adorner != null) {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if(adornerLayer != null) {
                    adornerLayer.Remove(Adorner);
                }

                Adorner = null;
            }
        }

        private static void ShowDecoratorProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StoryboardItemDecorator decorator = (StoryboardItemDecorator)d;
            bool showDecorator = (bool)e.NewValue;

            if(showDecorator) {
                decorator.ShowAdorner();
            }
            else {
                decorator.HideAdorner();
            }
        }
    }
}
