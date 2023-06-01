using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using System.Windows.Threading;
using UserBubble;


namespace Lottery101
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> studentNameList;
        LinkedList<Bubble> bubbleList;
        Timer timer;
        Random rnd;
        double velocity = 0;
        int flag = 0;
        bool showResult = false;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        /**
         * 刷新函数。每次调用都会计算并更改灯笼位置
         */
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            double velocity_locked = velocity;
            for (LinkedListNode<Bubble> node = bubbleList.First; node != null; node = node.Next)
            {
                Bubble bubble = node.Value;
                bubble.Step = velocity_locked;
                bubble.ChangePosition();  // 更新贝塞尔曲线t值
                Vector2 pos = bubble.MyBezierFunc(); // 根据当前t值计算坐标
                this.Dispatcher.Invoke(new Action(() => // 在UI线程更新UI
                {
                    Canvas.SetLeft(bubble, TransformX(pos.X));
                    Canvas.SetTop(bubble, TransformY(pos.Y));
                }));
            }
            if (velocity > 0)
            {
                if (flag == 0)  // 如果是自动减速模式（0；自动减速模式，1：手动停止模式）
                    velocity -= Constant.DECELERATION;
            }
            else
            {
                velocity = 0;
                if (showResult)
                {
                    showResult = false;
                    string showingContext = "";
                    string[] names = getCurrentNames();
                    for (int i = 0;i < Constant.WINNERS;i++)
                    {
                        showingContext += names[i] + "  ";
                    }
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ResultTextBlock.Text = showingContext;
                        ShowResult();
                    }));
                }
            }
        }

        private void Sort(double[] source, string[] names)
        {
            for (int i = 0; i < source.Length - 1; i++)
            {
                for (int j  = 0; j < source.Length - 1 - i; j++)
                {
                    if (source[j] > source[j+1])
                    {
                        Swap(source, j, j + 1);
                        Swap(names, j, j + 1);
                    }
                }
            }
        }

        private void Swap(double[] array, int index1, int index2)
        {
            double temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

        private void Swap(string[] array, int index1, int index2)
        {
            string temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

        private string[] getCurrentNames()
        {
            int index = 0;
            string[] names = new string[bubbleList.Count];
            double[] pos = new double[bubbleList.Count];
            foreach (Bubble bubble in bubbleList)
            {
                pos[index] = Math.Abs(bubble.CurrentPosInBezier - 0.5);
                names[index++] = bubble.GetText();
            }
            Sort(pos, names);
            return names;
        }

        /**
         * x坐标转换函数。将贝塞尔曲线得到的x坐标转换为实际x坐标
         */
        private double TransformX(double x)
        {
            return MyCanvas.ActualWidth * x * 0.01 - Constant.NAMEWIDTH / 2;
        }

        /**
         * y坐标转换函数。将贝塞尔曲线得到的y坐标转换为实际y坐标
         */
        private double TransformY(double y)
        {
            return MyCanvas.ActualHeight * y * 0.01 - Constant.NAMEHEIGHT / 2;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rnd = new Random();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excelPackage = new ExcelPackage("./names.xlsx");
            ExcelWorksheet sheet = excelPackage.Workbook.Worksheets.First();
            studentNameList = new List<string>(1000);
            foreach (var cell in sheet.Cells)
            {
                studentNameList.Add(cell.Text);
            }
            sheet.Dispose();
            excelPackage.Dispose();

            bubbleList = new LinkedList<Bubble>();
            double offsetPos = 0.5 / Constant.NAMESPERPAGE;  // 确保第一次显示在中心位置的偏移量
            for (int i = 0; i < Constant.NAMESPERPAGE; i++)
            {
                Bubble bubble = new Bubble();
                bubble.CurrentPosInBezier = (double)i / Constant.NAMESPERPAGE + offsetPos;
                bubble.Finished += Bubble_Finished;
                bubble.SetText(GetRandomName());

                Vector2 pos = bubble.MyBezierFunc();
                Canvas.SetLeft(bubble, TransformX(pos.X));
                Canvas.SetTop(bubble, TransformY(pos.Y));

                MyCanvas.Children.Add(bubble);
                bubbleList.AddLast(bubble);
            }

            timer = new Timer();  // 使用Timer来调用刷新函数更新UI
            timer.Interval = 15;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        /**
         * 随机获取名字。并在获取到之后从列表里移除
         */
        private string GetRandomName()
        {
            int index = rnd.Next(studentNameList.Count);
            string name = studentNameList[index];
            studentNameList.RemoveAt(index);
            return name;
        }

        /**
         * 灯笼运动到终点触发
         */
        private void Bubble_Finished(Bubble sender)
        {
            string passeddName = sender.GetText();
            sender.SetText(GetRandomName());
            studentNameList.Add(passeddName);
        }

        private void LotteryButton_Click(object sender, RoutedEventArgs e)
        {
            HideResult();
            flag = 0;
            showResult = true;
            velocity = Constant.VELOCITY;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            flag = 1;
            if ("开始".Equals(StartButton.Content))
            {
                HideResult();
                velocity = Constant.VELOCITY;
                StartButton.Content = "停止";
            }
            else
            {
                showResult = true;
                velocity = 0;
                StartButton.Content = "开始";
            }
        }

        private void HideResult()
        {
            ResultShowingButton.Content = "显示";
            ResultWindow.Visibility = Visibility.Collapsed;
        }

        private void ShowResult()
        {
            ResultShowingButton.Content = "隐藏";
            ResultWindow.Visibility = Visibility.Visible;
        }

        private void ResultShowingButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultWindow.Visibility == Visibility.Visible)
            {
                HideResult();
            }
            else
            {
                ShowResult();
            }
        }
    }
}
