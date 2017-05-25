using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace issb
{
    /// <summary>
    /// Описывает поведение диалогового окна, предназначенного для загрузки изображений из переданного списка файлов в соответствующие им объекты <see cref="BitmapImage"/>
    /// </summary>
    public partial class ImportImagesDialog : Window
    {
        /// <summary>
        /// Передаваемый диалоговому окну список файлов
        /// </summary>
        public string[] FilesToImport { get; set; }

        /// <summary>
        /// Коллекция вовзращаемых окном объектов BitmapImage
        /// </summary>
        public IReadOnlyCollection<BitmapImage> LoadedBitmaps { get; private set; }

        public ImportImagesDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Создает соответствующие элементам списка файлов <see cref="FilesToImport"/> объекты <see cref="BitmapImage"/> и размещает их в свойстве <see cref="LoadedBitmaps"/>, после чего закрывает окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void Window_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            List<BitmapImage> loadedBitmaps = new List<BitmapImage>();

            foreach(string filename in FilesToImport) {
                try {
                    loadedBitmaps.Add(new BitmapImage(new Uri(filename)));

                    ListBoxItem newItem = new ListBoxItem();
                    newItem.Content = filename;
                    ProcessedFilesListBox.Items.Add(newItem);
                }
                catch(Exception) { }
            }

            LoadedBitmaps = loadedBitmaps;

            Close();
        }
    }
}
