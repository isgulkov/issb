using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Windows;

namespace issb
{
    /// <summary>
    /// Описывает шаблон фона для видеораскадровки. Объекты данного класса предполагаются неизменяемыми.
    /// </summary>
    public class BackgroundTemplate
    {
        public int CanvasWidth { get; protected set; }
        public int CanvasHeight { get; protected set; }

        public IReadOnlyCollection<Rect> FrameRects { get; protected set; }

        public int NumFrames { get; protected set; }

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

        public void WriteToXML(FileStream fileStream)
        {
            using(StreamWriter streamWriter = new StreamWriter(fileStream)) {
                streamWriter.WriteLine("<?xml version='1.0'?>");

                streamWriter.WriteLine($"<BackgroundTemplate CanvasWidth=\"{CanvasWidth}\" CanvasHeight=\"{CanvasHeight}\">");

                foreach(Rect frameRect in FrameRects) {
                    streamWriter.WriteLine($"\t<Frame X=\"{frameRect.X}\" Y=\"{frameRect.Y}\" Width=\"{frameRect.Width}\" Height=\"{frameRect.Height}\" />");
                }

                streamWriter.WriteLine("</BackgroundTemplate>");
            }
        }
    }
}
