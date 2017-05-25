using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml;

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
        /// Считывает шаблон из переданного файла в XML-формате (см. ПЗ)
        /// </summary>
        /// <param name="fileStream">Поток, содержащий доступный для чтения файл в XML-формате (см. ПЗ)</param>
        /// <returns>Вновь считанный из файла шаблон</returns>
        public static BackgroundTemplate ReadFromXML(FileStream fileStream)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileStream);

            return ReadFromXMLDocument(xmlDoc);
        }

        /// <summary>
        /// Считывает шаблон из переданной строки в XML-формате (см. ПЗ)
        /// </summary>
        /// <param name="xmlString">Строка, содержащая представление шабона в XML-формате (см. ПЗ)</param>
        /// <returns>Вновь считанный из строки шаблон</returns>
        public static BackgroundTemplate ReadFromXML(string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            return ReadFromXMLDocument(xmlDoc);
        }

        static BackgroundTemplate ReadFromXMLDocument(XmlDocument xmlDoc)
        {
            BackgroundTemplate newTemplate = new BackgroundTemplate();

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

        /// <summary>
        /// Записывает данный шаблон в переданный файл в XML-формате (см. ПЗ)
        /// </summary>
        /// <param name="fileStream">Поток, в который предполагается записать шаблон</param>
        /// <param name="includeHeader">Включить ли в вывод XML-заголовок (полезно, если нужно записать шаблон посреди файла)</param>
        public void WriteAsXml(StreamWriter streamWriter, bool includeHeader = true)
        {
            if(includeHeader) {
                streamWriter.WriteLine("<?xml version='1.0'?>");
            }

            streamWriter.WriteLine($"<BackgroundTemplate CanvasWidth=\"{CanvasWidth}\" CanvasHeight=\"{CanvasHeight}\">");

            foreach(Rect frameRect in FrameRects) {
                streamWriter.WriteLine($"\t<Frame X=\"{frameRect.X}\" Y=\"{frameRect.Y}\" Width=\"{frameRect.Width}\" Height=\"{frameRect.Height}\" />");
            }

            streamWriter.WriteLine("</BackgroundTemplate>");
        }
    }
}
