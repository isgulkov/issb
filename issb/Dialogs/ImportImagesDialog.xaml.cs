using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace issb
{
    /// <summary>
    /// Диалоговое окно, предназначенное для загрузки изображений из переданного списка файлов в соответствующие им объекты BitmapImage
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
