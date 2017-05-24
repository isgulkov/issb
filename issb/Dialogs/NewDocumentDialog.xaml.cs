using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Win32;

namespace issb
{
    /// <summary>
    /// Описывает поведение диалогового окна создания нового документа
    /// </summary>
    public partial class NewDocumentDialog : Window
    {
        BackgroundTemplate _SelectedTemplate;

        /// <summary>
        /// Передаваемый диалоговому окну набор шаблонов фона раскадровки для отображения в соответствующем выпадающем списке или т.п.
        /// </summary>
        public IReadOnlyCollection<BackgroundTemplate> PresetTemplates { get; set; }

        /// <summary>
        /// Выбранный шаблон фона, возвращаемый диалоговым окном. В случае нажатия пользователем кнопки "Отмена", значение данного поля остается null
        /// </summary>
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

        /// <summary>
        /// Обрабатывает нажатие пользователем кнопки "Создать", записывает выбранный пользователем шаблон в поле <see cref="SelectedTemplate"/>. В случае успешного завершения данной операции окно закрыватеся
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
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

        /// <summary>
        /// Обрабатывает нажатие пользователем кнопки "Отмена", закрывает окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void CancelButtonClick(object sender, RoutedEventArgs eventArgs)
        {
            Close();
        }

        /// <summary>
        /// Открывает диалоговое окно открытия файла, выбранный пользователем файл записывает в соотствующий элемент <see cref="TextBox"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void BrowseButtonClick(object sender, RoutedEventArgs eventArgs)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Multiselect = false;
            dialog.Filter = "XML-файлы (*.xml)|*.xml";

            if(dialog.ShowDialog().Value) {
                FilePath.Text = dialog.FileName;
            }
        }

        /// <summary>
        /// Инициализирует выпадающий список шаблона переданным данному окну в поле <see cref="PresetTemplates"/> набором щаблонов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
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
