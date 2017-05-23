using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace issb
{
    public partial class NewDocumentDialog : Window
    {
        BackgroundTemplate _SelectedTemplate;

        public BackgroundTemplate SelectedTemplate
        {
            get
            {
                return _SelectedTemplate;
            }
            private set
            {
                _SelectedTemplate = value;
                Close();
            }
        }

        public NewDocumentDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(Radio1.IsChecked.Value) {

            }
            else if(Radio2.IsChecked.Value) {
                try {
                    using(FileStream fs = new FileStream(FilePath.Text, FileMode.Open)) {
                        SelectedTemplate = BackgroundTemplate.ReadFromXML(fs);
                    }
                }
                catch(Exception ex) {
                    MessageBox.Show(this, $"При попытке открытия файла произошла ошибка.\r\n\r\n{ex.Message}");
                }
            }

        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Multiselect = false;
            dialog.Filter = "XML-файлы (*.xml)|*.xml";

            if(dialog.ShowDialog().Value) {
                FilePath.Text = dialog.FileName;
            }
        }
    }
}
