using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace issb
{
    public class StoryboardItem : ContentControl
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected", typeof(bool), typeof(StoryboardItem), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(StoryboardItem));

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetMoveThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);
        }

        static StoryboardItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(StoryboardItem), new FrameworkPropertyMetadata(typeof(StoryboardItem)));
        }

        public StoryboardItem()
        {
            Loaded += new RoutedEventHandler(DesignerItem_Loaded);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs eventArgs)
        {
            base.OnPreviewMouseDown(eventArgs);
            StoryboardCanvas designer = VisualTreeHelper.GetParent(this) as StoryboardCanvas;

            if(designer != null) {
                if((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None) {
                    IsSelected = !IsSelected;
                }
                else {
                    if(!IsSelected) {
                        designer.DeselectAll();
                        IsSelected = true;
                    }
                }
            }

            eventArgs.Handled = false;
        }

        private void DesignerItem_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            if(Template != null) {
                ContentPresenter contentPresenter =
                    Template.FindName("PART_ContentPresenter", this) as ContentPresenter;

                DragThumb dragThumb = Template.FindName("PART_DragThumb", this) as DragThumb;

                if(contentPresenter != null && dragThumb != null) {
                    UIElement contentVisual =
                        VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;

                    if(contentVisual != null) {
                        ControlTemplate template =
                            GetDragThumbTemplate(contentVisual) as ControlTemplate;

                        if(template != null) {
                            dragThumb.Template = template;
                        }
                    }
                }
            }
        }
    }
}
