﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Windows;

namespace issb
{
    /// <summary>
    /// Описывает шаблон фона видеораскадровки. Объекты данного класса предполагаются неизменяемыми
    /// </summary>
    public class BackgroundTemplate
    {
        /// <summary>
        /// Ширина холста
        /// </summary>
        public int CanvasWidth { get; protected set; }

        /// <summary>
        /// Высота холста
        /// </summary>
        public int CanvasHeight { get; protected set; }

        /// <summary>
        /// Прямоугольник, каждый из которых представляет собой границу соответствующего кадра
        /// </summary>
        public IReadOnlyCollection<Rect> FrameRects { get; protected set; }

        /// <summary>
        /// Количество кадров в шаблоне
        /// </summary>
        public int NumFrames { get; protected set; }

        /// <summary>
        /// Считывает данный шаблон из переданного файла в XML-формате (см. ПЗ)
        /// </summary>
        /// <param name="fileStream">Поток, содержащий доступный для чтения файл в XML-формате (см. ПЗ)</param>
        /// <returns>Вновь считанный из файла объект-шаблон</returns>
        public static BackgroundTemplate ReadFromXML(FileStream fileStream)
        {
            BackgroundTemplate newTemplate = new BackgroundTemplate();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileStream);

            XmlNode rootNode = xmlDoc.GetElementsByTagName("BackgroundTemplate")[0];

            newTemplate.CanvasWidth = int.Parse(rootNode.Attributes["CanvasWidth"].Value);
            newTemplate.CanvasHeight = int.Parse(rootNode.Attributes["CanvasHeight"].Value);

            List<Rect> frameRects = new List<Rect>();

            XmlNodeList frameNodes = xmlDoc.GetElementsByTagName("Frame");

            foreach(XmlNode frameNode in frameNodes) {
                Rect newRect = new Rect();

                newRect.X = int.Parse(frameNode.Attributes["X"].Value);
                newRect.Y = int.Parse(frameNode.Attributes["Y"].Value);
                newRect.Width = int.Parse(frameNode.Attributes["Width"].Value);
                newRect.Height = int.Parse(frameNode.Attributes["Height"].Value);

                frameRects.Add(newRect);
            }

            newTemplate.FrameRects = frameRects;
            newTemplate.NumFrames = frameRects.Count;

            return newTemplate;
        }
    }
}