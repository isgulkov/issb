using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace issb
{
    public class BackgroundManager
    {
        BackgroundTemplate CurrentTempalte;
        List<Image> FrameBackgrounds;

        public BackgroundManager(BackgroundTemplate backgroundTempalte)
        {
            CurrentTempalte = backgroundTempalte; 
        }

        public void InitializeCanvas(StoryboardCanvas canvas)
        {
            canvas.Background = this;

            canvas.Width = CurrentTempalte.CanvasWidth;
            canvas.Height = CurrentTempalte.CanvasHeight;

            FrameBackgrounds = Enumerable.Repeat<Image>(null, CurrentTempalte.NumFrames).ToList();

            DrawingVisual drawingVisual = new DrawingVisual();
            using(DrawingContext drawingContext = drawingVisual.RenderOpen()) {
                for(int i = 0; i < CurrentTempalte.NumFrames; i++) {
                    Rectangle newRectangle = new Rectangle();

                    newRectangle.Stroke = Brushes.Black;
                    newRectangle.StrokeThickness = 1;

                    Rect frameRect = CurrentTempalte.FrameRects.ElementAt(i);

                    Image newImage = new Image();

                    foreach(FrameworkElement element in new FrameworkElement[] { newRectangle, newImage }) {
                        element.Width = frameRect.Width;
                        element.Height = frameRect.Height;

                        Canvas.SetLeft(element, frameRect.X);
                        Canvas.SetTop(element, frameRect.Y);

                        Panel.SetZIndex(element, -1000);

                        element.IsHitTestVisible = false;

                        canvas.Children.Add(element);
                    }

                    newImage.Stretch = Stretch.UniformToFill;

                    FrameBackgrounds[i] = newImage;
                }
            }
        }

        public void AddImageToFrame(int frameIndex, ImageSource bitmapImage)
        {
            if(frameIndex < 0 || frameIndex >= CurrentTempalte.NumFrames) {
                throw new ArgumentOutOfRangeException($"Frame index {frameIndex} out of range ({CurrentTempalte.NumFrames} frames only)");
            }

            FrameBackgrounds[frameIndex].Source = bitmapImage;
        }

        public void AddImageAt(Point point, ImageSource bitmapImage)
        {
            for(int i = 0; i < CurrentTempalte.NumFrames; i++) {
                if(CurrentTempalte.FrameRects.ElementAt(i).Contains(point)) {
                    AddImageToFrame(i, bitmapImage);
                }
            }
        }
    }
}
