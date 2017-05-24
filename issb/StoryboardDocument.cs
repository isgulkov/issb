using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace issb
{
    class StoryboardDocument
    {
        BackgroundTemplate Template;

        List<ImageSource> FrameBackgrounds;

        class ItemTuple : Tuple<Rect, RotateTransform, ImageSource>
        {
            public ItemTuple(Rect rect, RotateTransform rotateTransform, ImageSource imageSource) : base(rect, rotateTransform, imageSource) { }
        }

        List<ItemTuple> StoryboardItems;

        StoryboardDocument() { }

        public static StoryboardDocument LoadFromCanvas(StoryboardCanvas storyboardCanvas)
        {
            StoryboardDocument newDocument = new StoryboardDocument();

            newDocument.Template = storyboardCanvas.BackgroundManager.CurrentTempalte;

            List<ImageSource> frameBackgrounds = new List<ImageSource>();

            for(int i = 0; i < storyboardCanvas.BackgroundManager.CurrentTempalte.NumFrames; i++) {
                frameBackgrounds.Add(storyboardCanvas.BackgroundManager.GetImageOfFrame(i));
            }

            newDocument.FrameBackgrounds = frameBackgrounds;

            List<ItemTuple> storyboardItems = new List<ItemTuple>();

            UIElement[] storyboardCanvasChildren = new UIElement[storyboardCanvas.Children.Count];

            storyboardCanvas.Children.CopyTo(storyboardCanvasChildren, 0);

            foreach(UIElement element in storyboardCanvasChildren.OrderBy(x => Panel.GetZIndex(x))) {
                StoryboardItem item = element as StoryboardItem;

                if(item != null) {
                    RotateTransform itemTransform = item.RenderTransform as RotateTransform;
                
                    item.RenderTransform = null;

                    Rect itemRect = new Rect();

                    itemRect.X = Canvas.GetLeft(item);
                    itemRect.Y = Canvas.GetTop(item);
                    itemRect.Width = item.ActualWidth;
                    itemRect.Height = item.ActualHeight;

                    item.RenderTransform = itemTransform;

                    Image itemImageSource = (Image)item.Content;

                    ItemTuple newTuple = new ItemTuple(itemRect, itemTransform, itemImageSource.Source);

                    storyboardItems.Add(newTuple);
                }
            }

            newDocument.StoryboardItems = storyboardItems;

            return newDocument;
        }

        public void UnloadOntoCanvas(StoryboardCanvas storyboardCanvas)
        {
            storyboardCanvas.Children.Clear();

            BackgroundManager backgoundManager = new BackgroundManager(Template);

            backgoundManager.InitializeCanvas(storyboardCanvas);

            for(int i = 0; i < Template.NumFrames; i++) {
                if(FrameBackgrounds[i] != null) {
                    backgoundManager.AddImageToFrame(i, FrameBackgrounds[i]);
                }
            }

            foreach(ItemTuple itemTuple in StoryboardItems) {
                Image newImage = new Image();

                newImage.Source = itemTuple.Item3;
                newImage.IsHitTestVisible = false;

                StoryboardItem newItem = new StoryboardItem();

                newItem.Content = newImage;

                Rect itemRect = itemTuple.Item1;

                newItem.Width = itemRect.Width;
                newItem.Height = itemRect.Height;

                Canvas.SetLeft(newItem, itemRect.X);
                Canvas.SetTop(newItem, itemRect.Y);

                newItem.RenderTransform = itemTuple.Item2;

                storyboardCanvas.Children.Add(newItem);
            }
        }

        static BitmapImage Base64StringToImageSource(string imageString)
        {
            byte[] bytes = Convert.FromBase64String(imageString);

            using(MemoryStream stream = new MemoryStream(bytes)) {
                BitmapImage bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(stream.ToArray());
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static StoryboardDocument LoadFromXML(FileStream fileStream)
        {
            StoryboardDocument newDocument = new StoryboardDocument();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileStream);

            XmlNode templateNode = xmlDoc.GetElementsByTagName("BackgroundTemplate")[0];

            newDocument.Template = BackgroundTemplate.ReadFromXML(templateNode.OuterXml);

            List<ImageSource> frameBackgrounds = new List<ImageSource>();

            foreach(XmlNode backgroundNode in xmlDoc.GetElementsByTagName("StoryboardBackground")) {
                XmlAttribute contentAttr = backgroundNode.Attributes["Content"];

                if(contentAttr == null) {
                    frameBackgrounds.Add(null);
                }
                else {
                    frameBackgrounds.Add(Base64StringToImageSource(contentAttr.Value));
                }
            }

            newDocument.FrameBackgrounds = frameBackgrounds;

            List<ItemTuple> storyboardItems = new List<ItemTuple>();

            foreach(XmlNode itemNode in xmlDoc.GetElementsByTagName("StoryboardItem")) {
                Rect itemRect = new Rect();

                itemRect.X = double.Parse(itemNode.Attributes["X"].Value);
                itemRect.Y = double.Parse(itemNode.Attributes["Y"].Value);
                itemRect.Width = double.Parse(itemNode.Attributes["Width"].Value);
                itemRect.Height = double.Parse(itemNode.Attributes["Height"].Value);

                BitmapImage itemImageSource;

                XmlAttribute contentAttr = itemNode.Attributes["Content"];

                if(contentAttr == null) {
                    itemImageSource = null;
                }
                else {
                    itemImageSource = Base64StringToImageSource(contentAttr.Value);
                }

                RotateTransform itemTransform = new RotateTransform();

                XmlAttribute angleAttr = itemNode.Attributes["Angle"];

                if(angleAttr != null) {
                    itemTransform.Angle = double.Parse(angleAttr.Value);
                }

                storyboardItems.Add(new ItemTuple(itemRect, itemTransform, itemImageSource));
            }

            newDocument.StoryboardItems = storyboardItems;

            return newDocument;
        }

        static string ImageSourceToBase64String(ImageSource imageSource)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();

            // TODO: research if anything can be done about the types
            BitmapFrame frame = BitmapFrame.Create(imageSource as BitmapSource);

            encoder.Frames.Add(frame);

            using(MemoryStream memoryStream = new MemoryStream()) {
                encoder.Save(memoryStream);

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public void SaveToXML(FileStream fileStream)
        {
            using(StreamWriter streamWriter = new StreamWriter(fileStream)) {
                streamWriter.WriteLine("<?xml version='1.0'?>");

                streamWriter.WriteLine("<StoryboardDocument>");

                Template.WriteAsXml(streamWriter, false);

                streamWriter.WriteLine("<StoryboardBackgrounds>");

                foreach(ImageSource frameBackground in FrameBackgrounds) {
                    if(frameBackground == null) {
                        streamWriter.WriteLine("<StoryboardBackground />");
                    }
                    else {
                        streamWriter.Write("<StoryboardBackground Content=\"");

                        streamWriter.Write(ImageSourceToBase64String(frameBackground));

                        streamWriter.WriteLine("\" />");
                    }
                }

                streamWriter.WriteLine("</StoryboardBackgrounds>");

                streamWriter.WriteLine("<StoryboardItems>");

                foreach(ItemTuple storyboardItem in StoryboardItems) {
                    Rect itemRect = storyboardItem.Item1;

                    streamWriter.Write($"<StoryboardItem X=\"{itemRect.X}\" Y=\"{itemRect.Y}\" Width=\"{itemRect.Width}\" Height=\"{itemRect.Height}\"");

                    RotateTransform itemTransform = storyboardItem.Item2;

                    if(itemTransform != null) {
                        streamWriter.Write($" Angle=\"{itemTransform.Angle}\"");
                    }

                    ImageSource itemImage = storyboardItem.Item3;

                    if(itemImage != null) {
                        streamWriter.Write(" Content=\"");

                        streamWriter.Write(ImageSourceToBase64String(itemImage));

                        streamWriter.Write("\"");
                    }

                    streamWriter.WriteLine("/>");
                }

                streamWriter.WriteLine("</StoryboardItems>");

                streamWriter.WriteLine("</StoryboardDocument>");
            }
        }
    }
}
