using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FishyWPF.Models
{
    public class Location
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Distance { get; set; }

        public Location(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Location()
        {

        }

        public void CalculateDistance(Point fish)
        {
            Distance = Point.Subtract(new Point(X, Y), fish).Length;
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }
    }
}
