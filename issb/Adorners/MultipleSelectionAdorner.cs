using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace issb
{
    public class MultipleSelectionAdorner : Adorner
    {
        private Point? SelectionStartPoint, SelectionEndPoint;
        private Rectangle SelectionRect;
        private StoryboardCanvas Canvas;
        private VisualCollection Visuals;
        private Canvas AdornerCanvas;

        protected override int VisualChildrenCount
        {
            get
            {
                return Visuals.Count;
            }
        }

        public MultipleSelectionAdorner(StoryboardCanvas canvas, Point? dragStartPoint) : base(canvas)
        {
            Canvas = canvas;
            SelectionStartPoint = dragStartPoint;

            AdornerCanvas = new Canvas();
            AdornerCanvas.Background = Brushes.Transparent;
            Visuals = new VisualCollection(this);
            Visuals.Add(AdornerCanvas);

            SelectionRect = new Rectangle();
            SelectionRect.Stroke = Brushes.DarkGreen;
            SelectionRect.StrokeThickness = 1;
            SelectionRect.StrokeDashArray = new DoubleCollection(new double[] { 4 });

            AdornerCanvas.Children.Add(SelectionRect);
        }

        protected override void OnMouseMove(MouseEventArgs eventArgs)
        {
            if(eventArgs.LeftButton == MouseButtonState.Pressed) {
                if(!IsMouseCaptured) {
                    CaptureMouse();
                }

                SelectionEndPoint = eventArgs.GetPosition(this);
                UpdateSelectionRect();
                UpdateSelection();
                eventArgs.Handled = true;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs eventArgs)
        {
            if(IsMouseCaptured) {
                ReleaseMouseCapture();
            }

            AdornerLayer adornerLayer = Parent as AdornerLayer;
            if(adornerLayer != null) {
                adornerLayer.Remove(this);
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            AdornerCanvas.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return Visuals[index];
        }

        private void UpdateSelectionRect()
        {
            double left = Math.Min(SelectionStartPoint.Value.X, SelectionEndPoint.Value.X);
            double top = Math.Min(SelectionStartPoint.Value.Y, SelectionEndPoint.Value.Y);

            double width = Math.Abs(SelectionStartPoint.Value.X - SelectionEndPoint.Value.X);
            double height = Math.Abs(SelectionStartPoint.Value.Y - SelectionEndPoint.Value.Y);

            SelectionRect.Width = width;
            SelectionRect.Height = height;

            System.Windows.Controls.Canvas.SetLeft(SelectionRect, left);
            System.Windows.Controls.Canvas.SetTop(SelectionRect, top);
        }

        private void UpdateSelection()
        {
            Rect selectionRect = new Rect(SelectionStartPoint.Value, SelectionEndPoint.Value);

            foreach(UIElement element in Canvas.Children) {
                if(!(element is StoryboardItem)) {
                    continue;
                }

                StoryboardItem storyboardItem = (StoryboardItem)element;

                Rect itemRect = VisualTreeHelper.GetDescendantBounds(storyboardItem);
                Rect itemBounds = storyboardItem.TransformToAncestor(Canvas).TransformBounds(itemRect);

                if(selectionRect.Contains(itemBounds)) {
                    storyboardItem.IsSelected = true;
                }
                else {
                    storyboardItem.IsSelected = false;
                }
            }
        }
    }
}
