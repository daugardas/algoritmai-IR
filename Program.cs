using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom;

/// <summary>
/// The objective is to find the cheapest route to visit all the places and return to the starting point. 
/// The route is planned for two travelers. Route starting point and ending point match.
/// Same place can't be visited more than once (place is visited when one traveler is visits it).
/// The price between two places is the square root of th distance between them.
/// 
/// First task is to realise a program, which would give an optimal solution to each individual problem.
/// Determine, with what input size the program takes less than 10 seconds.
/// 
/// Second task is to realise a program, which woudl give a locally best solution to each individual problem. 
/// For example, when there is multiple options to choose, choose the best one.
/// 
/// Third task is to realise a program, which would give the solution created by Genetic Optimization method. 
/// The program should not take longer than 60 seconds.
/// </summary>
namespace sprendimas
{
    /// <summary>
    /// Represents a location that can be visited by two travelers. 
    /// </summary>
    class Place
    {
        /// <summary>
        /// The name of the location.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The ID of the location.
        /// </summary>
        public long ID { get; }

        /// <summary>
        /// The X coordinate of the location.
        /// </summary>
        public float X { get; }

        /// <summary>
        /// The Y coordinate of the location.
        /// </summary>
        public float Y { get; }

        /// <summary>
        /// Indicates if the location has been visited by a traveler.
        /// </summary>
        public bool Visited { get; set; }

        /// <summary>
        /// Creates a new instance of a Place.
        /// </summary>
        /// <param name="name">The name of the location.</param>
        /// <param name="id">The ID of the location.</param>
        /// <param name="x">The X coordinate of the location.</param>
        /// <param name="y">The Y coordinate of the location.</param>
        public Place(string name, long id, float x, float y)
        {
            Name = name;
            ID = id;
            X = x;
            Y = y;
            Visited = false;
        }

        /// <summary>
        /// Calculates the distance between this location and another location.
        /// </summary>
        /// <param name="place">The other location.</param>
        /// <returns>The distance between the two locations.</returns>
        public float distanceTo(Place place)
        {
            float dX = X - place.X;
            float dY = Y - place.Y;
            return (float)Math.Sqrt((double)(dX * dX + dY * dY));
        }

        /// <summary>
        /// Calculates the price of traveling from this location to another.
        /// </summary>
        /// <param name="place">The other location.</param>
        /// <returns>The price of traveling between the two locations.</returns>
        public float priceTo(Place place)
        {
            return (float)Math.Sqrt(distanceTo(place));
        }
    }

    internal class Program
    {

