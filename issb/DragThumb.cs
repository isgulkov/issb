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
        StoryboardCanvas CurrentCanvas;

        private void DragThumb_DragStarted(object sender, DragStartedEventArgs eventArgs)
        {
            CurrentItem = DataContext as StoryboardItem;

            if(CurrentItem != null) {
                CurrentCanvas = VisualTreeHelper.GetParent(CurrentItem) as StoryboardCanvas;
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

                foreach(StoryboardItem item in CurrentCanvas.SelectedItems) {
                    Point itemDragDelta = dragDelta;

                    RotateTransform itemTransform = item.RenderTransform as RotateTransform;

                    if(itemTransform != null) {
                        itemTransform.Transform(dragDelta);
                    }

                    Canvas.SetLeft(item, Canvas.GetLeft(item) + itemDragDelta.X);
                    Canvas.SetTop(item, Canvas.GetTop(item) + itemDragDelta.Y);
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
