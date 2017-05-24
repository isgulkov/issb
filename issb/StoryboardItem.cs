using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace issb
{
    /// <summary>
    /// Представляет собой элемент раскадровки, который можно размещать на рабочем холсте
    /// </summary>
    public class StoryboardItem : ContentControl
    {
        /// <summary>
        /// Выделен ли данный элемент
        /// </summary>
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
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StoryboardItem), new FrameworkPropertyMetadata(typeof(StoryboardItem)));
        }

        public StoryboardItem()
        {
            Loaded += new RoutedEventHandler(StoryboardItem_Loaded);

            Panel.SetZIndex(this, int.MinValue + 1);
        }

        /// <summary>
        /// Обрабатывает событие нажатия мыши на данном элементе раскадровки с целью поддержки множественного выделения элементов раскадровки
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs eventArgs)
        {
            base.OnPreviewMouseDown(eventArgs);
            StoryboardCanvas canvas = VisualTreeHelper.GetParent(this) as StoryboardCanvas;

            if(canvas != null) {
                if((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None) {
                    IsSelected = !IsSelected;
                }
                else {
                    if(!IsSelected) {
                        canvas.DeselectAll();
                        IsSelected = true;
                    }
                }
            }

            eventArgs.Handled = false;
        }

        /// <summary>
        /// Устанавливает для данного элемента раскадровки шаблон, добавляющий к нему элемент управления <see cref="DragThumb"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void StoryboardItem_Loaded(object sender, RoutedEventArgs eventArgs)
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

        /// <summary>
        /// Обрабатывает двойной щелчок мыши по данном элементу, перемещая его на холсте на передний план
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs eventArgs)
        {
            base.OnMouseDoubleClick(eventArgs);

            StoryboardCanvas canvas = VisualTreeHelper.GetParent(this) as StoryboardCanvas;

            if(canvas != null) {
                canvas.BringToFront(this);
            }
        }
    }
}
