using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace issb {
    public class DragThumb : Thumb {
        private void DraggableThumb_DragDelta(object sender, DragDeltaEventArgs eventArgs)
        {
            Control item = DataContext as Control;

            if(item != null) {
                double leftOffset = Canvas.GetLeft(item);
                double topOffset = Canvas.GetTop(item);

                Canvas.SetLeft(item, leftOffset + eventArgs.HorizontalChange);
                Canvas.SetTop(item, topOffset + eventArgs.VerticalChange);
            }
        }

        public DragThumb()
        {
            DragDelta += new DragDeltaEventHandler(DraggableThumb_DragDelta);
        }
    }
}