        static void Main(string[] args)
        {
            // set console encoding to UTF8
            Console.OutputEncoding = Encoding.UTF8;
            string fileNameFirstTask = "places_data_first_task.txt";
            string filePathFirstTask = Path.Combine(Environment.CurrentDirectory, @"..\..\", fileNameFirstTask);
            string fileNameSecondTask = "places_data.txt";
            string filePathSecondTask = Path.Combine(Environment.CurrentDirectory, @"..\..\", fileNameSecondTask);


            List<Place> secondTaskPlaces = InOut.ReadFileToList(filePathSecondTask);
            Console.WriteLine("Loaded second task places: " + secondTaskPlaces.Count);
            List<Place> firstTaskPlaces = secondTaskPlaces.Take(500).ToList();
            Console.WriteLine("Loaded first task places: " + firstTaskPlaces.Count);
            Stopwatch watch = new Stopwatch();
            //watch.Start();
            //List<Place> firstTaskSolution = SolveFirstTask(firstTaskPlaces, "FirstTaskSolution.bmp"); // can't use more that 10 places, because I run out of memory (I have 16gb)
            //watch.Stop();
            //Console.WriteLine("Solved first task in " + watch.Elapsed + "s. Route price: " + CalculateRoutePrice(firstTaskSolution));
            //watch.Restart();

            //List<Place> secondTaskSolution = SolveSecondTask(secondTaskPlaces, 0, "secondTasksolution.bmp");
            //watch.Stop();
            //Console.WriteLine("Solved second task in " + watch.Elapsed + "s. Route price: " + CalculateRoutePrice(secondTaskSolution));
            //int takeCount = 2;
            //for (int i = 0; i < 10; i++)
            //{
            //    watch.Restart();
            //    List<Place> thirdTaskSolution = SolveThirdTask(secondTaskPlaces.Take(takeCount).ToList(), "thirdTaskSolution.bmp");
            //    watch.Stop();
            //    Console.WriteLine("Solved third task in " + watch.Elapsed + "s. Route price: " + CalculateRoutePrice(thirdTaskSolution) + " ; c: " + thirdTaskSolution.Count);
            //    takeCount *= 2;
            //}

            watch.Restart();
            List<Place> thirdTaskSolution = SolveThirdTask(secondTaskPlaces, "thirdTaskSolution.bmp");
            watch.Stop();
            Console.WriteLine("Solved third task in " + watch.Elapsed + "s. Route price: " + CalculateRoutePrice(thirdTaskSolution));

            // second task using first tasks path
            //watch.Restart();
            //List<Place> secondTaskSolutionWithFirstTaskPlaces = SolveSecondTask(firstTaskPlaces, 0, "secondWithFirst.bmp");
            //watch.Stop();
            //Console.WriteLine("Solved second task with first task places in " + watch.Elapsed + "s. Route price: " + CalculateRoutePrice(secondTaskSolutionWithFirstTaskPlaces));

            //// third task using first tasks path
            //watch.Restart();
            //List<Place> thirdTaskSolutionWithFirstTaskPlaces = SolveThirdTask(firstTaskPlaces, "thirdWithFirst.bmp");
            //watch.Stop();
            //Console.WriteLine("Solved third task with first task places in " + watch.Elapsed + "s. Route price: " + CalculateRoutePrice(thirdTaskSolutionWithFirstTaskPlaces));
        }

        public static List<Place> SolveFirstTask(List<Place> places, string fileName)
        {
            // we generate all permutation of the list of places, where each permutation represents a possible route
            var permutations = GetPermutations(places, places.Count);

            double cheapestPrice = double.MaxValue;
            List<Place> cheapestRoute = null;

            foreach (var permutation in permutations)
            {
                double price = CalculateRoutePrice(permutation);

                if (price < cheapestPrice)
                {
                    cheapestPrice = price;
                    cheapestRoute = permutation;
                }
            }

            // add the last travel from the last point to the first point
            cheapestRoute.Add(places[0]);

            float maxX = -1, maxY = -1;
            float originX = float.PositiveInfinity, originY = float.PositiveInfinity;
            getWorldSizes(cheapestRoute, ref maxX, ref maxY, ref originX, ref originY);
            DrawGraph(fileName, cheapestRoute, originX, originY, maxX, maxY);
            return cheapestRoute;
        }

        // Method to calculate total price of a route
        public static float CalculateRoutePrice(List<Place> route)
        {
            float total = 0;
            for (int i = 0; i < route.Count - 1; i++)
            {
                total += route[i].priceTo(route[i + 1]);
            }

            // Add price from last place to the first one to complete the round trip
            total += route[route.Count - 1].priceTo(route[0]);
            return total;
        }

        // Method to generate all permutations of a list (all possible routes)
        public static List<List<Place>> GetPermutations(List<Place> list, int length)
        {
            // base case: returns a list
            if (length == 1)
            {
                var result = new List<List<Place>>();
                foreach (var element in list)
                {
                    result.Add(new List<Place> { element });
                }
                return result;
            }

            var previousPermutations = GetPermutations(list, length - 1);
            var permutations = new List<List<Place>>();

            foreach (var permutation in previousPermutations)
            {
                foreach (var element in list)
                {
                    if (!permutation.Contains(element))
                    {
                        var newPermutation = new List<Place>(permutation) { element };
                        permutations.Add(newPermutation);
                    }
                }
            }

            return permutations;
        }

        public static List<Place> SolveSecondTask(List<Place> places, int startIndex, string filename)
        {
            List<Place> solution = new List<Place>();
            places[startIndex].Visited = true;
            solution.Add(places[startIndex]);
            int currentPlaceIndex = startIndex;

            float maxX = -1, maxY = -1;
            float originX = float.PositiveInfinity, originY = float.PositiveInfinity;

            while (solution.Count < places.Count)
            {
                // find the cheapest place index from the current place index
                int cheapestIndex = SecondTaskFindCheapestPlaceToTravel(currentPlaceIndex, places);
                currentPlaceIndex = cheapestIndex;
                places[currentPlaceIndex].Visited = true;
                solution.Add(places[currentPlaceIndex]);
                if (places[currentPlaceIndex].X > maxX)
                {
                    maxX = places[currentPlaceIndex].X;
                }
                if (places[currentPlaceIndex].Y > maxY)
                {
                    maxY = places[currentPlaceIndex].Y;
                }
                if (places[currentPlaceIndex].X < originX)
                {
                    originX = places[currentPlaceIndex].X;
                }
                if (places[currentPlaceIndex].Y < originY)
                {
                    originY = places[currentPlaceIndex].Y;
                }
            }

            // the last travel should go from the last visited place to the starting point
            solution.Add(solution[0]);

            // draw solution
            DrawGraph(filename, solution, originX, originY, maxX, maxY);

            return solution;
        }

        public static int SecondTaskFindCheapestPlaceToTravel(int from, List<Place> places)
        {
            int cheapestIndex = 0;
            double cheapest = double.PositiveInfinity;
            for (int i = 0; i < places.Count; i++)
            {
                // if the current place is already visited, skip it
                if (!places[i].Visited)
                {
                    double price = places[from].priceTo(places[i]);
                    if (price < cheapest)
                    {
                        cheapest = price;
                        cheapestIndex = i;
                    }
                }
            }

            return cheapestIndex;
        }

        public static List<Place> SolveThirdTask(List<Place> places, string fileName)
        {
            // parameters
            int populationSize = 190;
            double crossoverProbability = 0.8;
            double mutationProbabilty = 0.1;
            int maximumIterationCount = 350;
            int tournamentSize = 180;

            // Initialize the population with random solutions
            List<List<Place>> population = new List<List<Place>>();
            for (int i = 0; i < populationSize; i++)
            {
                var route = new List<Place>();
                for (int j = 0; j < places.Count; j++)
                {
                    route.Add(places[j]);
                }
                Shuffle(route);
                population.Add(route);
            }
            List<Place> currentBestRoute = population[0];

            // find best price in population
            float currentBestRoutePrice = CalculateRoutePrice(currentBestRoute);
            for (int i = 1; i < populationSize; i++)
            {
                float routePrice = CalculateRoutePrice(population[i]);
                if (routePrice < currentBestRoutePrice)
                {
                    currentBestRoutePrice = routePrice;
                    currentBestRoute = population[i];
                }
            }

            //Console.WriteLine("currentBest: " + CalculateRoutePrice(currentBestRoute));

            // run the genetic algorithm
            for (int gen = 0; gen < maximumIterationCount; gen++)
            {
                List<List<Place>> newPopulation = new List<List<Place>>();

                // do tournament selection and crossover
                for (int i = 0; i < populationSize; i++)
                {
                    var parent1 = TournamentSelection(population, tournamentSize);
                    var parent2 = TournamentSelection(population, tournamentSize);

                    if (new Random().NextDouble() < crossoverProbability)
                    {
                        var child = Crossover(parent1, parent2);
                        newPopulation.Add(child);
                    }
                    else
                        newPopulation.Add(parent1);
                }
                // do mutation
                for (int i = 0; i < populationSize; i++)
                {
                    if (new Random().NextDouble() < mutationProbabilty)
                    {
                        Mutate(newPopulation[i]);
                    }
                }

                population = newPopulation;
                // find the best solution in the population
                List<Place> bestRoute = population.OrderBy(route => CalculateRoutePrice(route)).First();
                float bestRoutePrice = CalculateRoutePrice(bestRoute);
                if (bestRoutePrice < currentBestRoutePrice)
                {
                    currentBestRoute = bestRoute;
                    currentBestRoutePrice = bestRoutePrice;
                }
                //Console.WriteLine("currentBest: " + CalculateRoutePrice(currentBestRoute));
            }

            // the last travel should go from the last visited place to the starting point
            currentBestRoute.Add(currentBestRoute[0]);


            float maxX = -1, maxY = -1;
            float originX = float.PositiveInfinity, originY = float.PositiveInfinity;
            getWorldSizes(currentBestRoute, ref maxX, ref maxY, ref originX, ref originY);
            DrawGraph(fileName, currentBestRoute, originX, originY, maxX, maxY);
            return currentBestRoute;
        }

        public static List<Place> TournamentSelection(List<List<Place>> population, int tournamentSize)
        {
            Random rng = new Random();
            List<List<Place>> tournament = new List<List<Place>>();
            for (int i = 0; i < tournamentSize; i++)
            {
                int randomIndex = rng.Next(population.Count);
                tournament.Add(population[randomIndex]);

            }

            // return thr best individual according to the fitness function
            return tournament.OrderBy(route => CalculateRoutePrice(route)).First();
        }

        public static List<Place> Crossover(List<Place> parent1, List<Place> parent2)
        {
            Random rng = new Random();
            int crossoverPoint = rng.Next(parent1.Count);

            List<Place> child = new List<Place>();
            for (int i = 0; i < crossoverPoint; i++)
                child.Add(parent1[i]);

            for (int i = crossoverPoint; i < parent2.Count; i++)
                if (!child.Contains(parent2[i]))
                    child.Add(parent2[i]);

            // if the child is missing some places, add them at the end
            foreach (Place place in parent1)
                if (!child.Contains(place))
                    child.Add(place);
            return child;
        }

        public static void Mutate(List<Place> route)
        {
            Random rng = new Random();
            for (int i = 0; i < route.Count; i++)
            {
                int j = rng.Next(route.Count);
                Place temp = route[i];
                route[i] = route[j];
                route[j] = temp;
            }
        }

        public static void Shuffle<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }



        public static void DrawGraph(string fileName, List<Place> solution, float originX, float originY, float maxX, float maxY)
        {
            int imageWidth = 5000;
            int imageHeight = 5000;

            float scaleX = imageWidth / maxX;
            float scaleY = imageHeight / maxY;
            Bitmap image = new Bitmap(imageWidth, imageHeight);
            Graphics g = Graphics.FromImage(image);
            g.ScaleTransform(scaleX * 1.75f, scaleY * 20);

            // fill white background
            g.Clear(Color.White);

            Pen pen = new Pen(Color.Black, 20);

            for (int i = 0; i < solution.Count - 1; i++)
            {
                //PointF from = new PointF(((float)solution[i].X - originX) * scaleX, ((float)solution[i].Y - originY) * scaleY);
                //PointF to = new PointF(((float)solution[i + 1].X - originX) * scaleX, ((float)solution[i + 1].Y - originY) * scaleY);
                PointF from = new PointF(solution[i].X - originX, solution[i].Y - originY);
                PointF to = new PointF(solution[i + 1].X - originX, solution[i + 1].Y - originY);
                g.DrawLine(pen, from, to);
                if (i == 0)
                {
                    int size = 1500;
                    g.FillEllipse(Brushes.Blue, from.X - size / 2, from.Y - size / 2, size, size);
                }
            }

            Font font = new Font("Arial", 2000, FontStyle.Regular);
            // draw place names
            for (int i = 0; i < solution.Count; i++)
            {
                PointF textPoint = new PointF(solution[i].X - originX, solution[i].Y - originY);
                g.DrawString(solution[i].Name, font, Brushes.Red, textPoint);
            }

            g.DrawImage(image, 0, 0);

            pen.Dispose();
            g.Dispose();

            image.Save(fileName);
            image.Dispose();

        }
        public static void getWorldSizes(List<Place> places, ref float maxX, ref float maxY,
         ref float originX, ref float originY)
        {
            foreach (var place in places)
            {
                if (place.X > maxX)
                    maxX = place.X;
                if (place.Y > maxY)
                    maxY = place.Y;
                if (place.X < originX)
                    originX = place.X;
                if (place.Y < originY)
                    originY = place.Y;
            }
        }
    }
}
