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
    class StoryboardBackground
    {
        int BackgroundWidth = 500;
        int BackgroundHeight = 500;

        readonly int NumFrames;

        List<Rect> FrameRects;
        List<Image> FrameBackgrounds;

        public StoryboardBackground(int numFrames /* TODO: background settings */)
        {
            NumFrames = numFrames;

            FrameRects = new List<Rect>(new Rect[] { new Rect(10, 10, 480, 235), new Rect(10, 255, 480, 235) });
        }

        public void InitializeCanvas(Canvas canvas)
        {
            canvas.Width = BackgroundWidth;
            canvas.Height = BackgroundHeight;

            FrameBackgrounds = Enumerable.Repeat<Image>(null, NumFrames).ToList();

            DrawingVisual drawingVisual = new DrawingVisual();
            using(DrawingContext drawingContext = drawingVisual.RenderOpen()) {
                for(int i = 0; i < NumFrames; i++) {
                    Rectangle newRectangle = new Rectangle();

                    newRectangle.Stroke = Brushes.Black;
                    newRectangle.StrokeThickness = 1;

                    Rect frameRect = FrameRects[i];

                    Image newImage = new Image();

                    foreach(FrameworkElement element in new FrameworkElement[] { newRectangle, newImage }) {
                        element.Width = frameRect.Width;
                        element.Height = frameRect.Height;

                        Canvas.SetLeft(element, frameRect.X);
                        Canvas.SetTop(element, frameRect.Y);

                        Panel.SetZIndex(element, -1000);

                        canvas.Children.Add(element);
                    }

                    newImage.Stretch = Stretch.UniformToFill;

                    FrameBackgrounds[i] = newImage;
                }
            }
        }

        public void AddImageToFrame(int frameIndex, BitmapImage bitmapImage)
        {
            if(frameIndex < 0 || frameIndex >= NumFrames) {
                throw new ArgumentOutOfRangeException($"Frame index {frameIndex} out of range ({NumFrames} frames only)");
            }

            FrameBackgrounds[frameIndex].Source = bitmapImage;
        }
    }
}
