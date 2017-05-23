using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace issb {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        BackgroundManager CurrentBackgoundManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs eventArgs)
        {
            base.OnContentRendered(eventArgs);
            
            //CreateNewDocument();
        }

        private void NewDocumentMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            CreateNewDocument();
        }

        private void CreateNewDocument()
        {
            NewDocumentDialog newDocumentDialog = new NewDocumentDialog();

            newDocumentDialog.Owner = this; // for WidnowsStartupLocation="CenterOwner"

            newDocumentDialog.ShowDialog();

            if(newDocumentDialog.SelectedTemplate != null) {
                MainCanvas.Children.Clear();

                CurrentBackgoundManager = new BackgroundManager(newDocumentDialog.SelectedTemplate);

                CurrentBackgoundManager.InitializeCanvas(MainCanvas);
            }
        }

        private void ImportItemsMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.Multiselect = true;

            if(openDialog.ShowDialog().Value) {
                ImportImagesDialog importDialog = new ImportImagesDialog();

                importDialog.FilesToImport = openDialog.FileNames;

                importDialog.Owner = this;

                importDialog.ShowDialog();
            }
        }

        private void ImportBackgroundImagesMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
