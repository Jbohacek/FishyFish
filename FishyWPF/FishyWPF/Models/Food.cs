using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FishyWPF.Models
{
    public class Food
    {
        public Image FoodImage;
        public Point FoodLocation;

        private Canvas Parent;
        public bool Moved = false;
        public bool HasImage = true;

        public Food(Point mousePoint,Canvas parentCanvas)
        {
            BitmapImage image = new BitmapImage(
                new Uri("pack://application:,,,/Pictures/Food.png"));
            Image aa = new Image
            {
                Width = 15,
                Height = 15,

                Source = image
            };

            FoodImage = aa;
            Parent = parentCanvas;
            FoodLocation = new Point(mousePoint.X,mousePoint.Y);

            parentCanvas.Children.Add(FoodImage);
            Canvas.SetLeft(FoodImage,FoodLocation.X - FoodImage.Width/2);
            Canvas.SetTop(FoodImage,FoodLocation.Y - FoodImage.Height/2);
        }

        public void Remove()
        {
            Parent.Children.Remove(FoodImage);
            HasImage = false;
        }
        
    }
}
