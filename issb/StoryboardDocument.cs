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

        List<Rect> StoryboardItemRects;
        List<ImageSource> StoryboardItemImageSources;

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

            List<Rect> storyboardItemRects = new List<Rect>();
            List<ImageSource> storyboardItemImageSources = new List<ImageSource>();

            foreach(UIElement element in storyboardCanvas.Children) {
                StoryboardItem item = element as StoryboardItem;

                if(item != null) {
                    Rect newRect = new Rect();

                    newRect.X = Canvas.GetLeft(item);
                    newRect.Y = Canvas.GetTop(item);
                    newRect.Width = item.ActualWidth;
                    newRect.Height = item.ActualHeight;

                    storyboardItemRects.Add(newRect);
                    
                    Image itemImage = (Image)item.Content;

                    storyboardItemImageSources.Add(itemImage.Source);


                }
            }

            newDocument.StoryboardItemRects = storyboardItemRects;
            newDocument.StoryboardItemImageSources = storyboardItemImageSources;

            return newDocument;
        }

        public void UnloadOntoCanvas(StoryboardCanvas storyboardCanvas)
        {

        }

        public static StoryboardDocument LoadFromXML(FileStream fileStream)
        {
            StoryboardDocument newDocument = new StoryboardDocument();

            //

            return newDocument;
        }

        public void SaveToXML(FileStream fileStream)
        {

        }
    }
}
