using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace issb {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs eventArgs)
        {
            /*
             * TODO: remove
             */

            base.OnContentRendered(eventArgs);

            //NewDocumentMenuItem_Click(null, null);

            BackgroundTemplate bTempalte = null;

            using(FileStream fs = new FileStream(@"BackgroundTemplates\template010.xml", FileMode.Open)) {
                bTempalte = BackgroundTemplate.ReadFromXML(fs);
            }

            BackgroundManager bManager = new BackgroundManager(bTempalte);

            bManager.InitializeCanvas(MainCanvas);

            bManager.AddImageToFrame(0, new BitmapImage(new Uri(@"C:\Users\Public\Pictures\Sample Pictures\Chrysanthemum.jpg")));
            bManager.AddImageToFrame(1, new BitmapImage(new Uri(@"C:\Users\Public\Pictures\Sample Pictures\Desert.jpg")));

            bTempalte.WriteToXML(new FileStream(@"BackgroundTemplates\template010.xml", FileMode.OpenOrCreate));
        }

        private void NewDocumentMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            (new NewDocumentDialog()).ShowDialog();
        }
    }
}
