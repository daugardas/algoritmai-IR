using System;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace sprendimas
{
    internal class InOut
    {
        public static List<Place> ReadFileToList(string filePath)
        {
            var places = new List<Place>();
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(';');
                    places.Add(new Place(values[0], long.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
                }

            }
            return places;
        }
    }
}
