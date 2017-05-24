using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Win32;

namespace issb
{
    public partial class NewDocumentDialog : Window
    {
        BackgroundTemplate _SelectedTemplate;

        /// <summary>
        /// Передаваемый диалоговому окну набор шаблонов фона раскадровки
        /// </summary>
        public IReadOnlyCollection<BackgroundTemplate> PresetTemplates { get; set; }

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

        private void CreateButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            if(Radio1.IsChecked.Value) {
                SelectedTemplate = PresetTemplates.ElementAt(TemplatesComboBox.SelectedIndex);
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

        private void CancelButtonClick(object sender, RoutedEventArgs eventArgs)
        {
            Close();
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs eventArgs)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Multiselect = false;
            dialog.Filter = "XML-файлы (*.xml)|*.xml";

            if(dialog.ShowDialog().Value) {
                FilePath.Text = dialog.FileName;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            if(PresetTemplates != null) {
                foreach(BackgroundTemplate presetTemplate in PresetTemplates) {
                    ComboBoxItem newComboBoxItem = new ComboBoxItem();

                    newComboBoxItem.Content = $"Template with {presetTemplate.NumFrames} frames";

                    TemplatesComboBox.Items.Add(newComboBoxItem);
                }

                TemplatesComboBox.SelectedIndex = 0;
            }
            else {
                TemplatesComboBox.IsEnabled = false;
            }
        }
    }
}
