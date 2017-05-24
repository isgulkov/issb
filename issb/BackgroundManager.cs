﻿using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace issb
{
    /// <summary>
    /// Управляет отрисовкой фона раскадровки на холсте
    /// </summary>
    public class BackgroundManager
    {
        public BackgroundTemplate CurrentTempalte { get; private set; }
        List<Image> FrameBackgrounds;

        public BackgroundManager(BackgroundTemplate backgroundTempalte)
        {
            CurrentTempalte = backgroundTempalte; 
        }

        /// <summary>
        /// Инициализирует фон на выбранном холсте (отрисоывает прямоугольники, означающие границы кадров и создает соответствующие объекты типа Image)
        /// </summary>
        /// <param name="canvas">Холст, на котором предполагается инициализовать фон</param>
        public void InitializeCanvas(StoryboardCanvas canvas)
        {
            canvas.BackgroundManager = this;

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

        /// <summary>
        /// Устанавливают данное фоновое изображение для кадра с данным индексом
        /// </summary>
        /// <param name="frameIndex">Индекс кандра, для которого утсанавливается изображение</param>
        /// <param name="imageSource">Изображение, которое устанавливается для данного кадра</param>
        public void AddImageToFrame(int frameIndex, ImageSource imageSource)
        {
            if(frameIndex < 0 || frameIndex >= CurrentTempalte.NumFrames) {
                throw new ArgumentOutOfRangeException($"Frame index {frameIndex} out of range ({CurrentTempalte.NumFrames} frames only)");
            }

            FrameBackgrounds[frameIndex].Source = imageSource;
        }

        /// <summary>
        /// Возвращает объект ImageSource, установленный в качестве фонового изображения для кадра с заданным индексом
        /// </summary>
        /// <param name="frameIndex">Индекс кадра, для которого предполагается возвратить ImageSource, установленный в качестве его фонового изображения</param>
        /// <returns>Объект ImageSource, установленный в качестве фонового изображения для кадра с заданным индексом. Если для данного кадра фоновое изображение установлено, возвращает null</returns>
        public ImageSource GetImageOfFrame(int frameIndex)
        {
            if(frameIndex < 0 || frameIndex >= CurrentTempalte.NumFrames) {
                throw new ArgumentOutOfRangeException($"Frame index {frameIndex} out of range ({CurrentTempalte.NumFrames} frames only)");
            }

            return FrameBackgrounds[frameIndex].Source;
        }

        /// <summary>
        /// Устанавливают данное фоновое изображение для кадра, в который входит данная точка холста.
        /// 
        /// Если таких кадров несколько (не рекомендуется) фон устанавливается для всех таких кадров. Если таких кадров не существует, не изменяется ничего
        /// </summary>
        /// <param name="point">Точка, в которой предположительно находится кадр, для которого предполагается установить фоновое изображение</param>
        /// <param name="imageSource">Изображение, которое предполагается установить для кадра в данной точке</param>
        public void AddImageAt(Point point, ImageSource imageSource)
        {
            for(int i = 0; i < CurrentTempalte.NumFrames; i++) {
                if(CurrentTempalte.FrameRects.ElementAt(i).Contains(point)) {
                    AddImageToFrame(i, imageSource);
                }
            }
        }
    }
}
