using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace issb
{
    public class DragThumb : Thumb
    {
        StoryboardItem CurrentItem;
        RotateTransform CurrentTransForm;
        StoryboardCanvas CurrentCanvas;

        private void DragThumb_DragStarted(object sender, DragStartedEventArgs eventArgs)
        {
            CurrentItem = DataContext as StoryboardItem;

            if(CurrentItem != null) {
                CurrentCanvas = VisualTreeHelper.GetParent(CurrentItem) as StoryboardCanvas;
                CurrentTransForm = CurrentItem.RenderTransform as RotateTransform;
            }
        }

        private void DragThumb_DragDelta(object sender, DragDeltaEventArgs eventArgs)
        {
            if(CurrentItem != null && CurrentCanvas != null && CurrentItem.IsSelected) {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;

                foreach(StoryboardItem item in CurrentCanvas.SelectedItems) {
                    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                    minTop = Math.Min(Canvas.GetTop(item), minTop);
                }

                double dHorizontal = Math.Max(-minLeft, eventArgs.HorizontalChange);
                double dVertical = Math.Max(-minTop, eventArgs.VerticalChange);

                Point dragDelta = new Point(dHorizontal, dVertical);

                if(CurrentTransForm != null) {
                    dragDelta = CurrentTransForm.Transform(dragDelta);
                }

                foreach(StoryboardItem item in CurrentCanvas.SelectedItems) {
                    Canvas.SetLeft(item, Canvas.GetLeft(item) + dragDelta.X);
                    Canvas.SetTop(item, Canvas.GetTop(item) + dragDelta.Y);
                }

                eventArgs.Handled = true;
            }
        }

        public DragThumb()
        {
            DragStarted += new DragStartedEventHandler(DragThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(DragThumb_DragDelta);
        }
    }
}
