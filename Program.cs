using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace sprendimas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // set console encoding to UTF8
            Console.OutputEncoding = Encoding.UTF8;
            //

            List<Place> places = new List<Place>();
            string fileName = "places_data.txt";
            string filePath = Path.Combine(Environment.CurrentDirectory, @"..\..\", fileName);
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(';');
                    places.Add(new Place(values[0], long.Parse(values[1]), double.Parse(values[2]), double.Parse(values[3])));
                }

            }

            Console.WriteLine("Places loaded: " + places.Count);
        }
    }

    class Place
    {
        public string Name { get; }
        public long ID { get; }
        public double X { get; }
        public double Y { get; }
        public Place(string name, long id, double x, double y)
        {
            Name = name;
            ID = id;
            X = x;
            Y = y;
        }

        public distanceTo(Place place)
        {

        }
    }
}
