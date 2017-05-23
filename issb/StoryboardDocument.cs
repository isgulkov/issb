using System;
using System.Collections.Generic;
using System.IO;
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

        List<Tuple<Rect, ImageSource>> StoryboardItems;

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

            List<Tuple<Rect, ImageSource>> storyboardItems = new List<Tuple<Rect, ImageSource>>();

            foreach(UIElement element in storyboardCanvas.Children) {
                StoryboardItem item = element as StoryboardItem;

                if(item != null) {

                    Rect newRect = new Rect();

                    newRect.X = Canvas.GetLeft(item);
                    newRect.Y = Canvas.GetTop(item);
                    newRect.Width = item.ActualWidth;
                    newRect.Height = item.ActualHeight;

                    Image itemImage = (Image)item.Content;

                    Tuple<Rect, ImageSource> newTuple = new Tuple<Rect, ImageSource>(newRect, itemImage.Source);

                    storyboardItems.Add(newTuple);

                }
            }

            newDocument.StoryboardItems = storyboardItems;

            return newDocument;
        }

        public void UnloadOntoCanvas(StoryboardCanvas storyboardCanvas)
        {

        }

        ImageSource Base64StringToImageSource(string imageString)
        {
            byte[] bytes = Convert.FromBase64String(imageString);

            using(MemoryStream stream = new MemoryStream(bytes)) {
                return BitmapFrame.Create(stream);
            }
        }

        public static StoryboardDocument LoadFromXML(FileStream fileStream)
        {
            StoryboardDocument newDocument = new StoryboardDocument();

            //

            return newDocument;
        }

        string ImageSourceToBase64String(ImageSource imageSource)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            // TODO: research if something can be done
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

                streamWriter.WriteLine("<StoryboarBackgrounds>");

                foreach(ImageSource frameBackground in FrameBackgrounds) {
                    if(frameBackground == null) {
                        streamWriter.WriteLine("<StoryboarBackground />");
                    }
                    else {
                        streamWriter.Write("<StoryboarBackground Content=\"");

                        streamWriter.Write(ImageSourceToBase64String(frameBackground));

                        streamWriter.WriteLine("\" />");
                    }
                }

                streamWriter.WriteLine("</StoryboarBackgrounds>");

                streamWriter.WriteLine("<StoryboardItems>");

                foreach(Tuple<Rect, ImageSource> storyboardItem in StoryboardItems) {
                    Rect itemRect = storyboardItem.Item1;

                    streamWriter.Write($"<StoryboardItem X=\"{itemRect.X}\" Y=\"{itemRect.Y}\" Width=\"{itemRect.Width}\" Height=\"{itemRect.Height}\" Content=\"");

                    streamWriter.Write(ImageSourceToBase64String(storyboardItem.Item2));

                    streamWriter.WriteLine("\" />");
                }

                streamWriter.WriteLine("</StoryboardItems>");

                streamWriter.WriteLine("</StoryboardDocument>");
            }
        }
    }
}
