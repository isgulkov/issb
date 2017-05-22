using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace issb
{
    public class RotateThumb : Thumb
    {
        private Canvas StoryboardCanvas;
        private ContentControl StoryboardItem;

        private Point CenterPoint;
        private Vector StartVector;
        private double InitialAngle;
        private RotateTransform RotateTransform;

        public RotateThumb()
        {
            DragDelta += new DragDeltaEventHandler(RotateThumb_DragDelta);
            DragStarted += new DragStartedEventHandler(RotateThumb_DragStarted);
        }

        private void RotateThumb_DragStarted(object sender, DragStartedEventArgs eventArgs)
        {
            StoryboardItem = DataContext as ContentControl;

            if(StoryboardItem != null) {
                StoryboardCanvas = VisualTreeHelper.GetParent(StoryboardItem) as Canvas;

                if(StoryboardCanvas != null) {
                    CenterPoint = StoryboardItem.TranslatePoint(
                        new Point(StoryboardItem.Width * StoryboardItem.RenderTransformOrigin.X,
                                  StoryboardItem.Height * StoryboardItem.RenderTransformOrigin.Y),
                                  StoryboardCanvas);

                    Point startPoint = Mouse.GetPosition(StoryboardCanvas);
                    StartVector = Point.Subtract(startPoint, CenterPoint);

                    RotateTransform = StoryboardItem.RenderTransform as RotateTransform;
                    if(RotateTransform == null) {
                        StoryboardItem.RenderTransform = new RotateTransform(0);
                        InitialAngle = 0.0;
                    }
                    else {
                        InitialAngle = RotateTransform.Angle;
                    }
                }
            }
        }

        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs eventArgs)
        {
            if(StoryboardItem != null && StoryboardCanvas != null) {
                Point currentPoint = Mouse.GetPosition(StoryboardCanvas);
                Vector deltaVector = Point.Subtract(currentPoint, CenterPoint);

                double angle = Vector.AngleBetween(StartVector, deltaVector);

                RotateTransform rotateTransform = StoryboardItem.RenderTransform as RotateTransform;
                rotateTransform.Angle = InitialAngle + Math.Round(angle, 0);
                StoryboardItem.InvalidateMeasure();
            }
        }
    }
}
