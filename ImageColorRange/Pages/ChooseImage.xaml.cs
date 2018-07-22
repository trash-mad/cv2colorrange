using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageColorRange
{
    /// <summary>
    /// Логика взаимодействия для ChooseImage.xaml
    /// </summary>
    public partial class ChooseImage : System.Windows.Controls.UserControl
    {
        public Action<string> Next { get; set; }

        public ChooseImage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Изображение|*.jpg";
            openFileDialog1.Title = "Выберите картинку";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Console.WriteLine("Выбран файл: {0}", openFileDialog1.FileName);
                Next(openFileDialog1.FileName);
            }
        }
    }
}
