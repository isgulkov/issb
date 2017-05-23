using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace issb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundManager CurrentBackgoundManager;

        List<BackgroundTemplate> CurrentTemplates = new List<BackgroundTemplate>();

        string _CurrentFilePath = null;

        string CurrentFilePath
        {
            get
            {
                return _CurrentFilePath;
            }
            set
            {
                _CurrentFilePath = value;

                if(string.IsNullOrEmpty(value)) {
                    Title = "issb";
                }
                else {
                    Title = $"{Path.GetFileName(value)} — issb";
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs eventArgs)
        {
            base.OnContentRendered(eventArgs);

            try {
                PresetLibrary presetLibrary = null;

                using(FileStream fileStream = new FileStream(@"PresetLibraries\DefaultPresets.xml", FileMode.Open)) {
                    presetLibrary = PresetLibrary.LoadFromXML(fileStream, @"PresetLibraries\DefaultPresets.xml");
                }

                LoadBitmapImagesIntoItemsToolbox(presetLibrary.Items);

                LoadBitmapImagesIntoBackgroundsToolbox(presetLibrary.Backgrounds);

                CurrentTemplates = CurrentTemplates.Concat(presetLibrary.Tempates).ToList();
            }
            catch(Exception ex) {
                MessageBox.Show(this, $"Произошла ошибка при загрузке предустановленного контента\r\n\r\n{ex.Message}");
            }
        }

        private void NewDocumentMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            CreateNewDocument();
        }

        private void CreateNewDocument()
        {
            NewDocumentDialog newDocumentDialog = new NewDocumentDialog();

            newDocumentDialog.PresetTemplates = CurrentTemplates;
            newDocumentDialog.Owner = this; // for WidnowsStartupLocation="CenterOwner"

            newDocumentDialog.ShowDialog();

            if(newDocumentDialog.SelectedTemplate != null) {
                CurrentFilePath = null;

                MainCanvas.Children.Clear();

                CurrentBackgoundManager = new BackgroundManager(newDocumentDialog.SelectedTemplate);

                CurrentBackgoundManager.InitializeCanvas(MainCanvas);
            }
        }

        IReadOnlyCollection<BitmapImage> ImportImages()
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.Multiselect = true;

            if(openDialog.ShowDialog().Value) {
                ImportImagesDialog importDialog = new ImportImagesDialog();

                importDialog.FilesToImport = openDialog.FileNames;

                importDialog.Owner = this;

                importDialog.ShowDialog();

                return importDialog.LoadedBitmaps;
            }

            return null;
        }

        void LoadBitmapImagesIntoToolbox(IReadOnlyCollection<BitmapImage> bitmapImages, Toolbox toolbox, ToolboxItem.ItemMode itemMode)
        {
            foreach(BitmapImage bitmapImage in bitmapImages) {
                ToolboxItem newToolboxItem = new ToolboxItem();

                Image newImage = new Image();

                newImage.Source = bitmapImage;
                newImage.IsHitTestVisible = false;

                newToolboxItem.Content = newImage;
                newToolboxItem.Mode = itemMode;

                toolbox.Items.Add(newToolboxItem);
            }
        }

        void LoadBitmapImagesIntoItemsToolbox(IReadOnlyCollection<BitmapImage> bitmapImages)
        {
            LoadBitmapImagesIntoToolbox(bitmapImages, ItemsToolbox, ToolboxItem.ItemMode.StoryboardItem);
        }

        void LoadBitmapImagesIntoBackgroundsToolbox(IReadOnlyCollection<BitmapImage> bitmapImages)
        {
            LoadBitmapImagesIntoToolbox(bitmapImages, BackgroundsToolbox, ToolboxItem.ItemMode.StoryboardBackground);
        }

        private void ImportItemsMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            IReadOnlyCollection<BitmapImage> bitmapImages = ImportImages();

            LoadBitmapImagesIntoItemsToolbox(bitmapImages);
        }

        private void ImportBackgroundImagesMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            IReadOnlyCollection<BitmapImage> bitmapImages = ImportImages();

            LoadBitmapImagesIntoBackgroundsToolbox(bitmapImages);
        }

        private void ExportMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Filter = "Изображение в формате PNG (*.png)|*.png";

            if(saveDialog.ShowDialog().Value) {
                try {
                    SaveCanvasToPath(saveDialog.FileName);
                }
                catch(Exception ex) {
                    MessageBox.Show(this, $"При экспортировании документа произошла ошибка\r\n\r\n{ex.Message}");
                }
            }
        }

        void SaveCanvasToPath(string filePath)
        {
            Transform transform = MainCanvas.LayoutTransform;

            MainCanvas.LayoutTransform = null;

            Size size = new Size(MainCanvas.ActualWidth, MainCanvas.ActualHeight);

            MainCanvas.Measure(size);
            MainCanvas.Arrange(new Rect(size));

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96.0, 96.0, PixelFormats.Pbgra32);

            renderBitmap.Render(MainCanvas);

            MainCanvas.LayoutTransform = transform;

            WriteableBitmap writableBitmap = new WriteableBitmap(renderBitmap);

            using(FileStream fileStream = new FileStream(filePath, FileMode.Create)) {
                PngBitmapEncoder encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(writableBitmap));

                encoder.Save(fileStream);
            }
        }

        private void OpenDocumentMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            
        }

        private void SaveDocumentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentFilePath == null) {
                SaveDocumentAs();
            }
            else {
                SaveDocument(CurrentFilePath);
            }
        }

        private void SaveDocumentAsMenuItem_Click_1(object sender, RoutedEventArgs eventArgs)
        {
            SaveDocumentAs();
        }

        void SaveDocument(string filePath)
        {
            StoryboardDocument document = StoryboardDocument.LoadFromCanvas(MainCanvas);

            try {
                using(FileStream fileStream = new FileStream(filePath, FileMode.Create)) {
                    document.SaveToXML(fileStream);
                }
            }
            catch(Exception ex) {
                MessageBox.Show(this, $"Сохранить документ не удалось\r\n\r\n{ex.Message}");
            }
        }

        void SaveDocumentAs()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Filter = "XML-формат видеораскадровки (*.sb)|*.sb";

            if(saveDialog.ShowDialog().Value) {
                CurrentFilePath = saveDialog.FileName;

                SaveDocument(saveDialog.FileName);
            }
        }
    }
}
