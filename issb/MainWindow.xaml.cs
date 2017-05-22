using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace issb {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            /*
             * TODO: remove
             */

            base.OnContentRendered(e);
            
            NewDocumentMenuItem_Click(null, null);
        }

        private void NewDocumentMenuItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            (new NewDocumentDialog()).ShowDialog();
        }
    }
}
