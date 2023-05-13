using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Drawing;
using FishyWPF.Functions;
using FishyWPF.Models;
using System.Windows.Media.Animation;
using Point = System.Windows.Point;

namespace FishyWPF
{
    //Distance
    //closer = better




    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly System.Windows.Threading.DispatcherTimer _dispatcherTimer = new();

        private Location foodLocation = new Location();
        List<Point> points = new List<Point>();

        public MainWindow()
        {
            InitializeComponent();

            MoveFish();

            _dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick!);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,100);
            _dispatcherTimer.Start();

            
        }

        private bool moving = false;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            

            MoveFish();
        }


        public void MoveFish()
        {

            if (points.Count <= 0) return;
            if (moving) return;


            MoveTo(FishImage, points.Last());
            points.Remove(points.Last());

            moving = true;
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage image = new BitmapImage(
                new Uri("pack://application:,,,/Pictures/Food.png"));

            Image aa = new Image
            {
                Width = 15,
                Height = 15,

                Source = image
            };

            paintSurface.Children.Add(aa);

            Point p = e.GetPosition(this);
            double x = p.X;
            double y = p.Y;

            Canvas.SetLeft(aa,x);
            Canvas.SetTop(aa,y);

           

            Debug.WriteLine("---------------");

            points.Add(p);
        }

        public void MoveTo(Image moveImage,Point targetPoint)
        {
            Point moveImagePoint = moveImage.TransformToAncestor(Application.Current.MainWindow)
                .Transform(new Point(0, 0));

           // double whereX = 0;
           // double whereY = 0;

           //whereX = moveImagePoint.X > targetPoint.X ? moveImagePoint.X - targetPoint.X : moveImagePoint.X + targetPoint.X;
           //whereY = moveImagePoint.Y > targetPoint.Y ? moveImagePoint.Y - targetPoint.Y : moveImagePoint.Y + targetPoint.Y;



            Storyboard myMovementStoryboard = new Storyboard();

            //X
            DoubleAnimation doubleAnimationX = new DoubleAnimation { From = moveImagePoint.X, To = targetPoint.X - moveImage.Width/2, Duration = new Duration(TimeSpan.FromMilliseconds(5000)) };
            Storyboard.SetTarget(doubleAnimationX, moveImage);
            Storyboard.SetTargetProperty(doubleAnimationX, new PropertyPath("(Canvas.Left)"));
            
            //Y
            DoubleAnimation doubleAnimationY = new DoubleAnimation { From = moveImagePoint.Y, To = targetPoint.Y - moveImage.Height/2, Duration = new Duration(TimeSpan.FromMilliseconds(5000)) };
            Storyboard.SetTarget(doubleAnimationY, moveImage);
            Storyboard.SetTargetProperty(doubleAnimationY, new PropertyPath("(Canvas.Top)"));
            


            myMovementStoryboard.Children.Add(doubleAnimationX);
            myMovementStoryboard.Children.Add(doubleAnimationY);
            myMovementStoryboard.Completed += (sender, eArgs) =>
            {
                moving = false;
                MoveFish();
            };
            myMovementStoryboard.Begin();

            
            

        }
    }
}
