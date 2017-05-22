using System;
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

            BackgroundManager bg = new BackgroundManager(2);

            bg.InitializeCanvas(MainCanvas);

            bg.AddImageToFrame(0, new BitmapImage(new Uri(@"C:\Users\Public\Pictures\Sample Pictures\Chrysanthemum.jpg")));
            bg.AddImageToFrame(1, new BitmapImage(new Uri(@"C:\Users\Public\Pictures\Sample Pictures\Desert.jpg")));
        }

        private void NewDocumentMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            (new NewDocumentDialog()).ShowDialog();
        }
    }
}
