using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Project_2
{
    class Program
    {
        static List<Product> dataset = new List<Product>();
        static List<Product> heapSorted = new List<Product>();
        static List<Product> quickSorted = new List<Product>();

        static void Main(string[] args)     //Main Method.
        {
            while (true)                    //Basic while loop for menu, standard practice.
            {                               //Notice How I dont have to remember which side of the screen the arrow goes to for output and input 🙂 lmao.
                Console.WriteLine("\n=== Sorting Algorithm Comparison Program ==="); 
                Console.WriteLine("1. Generate dataset");
                Console.WriteLine("2. Run Heap Sort");
                Console.WriteLine("3. Run Quick Sort");
                Console.WriteLine("4. Compare algorithm performance");
                Console.WriteLine("5. Display sorted results");
                Console.WriteLine("6. Exit");
                Console.Write("Select option: ");
                string choice = Console.ReadLine();

                switch (choice)             //Standard switch case with default for input verification in C#
                {
                    case "1": GenerateDatasetMenu(); break;
                    case "2": RunHeapSort(); break;
                    case "3": RunQuickSort(); break;
                    case "4": PerformanceComarison(); break;
                    case "5": DisplayResults(); break;
                    case "6":
                        {
                            Console.WriteLine("Thank you for using this program");
                            return;
                        }
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }


        static void GenerateDatasetMenu()   //Dataset generation menu
        {
            Console.WriteLine("\nDataset Generation:");
            Console.WriteLine("1. 100 products");
            Console.WriteLine("2. 1,000 products");
            Console.WriteLine("3. 10,000 products");
            Console.WriteLine("4. 100,000 products");
            Console.WriteLine("5. 500,000 products");
            Console.WriteLine("6. 1,000,000 products");
            Console.WriteLine("7. 100,000,000 products (Good luck lmao)");   //Im ngl, this is for shits and giggles
            Console.Write("Select size: ");

            int size = Console.ReadLine() switch    //Another switch case, but this one has to map the input to the dataset size.
            {
                "1" => 100,
                "2" => 1000,
                "3" => 10000,
                "4" => 100000,
                "5" => 500000,
                "6" => 1000000,
                "7" => 100000000,
                _ => 0
            };

            if (size > 0)                           //Input Verification
            {
                dataset = GenerateDataset(size);
                Console.WriteLine($"Dataset with {size} products generated."); //Dollarsign allows for string interpolation
            }
            else
                Console.WriteLine("Invalid selection.");
        }

        static List<Product> GenerateDataset(int size)      //Dataset generation function. Uses Random class, which is a standard C# library for generating random numbers.
        {                                                   //Var is just a way to let the compiler choose the type of variable.
            Random rand = new Random();
            var list = new List<Product>();

            for (int i = 0; i < size; i++)
            {
                list.Add(new Product                        //Formatted this way so that the code line isnt like 67 words long or something
                {
                    ID = rand.Next(1, 1000000),
                    Quantity = rand.Next(1, 1000),
                    Price = rand.NextDouble() * 1000        //This is cause the double generated is from 0 to 1
                });
            }

            return list;
        }


        static SortMetrics RunHeapSort()  //Formatted these functions so that it returns the metric object, while still running the sort on the dataset copy.
        {
            if (dataset.Count == 0)
            {
                Console.WriteLine("Dataset Not Yet Generated For Heap Sort.");      //Ensures dataset is generated
                return null;
            }

            heapSorted = new List<Product>(dataset);
            SortMetrics metrics = new SortMetrics();    //Metric object creation

            Stopwatch Stopwatch = Stopwatch.StartNew();        //C# timer feature
            HeapSort(heapSorted, metrics);
            Stopwatch.Stop();

            metrics.ExecutionTime = Stopwatch.Elapsed.TotalMilliseconds;

            Console.WriteLine("\nHeap Sort Completed.");
            PrintMetrics(metrics);

            return metrics;
        }

        static void HeapSort(List<Product> arr, SortMetrics metric) //Standard Heap sort implementation, i= index, n = size of heap, metric = metric object
        {                                                           //Notice the metric object gets passed to all helper functions. This is to track swaps and comparisons
                                                                    //Ngl, took longer than I'd like to admit to find out I had to do this 💀
            int n = arr.Count;

            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(arr, n, i, metric);

            for (int i = n - 1; i > 0; i--)
            {
                Swap(arr, 0, i, metric);
                Heapify(arr, i, 0, metric);
            }
        }

        static void Heapify(List<Product> arr, int n, int i, SortMetrics metric)    //Standard heapify helper
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < n && Compare(arr[left], arr[largest], metric) > 0)
                largest = left;

            if (right < n && Compare(arr[right], arr[largest], metric) > 0)
                largest = right;

            if (largest != i)
            {
                Swap(arr, i, largest, metric);
                Heapify(arr, n, largest, metric);
            }
        }


        static SortMetrics RunQuickSort() //Same as before, returns the metric object, while still running the sort on the dataset copy.
        {
            if (dataset.Count == 0)
            {
                Console.WriteLine("Dataset Not Yet Generated For Quick Sort");      //Ensures dataset is generated
                return null;
            }

            quickSorted = new List<Product>(dataset);
            SortMetrics metrics = new SortMetrics();        //Metric object

            Stopwatch Stopwatch = Stopwatch.StartNew();
            QuickSort(quickSorted, 0, quickSorted.Count - 1, metrics);
            Stopwatch.Stop();

            metrics.ExecutionTime = Stopwatch.Elapsed.TotalMilliseconds;

            Console.WriteLine("\nQuick Sort Completed.");
            PrintMetrics(metrics);

            return metrics;
        }

        static void QuickSort(List<Product> arr, int low, int high, SortMetrics metrics)  //Standard Quick Sort for C#
        {
            if (low < high)
            {
                int pivot = PivotSeperate(arr, low, high, metrics);
                QuickSort(arr, low, pivot - 1, metrics);
                QuickSort(arr, pivot + 1, high, metrics);
            }
        }

        static int PivotSeperate(List<Product> arr, int low, int high, SortMetrics metrics)     //standard pivot point seperation
        {                                                                                       //Does take the median of three pivots, common practice
            int mid = (low + high) / 2;
            int pivotIndex = MedianOfThreePivots(arr, low, mid, high, metrics);
            Swap(arr, pivotIndex, high, metrics);

            Product pivot = arr[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (Compare(arr[j], pivot, metrics) < 0)
                {
                    i++;
                    Swap(arr, i, j, metrics);
                }
            }

            Swap(arr, i + 1, high, metrics);
            return i + 1;
        }

        static int MedianOfThreePivots(List<Product> arr, int a, int b, int c, SortMetrics metrics)     //pivot point median helper
        {
            if (Compare(arr[a], arr[b], metrics) < 0)
            {
                if (Compare(arr[b], arr[c], metrics) < 0) return b;
                else if (Compare(arr[a], arr[c], metrics) < 0) return c;
                else return a;
            }
            else
            {
                if (Compare(arr[a], arr[c], metrics) < 0) return a;
                else if (Compare(arr[b], arr[c], metrics) < 0) return c;
                else return b;
            }
        }


        static void PerformanceComarison()      //Function to compare the performance of the two algorithms.
        {                                       //I do have it set to run the sorts again, incase you misclicked
                                                //Though this could also prevent someone running quick sort on a 100 dataset and heap on a 500k dataset and comparing those results.
            var heapMetrics = RunHeapSort();
            var quickMetrics = RunQuickSort();

            if (heapMetrics == null || quickMetrics == null)    //Makes sure both algos were run. Though this could only happen if dateset was not generated.
            {                                                   //Kind of redundant, cause I already have this in the sort functions but oh well 😐 better safe than sorry tbh
                Console.WriteLine("Cannot compare algorithms, Generate Dataset first");
                return;
            }

            Console.WriteLine("\n=== Algorithm Comparison ===");
            Console.WriteLine("Heap Sort Metrics:");
            PrintMetrics(heapMetrics);

            Console.WriteLine("\nQuick Sort Metrics:");
            PrintMetrics(quickMetrics);
        }


        static void DisplayResults()        //Function to display sorted results. 
        {                                   //Simple menu to choose which
            Console.WriteLine("\n1. Heap Sorted");
            Console.WriteLine("2. Quick Sorted");
            Console.WriteLine("Choice: ");
            string choice = Console.ReadLine();

            var list = choice == "1" ? heapSorted : quickSorted;
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var p in list.GetRange(0, list.Count))     //YES this will print every single product 😈
                                                                //YES this is O(n) time just for a display function
                                                                //YES im kinda crazy lol
                                                                //You know what im gonna add a timer to this for fun
            {                                                   //Good luck on the 100M dataset LMAO 💀
                Console.WriteLine(p);
            }
            stopwatch.Stop();
            Console.WriteLine($"Displayed {list.Count} products in {stopwatch.Elapsed.TotalSeconds:F4} seconds :) ");
        }


        static int Compare(Product a, Product b, SortMetrics metric)    //Helper function to compare based on ID, and increments metric
        {
            metric.Comparisons++;
            return a.ID.CompareTo(b.ID);
        }

        static void Swap(List<Product> arr, int i, int j, SortMetrics metric)   //Same as before, but with swaps
        {
            metric.Swaps++;
            var temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        static void PrintMetrics(SortMetrics metric)    //Probably a helper function to print the metrics. Just a hunch though, not too sure in all honesty, could very well just be some arbitrary code fr. Machinations of my mind truly are an enigma.
        {
            Console.WriteLine($"Execution Time: {metric.ExecutionTime:F4} milliseconds");  //The Dollarsign allows string interpolation
            Console.WriteLine($"Comparisons: {metric.Comparisons}");                       //The F4 rounds to 4 decimal places
            Console.WriteLine($"Swaps: {metric.Swaps}");
        }
    }


    //Classes for Products to be stored in the ArrayList and for a Metric object to store performance of each Algorithm

    class Product
    {
        public int ID { get; set; }
        public int Quantity { get; set; }     //Basic Getters and Setters with C# lmao C++ could never
        public double Price { get; set; }

        public override string ToString()    //To String ovverride. Standard for displaying object info.
        {
            return $"ID: {ID}, Qty: {Quantity}, Price: {Price:F2}";
        }
    }

    class SortMetrics
    {
        public double ExecutionTime { get; set; }
        public long Comparisons { get; set; }   //Once agin, C++ could never lol
        public long Swaps { get; set; }
    }
}
