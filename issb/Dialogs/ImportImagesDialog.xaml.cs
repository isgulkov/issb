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

        BackgroundWorker LoadImagesWorker = new BackgroundWorker();

        public ImportImagesDialog()
        {
            InitializeComponent();

            LoadImagesWorker.WorkerSupportsCancellation = true;
            LoadImagesWorker.WorkerReportsProgress = true;

            LoadImagesWorker.DoWork += LoadImagesWorker_DoWork;
            LoadImagesWorker.ProgressChanged += LoadImagesWorker_ProgressChanged;
            LoadImagesWorker.RunWorkerCompleted += LoadImagesWorker_RunWorkerCompleted;
        }
        
        void LoadImagesWorker_DoWork(object sender, DoWorkEventArgs eventArgs)
        {
            List<BitmapImage> bitmaps = new List<BitmapImage>();

            foreach(string filename in FilesToImport) {
                try {
                    Thread.Sleep(1000 / FilesToImport.Length);

                    LoadImagesWorker.ReportProgress(0, filename);
                }
                catch(Exception) { }

                if(LoadImagesWorker.CancellationPending) {
                    break;
                }
            }

            LoadedBitmaps = bitmaps;
        }

        void LoadImagesWorker_ProgressChanged(object sender, ProgressChangedEventArgs eventArgs)
        {
            ListBoxItem newItem = new ListBoxItem();

            newItem.Content = eventArgs.UserState as string;

            ProcessedFilesListBox.Items.Add(newItem);
        }

        void LoadImagesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs eventArgs)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            LoadImagesWorker.RunWorkerAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs eventArgs)
        {
            LoadImagesWorker.CancelAsync();
        }
    }
}
