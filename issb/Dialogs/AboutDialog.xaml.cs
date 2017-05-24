using System.Windows;

namespace issb
{
    /// <summary>
    /// Описывает поведение диалогового окна "О программе"
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
