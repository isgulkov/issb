using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace issb
{
    /// <summary>
    /// Описывает поведение элемента-окна, являющегося главным окном программы
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Название программы (для отображения в заголовке главного окна)
        /// </summary>
        readonly string ProgramName = "issb";

        /// <summary>
        /// Набор шаблонов фона, предназначенный для отображения в диалоговом окне создания нового документа
        /// </summary>
        List<BackgroundTemplate> CurrentTemplates = new List<BackgroundTemplate>();
        
        /// <summary>
        /// Флаг режима "без документа", при котором в программе не открыт и не создан документ-видеораскадровка, и в связи с этим запрещена операция сохранения документа
        /// </summary>
        bool NoDocumentMode = true;

        /// <summary>
        /// Отключает режим "без документа", активируя при этом пункты меню, соответствующие операции сохранения документа
        /// </summary>
        void DisableNoDocumentMode()
        {
            NoDocumentMode = false;

            SaveDocumentMenuItem.IsEnabled = true;
            SaveDocumentAsMenuItem.IsEnabled = true;
        }

        /// <summary>
        /// Путь к открытому на данный момент файлу. Переменной присваивается null в том случае, если документ создан, но не сохранен в файле
        /// </summary>
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
                    Title = $"Новая видеораскадровка — {ProgramName}";
                }
                else {
                    Title = $"{Path.GetFileName(value)} — {ProgramName}";
                }
            }
        }
        
        string _CurrentFilePath = null;

        public MainWindow()
        {
            InitializeComponent();

            Title = ProgramName;
        }

        /// <summary>
        /// При загрузке окна загружает библиотеку предварительно загруженных элементов, изображений-фонов и шаблонов фона из библиотеки фонов, по умолчанию расположенной в папке программы
        /// </summary>
        /// <param name="eventArgs"></param>
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

        /// <summary>
        /// При нажатии кнопок клавиатуры Delete и Backspace инициирует операцию удаления выделенных элементов раскадровки с рабочего холста
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnKeyDown(KeyEventArgs eventArgs)
        {
            base.OnKeyDown(eventArgs);
            
            if(eventArgs.Key == Key.Back || eventArgs.Key == Key.Delete) {
                MainCanvas.DeleteSelectedItems();

                eventArgs.Handled = true;
            }
        }

        /// <summary>
        /// Обратаывает нажатие пункта меню, инициируя операцию создания нового документа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void NewDocumentMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            CreateNewDocument();
        }

        /// <summary>
        /// Отображает диалоговое окно "Новый документ" и, в случае нажатия пользователем кнопки "OK" в нем, производит создание нового документа
        /// </summary>
        private void CreateNewDocument()
        {
            NewDocumentDialog newDocumentDialog = new NewDocumentDialog();

            newDocumentDialog.PresetTemplates = CurrentTemplates;
            newDocumentDialog.Owner = this; // for WidnowsStartupLocation="CenterOwner"

            newDocumentDialog.ShowDialog();

            if(newDocumentDialog.SelectedTemplate != null) {
                CurrentFilePath = null;

                MainCanvas.Children.Clear();

                BackgroundManager backgoundManager = new BackgroundManager(newDocumentDialog.SelectedTemplate);

                backgoundManager.InitializeCanvas(MainCanvas);

                if(NoDocumentMode) {
                    DisableNoDocumentMode();
                }
            }
        }

        /// <summary>
        /// Отображает диалоговое окно открытия файла и, в случае выбора пользователем файлов в нем, возвращает коллекцию объектов <see cref="BitmapImage"/>, соответствующих выбранным пользователем файлов-изображений
        /// </summary>
        /// <returns>Коллекция изображений, импортированных с диска, в виде объектов <see cref="BitmapImage"/></returns>
        IReadOnlyCollection<BitmapImage> ImportImages()
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.Multiselect = true;

            if(openDialog.ShowDialog().Value) {
                try {
                    ImportImagesDialog importDialog = new ImportImagesDialog();

                    importDialog.FilesToImport = openDialog.FileNames;

                    importDialog.Owner = this;

                    importDialog.ShowDialog();

                    return importDialog.LoadedBitmaps;
                }
                catch(Exception ex) {
                    MessageBox.Show(this, $"При импортировании изображений произошла ошибка\r\n\r\n{ex.Message}");
                }
            }

            return null;
        }

        /// <summary>
        /// Загружает элементы переданной коллекции изображений, представленные в виде объектов <see cref="BitmapImage"/>, в виде элементов панели инструментов главного окна
        /// </summary>
        /// <param name="bitmapImages">Коллекция изображений, которые предполагается загрузить в виде элементов панели инструментов</param>
        /// <param name="toolbox">Панель инструментов, в которую предполагается загрузить переданные изображения</param>
        /// <param name="itemMode">Вид элементов панели управления, которые предполагается загрузить в выбранную панель -- элементы раскадровки или изображения-фоны</param>
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

        /// <summary>
        /// Загружает элементы переданной коллекции изображений, в виде элементов панели инструментов, предназначенной для элементов раскадровки 
        /// </summary>
        /// <param name="bitmapImages">Коллекция изображений, которые предполагается загрузить в виде элементов панели инструментов, предназначенной для элементов раскадровки</param>
        void LoadBitmapImagesIntoItemsToolbox(IReadOnlyCollection<BitmapImage> bitmapImages)
        {
            LoadBitmapImagesIntoToolbox(bitmapImages, ItemsToolbox, ToolboxItem.ItemMode.StoryboardItem);
        }

        /// <summary>
        /// Загружает элементы переданной коллекции изображений, в виде элементов панели инструментов, предназначенной для изображений-фонов 
        /// </summary>
        /// <param name="bitmapImages">Коллекция изображений, которые предполагается загрузить в виде элементов панели инструментов, предназначенной для изображений-фонов</param>
        void LoadBitmapImagesIntoBackgroundsToolbox(IReadOnlyCollection<BitmapImage> bitmapImages)
        {
            LoadBitmapImagesIntoToolbox(bitmapImages, BackgroundsToolbox, ToolboxItem.ItemMode.StoryboardBackground);
        }

        /// <summary>
        /// обратаывает нажатие на пункт меню, соответствующий операции импортирования элементов раскадровки, инициируя соответствующую операцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ImportItemsMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            IReadOnlyCollection<BitmapImage> bitmapImages = ImportImages();

            if(bitmapImages != null) {
                LoadBitmapImagesIntoItemsToolbox(bitmapImages);
            }
        }

        /// <summary>
        /// обратаывает нажатие на пункт меню, соответствующий операции импортирования изображений-фонов, инициируя соответствующую операцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ImportBackgroundImagesMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            IReadOnlyCollection<BitmapImage> bitmapImages = ImportImages();

            if(bitmapImages != null) {
                LoadBitmapImagesIntoBackgroundsToolbox(bitmapImages);
            }
        }

        /// <summary>
        /// обратаывает нажатие на пункт меню, соответствующий операции эспортирования видеораскадровки в виде изображения в формате PNG, инициируя соответствующую операцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
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

        /// <summary>
        /// Сохраняет текущее содержимое рабочего холста в файле, находящемуся или вновь создаваемому по переданному пути, в виде изображения в формате PNG
        /// </summary>
        /// <param name="filePath">Путь, по которому предполагается сохранить текущее содержимое рабочего холста</param>
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

        /// <summary>
        /// обратаывает нажатие на пункт меню, соответствующий операции открытия файла видеораскадровки, инициируя соответствующую операцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OpenDocumentMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.Filter = "XML-формат видеораскадровки (*.sb)|*.sb";

            if(openDialog.ShowDialog().Value) {
                try {
                    OpenDocument(openDialog.FileName);
                }
                catch(Exception ex) {
                    MessageBox.Show(this, $"При попытке открытия файла произошла ошибка\r\n\r\n{ex.Message}");
                }
            }
        }

        /// <summary>
        /// Открывает документ-видеораскадровку, сохраненный в файле по заданному пути, и выгружает его на рабочий холст главного окна
        /// </summary>
        /// <param name="filePath">Путь к файлу, содержащему сохраненный документ-видеораскадровку</param>
        void OpenDocument(string filePath)
        {
            using(FileStream fileStream = new FileStream(filePath, FileMode.Open)) {
                StoryboardDocument newDocument = StoryboardDocument.LoadFromXML(fileStream);

                newDocument.UnloadOntoCanvas(MainCanvas);

                CurrentFilePath = filePath;

                if(NoDocumentMode) {
                    DisableNoDocumentMode();
                }
            }
        }

        /// <summary>
        /// Обратаывает нажатие на пункт меню, соответствующий операции сохранения файла видеораскадровки, инициируя соответствующую операцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void SaveDocumentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(NoDocumentMode) {
                return;
            }

            if(CurrentFilePath == null) {
                SaveDocumentAs();
            }
            else {
                SaveDocument(CurrentFilePath);
            }
        }

        /// <summary>
        /// Обратаывает нажатие на пункт меню, соответствующий операции "сохранения файла видеораскадровки как", инициируя соответствующую операцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void SaveDocumentAsMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            if(NoDocumentMode) {
                return;
            }

            SaveDocumentAs();
        }

        /// <summary>
        /// Загружает с рабочего холста документ-раскадровку и сохраняет его по указанному пути
        /// </summary>
        /// <param name="filePath">Путь, по которому предполагается сохранить документ</param>
        void SaveDocument(string filePath)
        {
            StoryboardDocument document = StoryboardDocument.LoadFromCanvas(MainCanvas);

            try {
                using(FileStream fileStream = new FileStream(filePath, FileMode.Create)) {
                    document.SaveToXML(fileStream);
                }
            }
            catch(Exception ex) {
                MessageBox.Show(this, $"При попытке сохранения файла произошла ошибка\r\n\r\n{ex.Message}");
            }
        }

        /// <summary>
        /// Загружает с рабочего холста документ-раскадровку и сохраняет его по выбираемому пользователем в диалоговом окне пути
        /// </summary>
        void SaveDocumentAs()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Filter = "XML-формат видеораскадровки (*.sb)|*.sb";

            if(saveDialog.ShowDialog().Value) {
                CurrentFilePath = saveDialog.FileName;

                SaveDocument(saveDialog.FileName);
            }
        }

        /// <summary>
        /// Завершает работу программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ExitMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Отображает диалоговое окно "О программе"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void AboutMenuItem_Click_1(object sender, RoutedEventArgs eventArgs)
        {
            AboutDialog aboutDialog = new AboutDialog();

            aboutDialog.Owner = this;

            aboutDialog.ShowDialog();
        }
    }
}
