using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace issb
{
    class StoryboardDocument
    {
        BackgroundTemplate Template;

        List<BitmapImage> FrameBackgrounds;

        List<Rect> StoryboardItemRects;
        List<BitmapImage> StoryboardItemBitmapImages;

        StoryboardDocument() { }

        public static StoryboardDocument LoadFromCanvas(StoryboardCanvas storyboardCanvas)
        {
            StoryboardDocument newDocument = new StoryboardDocument();

            //

            return newDocument;
        }

        public void UnloadOntoCanvas(StoryboardCanvas storyboardCanvas)
        {

        }

        public static StoryboardDocument LoadFromFile(FileStream fileStream)
        {
            StoryboardDocument newDocument = new StoryboardDocument();

            //

            return newDocument;
        }

        public void SaveToFile(FileStream fileStream)
        {

        }
    }
}
