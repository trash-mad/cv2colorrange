using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageColorRange
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ChooseImage ci = new ChooseImage();
            ChooseBackground cb;
            DetectedColor dt;
            ci.Next = (path) =>
            {
                cb = new ChooseBackground(path);
                cb.Next = (a, b) =>
                {
                    dt = new DetectedColor(a, b);
                    PageControl.ShowPage(dt);
                };
                PageControl.ShowPage(cb);
            };
            PageControl.ShowPage(ci);
        }
    }
}
