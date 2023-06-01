using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UserBubble
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Bubble : UserControl
    {

        public Bubble()
        {
            InitializeComponent();
            LocalText = TextShower.Text;
            this.Width = Constant.NAMEWIDTH;
            this.Height = Constant.NAMEHEIGHT;
            TextShower.FontSize = Constant.FONTSIZE;
        }

        
        public delegate void FinishedHandler(Bubble sender);
        public event FinishedHandler Finished;

        public double Step { get; set; }

        private string LocalText;

        public void SetText(string text)
        {
            LocalText = text;
            this.Dispatcher.Invoke(() =>
            {
                TextShower.Text = text;
            });
        }

        public string GetText()
        {
            return LocalText;
        }

        public double CurrentPosInBezier { get; set; }

        public void ChangePosition()
        {
            CurrentPosInBezier += Step;
            if (CurrentPosInBezier > 1)
            {
                CurrentPosInBezier--;
                if (Finished != null)
                {
                    Finished(this);
                }
            }
        }

        public Vector2 MyBezierFunc() //获取当前t值贝塞尔曲线的坐标
        {
            return Bezier(Constant.BEZIERPOINTS, (float)CurrentPosInBezier);
        }

        private static int BinomialCoef(int n, int r)
        {
            if (r == 0 || r == n) return 1;
            else return Factorial(n) / (Factorial(r) * Factorial(n - r));
        }

        private static int Factorial(int num)
        {
            if (num == 0)
            {
                return 0;
            }
            else if (num == 1)
            {
                return 1;
            }
            else
            {
                return num * Factorial(num - 1);
            }
        }

        private static Vector2 Bezier(Vector2[] points, float t)
        {
            Vector2 result = new Vector2(0, 0);
            int degree = points.Length - 1;
            for (int i = 0; i <= degree; i++)
            {
                result += BinomialCoef(degree, i) * points[i] * (float)Math.Pow(t, i) * (float)Math.Pow(1 - t, degree - i);
            }
            return result;
        }
    }
}