using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Windows.Media.Imaging;

namespace issb
{
    /// <summary>
    /// Представляет собой библиотеку предустановленных элементов, фонов и шаблонов фона раскадровки, загружаемую с помощью конфигурационного файла в XML-формате (см. ПЗ)
    /// </summary>
    public class PresetLibrary
    {
        public IReadOnlyCollection<BitmapImage> Items { get; private set; }
        public IReadOnlyCollection<BitmapImage> Backgrounds { get; private set; }
        public IReadOnlyCollection<BackgroundTemplate> Tempates { get; private set; }

        PresetLibrary() { }

        /// <summary>
        /// Загружает предустановленные элементы, фоны и шаблоны фона раскадровки из файлов, описанных в переданном конфигурационном файле в XML-формате (см. ПЗ)
        /// </summary>
        /// <param name="fileStream">Поток, содержащий доступный для чтения файл в XML-формате (см. ПЗ)</param>
        /// <param name="xmlFilePath">Путь к XML-файлу, по которому открыт поток из параметра fileStream (для поддержки относительных путей)</param>
        /// <returns>Библиотека, содержащая загруженные предустановленные элементы, фоны и шаблоны фона раскадровки</returns>
        public static PresetLibrary LoadFromXML(FileStream fileStream, string xmlFilePath)
        {
            string xmlFolderPath = Path.GetDirectoryName(xmlFilePath);

            PresetLibrary newLibrary = new PresetLibrary();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileStream);

            XmlNodeList itemNodes = xmlDoc.GetElementsByTagName("Item");

            newLibrary.Items = BitMapImagesFromNodes(itemNodes, xmlFolderPath);

            XmlNodeList backgroundNodes = xmlDoc.GetElementsByTagName("Background");

            newLibrary.Backgrounds = BitMapImagesFromNodes(backgroundNodes, xmlFolderPath);

            XmlNodeList templateNodes = xmlDoc.GetElementsByTagName("Template");

            List<BackgroundTemplate> templates = new List<BackgroundTemplate>();

            foreach(XmlNode templateNode in templateNodes) {
                string filePath = Path.Combine(xmlFolderPath, templateNode.Attributes["Src"].Value);

                if(filePath != null) {
                    try {
                        BackgroundTemplate newTemplate;

                        using(FileStream templateFileStream = new FileStream(filePath, FileMode.Open)) {
                            newTemplate = BackgroundTemplate.ReadFromXML(templateFileStream);
                        }

                        templates.Add(newTemplate);
                    }
                    catch(Exception) { }
                }
            }

            newLibrary.Tempates = templates;

            return newLibrary;
        }

        /// <summary>
        /// Возвращает список объектов BitmapImage, загруженных из файлов с путями, находящихся в аттрибутах Src соответствующих XML-элементов
        /// 
        /// Данный метод создан с целью того, чтобы устранить провторяющийся код при загрузке эементов и фонов в методе LoadFromXML в сответствие с принципом DRY
        /// </summary>
        /// <param name="nodeList">Коллекция XML->лементов, из аттрибутов Src которых возьмутся пути, из которых загрузятся объекты BitmapImage</param>
        /// <param name="xmlFolderPath">Путь к директории, в которой лежит обратаываемый XML-файл (для поддержки относительных путей)</param>
        /// <returns>Список объектов BitmapImage, загруженных из аттритубов Src соответствующих XML-элементов переданной коллекции</returns>
        static List<BitmapImage> BitMapImagesFromNodes(XmlNodeList nodeList, string xmlFolderPath)
        {
            List<BitmapImage> bitmapImages = new List<BitmapImage>();

            foreach(XmlNode itemNode in nodeList) {
                try {
                    string filePath = Path.Combine(xmlFolderPath, itemNode.Attributes["Src"].Value);

                    if(filePath != null) {
                        BitmapImage newBitmapImage = new BitmapImage(new Uri(filePath, UriKind.RelativeOrAbsolute));

                        bitmapImages.Add(newBitmapImage);
                    }
                }
                catch(FileNotFoundException) { }
                catch(UriFormatException) { }
            }

            return bitmapImages;
        }
    }
}
