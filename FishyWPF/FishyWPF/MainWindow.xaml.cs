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
using System.Windows.Threading;
using Point = System.Windows.Point;

namespace FishyWPF
{
    




    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly System.Windows.Threading.DispatcherTimer _dispatcherTimer = new();
        private readonly DispatcherTimer _RemoveFood = new DispatcherTimer();

        private Location foodLocation = new Location();
        List<Point> points = new List<Point>();

        List<Food> foodList = new List<Food>();

        private Point _FishLocation = new Point();

        private bool moving = false;

        public MainWindow()
        {
            InitializeComponent();

            MoveFish();

            _dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick!);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,100);
            _dispatcherTimer.Start();

            _RemoveFood.Tick += new EventHandler(_RemoveFood_Tick!);
            _RemoveFood.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _RemoveFood.Start();


            

        }

        private void _RemoveFood_Tick(object? sender, EventArgs e)
        {
            //Debug.WriteLine("foodlist count: " + foodList.Count);
            if (foodList.Count < 0) return;


            Point moveImagePoint = FishImage.TransformToAncestor(Application.Current.MainWindow)
                .Transform(new Point(0, 0));
            moveImagePoint.X += FishImage.Width / 2;
            moveImagePoint.Y += FishImage.Height / 2;

            var selectedPointList = foodList.Select(x => new Location(x.FoodLocation.X, x.FoodLocation.Y)).ToList();
            foreach (var location in selectedPointList)
            {
                location.X += 16;
                location.Y += 16;
                location.CalculateDistance(moveImagePoint);
            }

            

            foreach (var location in selectedPointList)
            {
                //Debug.WriteLine("Distance: " + location.Distance);
                if (location.Distance < 30)
                {
                    var location1 = location;
                    location1.X -= 16;
                    location1.Y -= 16;
                    var food = foodList.Where(x => x.HasImage).FirstOrDefault(x => x.FoodLocation == location1.ToPoint());
                    if (food != null)
                    {
                        food.Remove();
                        break;
                    }
                    
                    
                }

                
            }
        }

        

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            MoveFish();
        }


        public void MoveFish()
        {

            if (foodList.Count(x => !x.Moved) <= 0) return;
            if (moving) return;

            var selectedPointList = GetDistanceFishFood();

            var selectedPoint = selectedPointList.OrderBy(x => x.Distance).First();
            var selectedFood = foodList.FirstOrDefault(x => x.FoodLocation == selectedPoint.ToPoint());
            selectedFood!.Moved = true;

            MoveTo(FishImage, selectedPoint.ToPoint());
            

            moving = true;
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foodList.Add(new Food(e.GetPosition(this),paintSurface));

            
        }

        public void MoveTo(Image moveImage,Point targetPoint)
        {
            Point moveImagePoint = moveImage.TransformToAncestor(Application.Current.MainWindow)
                .Transform(new Point(0, 0));

            

            double distance = Point.Subtract(targetPoint, moveImagePoint).Length;
            Debug.WriteLine("Distance: " + distance);


            var angle = GetAngleOfLineBetweenTwoPoints(new PointF((float)moveImagePoint.X, (float)moveImagePoint.Y), new PointF((float)targetPoint.X, (float)targetPoint.Y));

            RotateTransform rotateTransform = new RotateTransform(angle);
            rotateTransform.CenterX = 16;
            rotateTransform.CenterY = 16;
            
            Debug.WriteLine(rotateTransform.CenterX);
            FishImage.RenderTransform = rotateTransform;

            Debug.WriteLine("angle: " + angle);

            if (angle < 90 || angle > 270)
            {
                FishImage.Source = new BitmapImage(
                    new Uri("pack://application:,,,/Pictures/FishMainFlipHor.png"));

            }
            else if (angle > 90 && angle < 270)
            {
                FishImage.Source = new BitmapImage(
                    new Uri("pack://application:,,,/Pictures/FishMain.png"));
            }

            


            var speed = (distance / 100 ) * 1 * 1000;
            Debug.WriteLine("speed: " + speed);
            Debug.WriteLine("----");
            // double whereX = 0;
            // double whereY = 0;

            //whereX = moveImagePoint.X > targetPoint.X ? moveImagePoint.X - targetPoint.X : moveImagePoint.X + targetPoint.X;
            //whereY = moveImagePoint.Y > targetPoint.Y ? moveImagePoint.Y - targetPoint.Y : moveImagePoint.Y + targetPoint.Y;



            Storyboard myMovementStoryboard = new Storyboard();

            //X
            DoubleAnimation doubleAnimationX = new DoubleAnimation { From = moveImagePoint.X, To = targetPoint.X - moveImage.Width/2, Duration = new Duration(TimeSpan.FromMilliseconds(speed)) };
            Storyboard.SetTarget(doubleAnimationX, moveImage);
            Storyboard.SetTargetProperty(doubleAnimationX, new PropertyPath("(Canvas.Left)"));
            
            //Y
            DoubleAnimation doubleAnimationY = new DoubleAnimation { From = moveImagePoint.Y, To = targetPoint.Y - moveImage.Height/2, Duration = new Duration(TimeSpan.FromMilliseconds(speed)) };
            Storyboard.SetTarget(doubleAnimationY, moveImage);
            Storyboard.SetTargetProperty(doubleAnimationY, new PropertyPath("(Canvas.Top)"));
            


            myMovementStoryboard.Children.Add(doubleAnimationX);
            myMovementStoryboard.Children.Add(doubleAnimationY);
            myMovementStoryboard.Completed += (sender, eArgs) =>
            {
                moving = false;
                _FishLocation = moveImage.TransformToAncestor(Application.Current.MainWindow)
                    .Transform(new Point(0, 0));
                MoveFish();
            };
            myMovementStoryboard.Begin();

            
            

        }

        public List<Location> GetDistanceFishFood()
        {
            var selectedPointList = foodList.Where(x => x.Moved == false).Select(x => new Location(x.FoodLocation.X, x.FoodLocation.Y)).ToList();
            foreach (var location in selectedPointList)
            {
                location.CalculateDistance(_FishLocation);
            }

            return selectedPointList;
        }
        public static double GetAngleOfLineBetweenTwoPoints(PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            var res =  Math.Atan2(yDiff, xDiff) * (180 / Math.PI);

            if (res > 0 && res < 180)
            {
                
                return res;
            }

            if (res < 0)
            {
                res *= -1;
                return res;
            }


            return res;
        }
    }
}
