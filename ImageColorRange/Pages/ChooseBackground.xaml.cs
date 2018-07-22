using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Логика взаимодействия для ChooseBackground.xaml
    /// </summary>
    public partial class ChooseBackground : System.Windows.Controls.UserControl
    {
        Bitmap myBitmap = null;
        public Action<string,string> Next { get; set; }

        private void DrawBitmap()
        {
            Console.WriteLine("Очистка окна");
            foreach(UIElement child in CurrentImage.Children)
            {
                if(child is System.Windows.Shapes.Rectangle)
                {
                    ((System.Windows.Shapes.Rectangle)child).MouseDown -= ShowColor;
                }
            }

            CurrentImage.Children.Clear();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Console.WriteLine("Начало вывода");
            int count = 0;
            int result = myBitmap.Height * myBitmap.Width;

            for (int x = 0; x < myBitmap.Width; x++)
            {
                for (int y = 0; y < myBitmap.Height; y++)
                {
                    System.Drawing.Color pixelColor = myBitmap.GetPixel(x, y);
                    var rectangle = new System.Windows.Shapes.Rectangle()
                    {
                        Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(pixelColor.R, pixelColor.G, pixelColor.B)),
                        Width = Convert.ToDouble(1),
                        Height = Convert.ToDouble(1),
                        Margin = new Thickness(left: x, top: y, right: 0, bottom: 0)
                    };
                    rectangle.MouseDown += ShowColor;
                    CurrentImage.Children.Add(rectangle);
                    count++;
                    Console.WriteLine("Вывод пикселей: {0}/{1}", count, result);
                }
            }

            Console.WriteLine("Обновление окна");
        }

        private void ShowColor(object sender, MouseButtonEventArgs e)
        {
            var rec = sender as System.Windows.Shapes.Rectangle;
            var solid = rec.Fill as SolidColorBrush;
            if (solid != null)
            {
                ColorRectangle.Fill = solid;
                ColorText.Text = string.Format("RGB({0}, {1}, {2})", solid.Color.R, solid.Color.G, solid.Color.B);
            }
        }

        public ChooseBackground(string path)
        {
            InitializeComponent();

            myBitmap = new Bitmap(path);
            CurrentImage.Width = myBitmap.Width;
            CurrentImage.Height = myBitmap.Height;

            DrawBitmap();
        }

        private void ApplyFilter(object sender, RoutedEventArgs e)
        {
            int blackdelta = 0;
            int cornerdelta = 0;
            int tmp=0;

            bool needUpdate=false;

            int count = 0;
            int result = myBitmap.Height * myBitmap.Width;

            System.Drawing.Color black = System.Drawing.Color.FromArgb(0, 0, 0);

            if (Int32.TryParse(BlackDelta.Text,out blackdelta))
            {
                Console.WriteLine("Обработка серых пикселей по погрешности");
                for (int x = 0; x < myBitmap.Width; x++)
                {
                    for (int y = 0; y < myBitmap.Height; y++)
                    {
                        System.Drawing.Color pixelColor = myBitmap.GetPixel(x, y);
                        tmp = pixelColor.R + pixelColor.G + pixelColor.B;
                        if (tmp < blackdelta)
                        {
                            pixelColor = black;
                            myBitmap.SetPixel(x, y, pixelColor);
                        }
                        count++;
                        Console.WriteLine("Обработано пикселей: {0}/{1}", count, result);
                    }
                }
                count = 0;
                needUpdate = true;
            }

            if (Int32.TryParse(CornerDelta.Text, out cornerdelta))
            {
                Console.WriteLine("Обработка краев изображения относительно черных пикселей");

                tmp = cornerdelta;

                //Слева-направо
                for (int y = 0; y < myBitmap.Height; y++)
                {
                    for (int x = 0; x < myBitmap.Width; x++)
                    {
                        System.Drawing.Color pixelColor = myBitmap.GetPixel(x, y);

                        if (pixelColor.R != 0||pixelColor.G!=0||pixelColor.B!=0)
                        {
                            if (tmp >= 0)
                            {
                                tmp--;
                                myBitmap.SetPixel(x, y, black);
                                Console.WriteLine("Обработка слева-направо. Пиксель({0}, {1}). Обработано: {2}/{3}", x, y, count, result);
                            }
                        }
                        else
                        {
                            tmp = cornerdelta;
                        }
                        count++;
                    }
                }
                count = 0;

                //Справа-налево
                for (int y = myBitmap.Height - 1; y != 0; y--)
                {
                    for (int x = myBitmap.Width - 1; x != 0; x--)
                    {
                        System.Drawing.Color pixelColor = myBitmap.GetPixel(x, y);

                        if (pixelColor.R != 0 || pixelColor.G != 0 || pixelColor.B != 0)
                        {
                            if (tmp >= 0)
                            {
                                tmp--;
                                myBitmap.SetPixel(x, y, black);
                                Console.WriteLine("Обработка справа-налево. Пиксель({0}, {1}). Обработано: {2}/{3}", x, y, count, result);
                            }
                        }
                        else
                        {
                            tmp = cornerdelta;
                        }
                        count++;
                    }
                }
                count = 0;

                //Сверху-вниз
                for (int x = 0; x < myBitmap.Width; x++)
                {
                    for (int y = 0; y < myBitmap.Height; y++)
                    {
                        System.Drawing.Color pixelColor = myBitmap.GetPixel(x, y);

                        if (pixelColor.R != 0 || pixelColor.G != 0 || pixelColor.B != 0)
                        {
                            if (tmp >= 0)
                            {
                                tmp--;
                                myBitmap.SetPixel(x, y, black);
                                Console.WriteLine("Обработка сверху-вниз. Пиксель({0}, {1}). Обработано: {2}/{3}", x, y, count, result);
                            }
                        }
                        else
                        {
                            tmp = cornerdelta;
                        }
                        count++;
                    }
                }
                count = 0;

                //Снизу-вверх
                for (int x = 0; x < myBitmap.Width; x++)
                {
                    for (int y = myBitmap.Height - 1; y != 0; y--)
                    {
                        System.Drawing.Color pixelColor = myBitmap.GetPixel(x, y);

                        if (pixelColor.R != 0 || pixelColor.G != 0 || pixelColor.B != 0)
                        {
                            if (tmp >= 0)
                            {
                                tmp--;
                                myBitmap.SetPixel(x, y, black);
                                Console.WriteLine("Обработка снизу-вверх. Пиксель({0}, {1}). Обработано: {2}/{3}", x, y, count, result);
                            }
                        }
                        else
                        {
                            tmp = cornerdelta;
                        }
                        count++;
                    }
                }
                count = 0;
                needUpdate = true;


                if (needUpdate) DrawBitmap();
            }
        }

        private void SaveImage(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "jpg files (*.jpg)|*.jpg";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                myBitmap.Save(saveFileDialog1.FileName);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double h_min = 99999999999999999;
            double s_min = 99999999999999999;
            double v_min = 99999999999999999;

            double h_max = 0;
            double s_max = 0;
            double v_max = 0;

            int count = 0;
            int result = myBitmap.Height * myBitmap.Width;

            string fname = System.IO.Path.Combine(System.IO.Path.GetTempPath(), string.Format("script{0}.py",Guid.NewGuid().ToString()));
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("ImageColorRange.Script.py"))
            using (Stream output = File.Create(fname))
            {
                input.CopyTo(output);
            }


            for (int x = 0; x < myBitmap.Width; x++)
            {
                for (int y = 0; y < myBitmap.Height; y++)
                {
                    System.Drawing.Color pixelColor = myBitmap.GetPixel(x, y);
                    if (pixelColor.R != 0 || pixelColor.G != 0 || pixelColor.B != 0)
                    {
                        var proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "python.exe",
                                Arguments = string.Format("{0} {1} {2} {3}",fname,pixelColor.B,pixelColor.G,pixelColor.R),
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            }
                        };


                        proc.Start();
                        while (!proc.StandardOutput.EndOfStream)
                        {
                            string line = proc.StandardOutput.ReadLine();
                            Console.WriteLine(line);
                            var arr = line.Split(',');

                            var hsv = new { H = int.Parse(arr[0]), S = int.Parse(arr[1]), V = int.Parse(arr[2]) };

                            if (h_min > hsv.H) h_min = hsv.H;
                            if (s_min > hsv.S) s_min = hsv.S;
                            if (v_min > hsv.V) v_min = hsv.V;

                            if (h_max < hsv.H) h_max = hsv.H;
                            if (s_max < hsv.S) s_max = hsv.S;
                            if (v_max < hsv.V) v_max = hsv.V;
                        }


                    }
                    
                    count++;
                    Console.WriteLine("Выделение диапозона цветов: {0}/{1}", count, result);
                }
            }

            Console.WriteLine("Минимум: HSV({0};{1};{2})", h_min, s_min, v_min);
            Console.WriteLine("Максимум: HSV({0};{1};{2})", h_max, s_max, v_max);

            Next(string.Format("lower_color = np.array([{0},{1},{2}], dtype=numpy.uint8)", h_min, s_min, v_min),string.Format("upper_color = np.array([{0},{1},{2}], dtype=numpy.uint8)", h_max, s_max, v_max));
        }
    }
}
