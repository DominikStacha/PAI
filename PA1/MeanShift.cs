using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA1
{
    public class MeanShift
    {
        public IList<double[]> Data { get; }

        public MeanShift(string dataFile)
        {
            string path = Path.GetFullPath(dataFile);
            Data = ImportData(path);
        }

        private IList<double[]> ImportData(string path, char splitCharacter = ';')
        {
            string[] lines = File.ReadAllLines(path, Encoding.Default);
            if (lines.Length == 0) return null;
            int rowCount = lines.Length;
            int columnCount = lines[0].Split(splitCharacter).Length;
            IList<double[]> data = new List<double[]>();

            foreach (var row in lines)
            {
                data.Add(row.Split(splitCharacter).Select(double.Parse).ToArray());
            }

            return data;
        }

        public IList<double[]> Solve(int iterationCount = 1000, double bandwidth = 5)
        {
            ConcurrentDictionary<int, double[]> centroids = new ConcurrentDictionary<int, double[]>();
            for (int i = 0; i < Data.Count; i++)
            {
                centroids[i] = Data[i];
            }

            int iteration = 0;
            while (iteration < iterationCount)
            {
                ConcurrentDictionary<int, double[]> newCentroids = new ConcurrentDictionary<int, double[]>(centroids);

                Parallel.For(0, centroids.Count, i =>
                {
                    double[] centroid = centroids[i];
                    IList<int> indexes = new List<int>();
                    IList<double[]> neighbors = new List<double[]>();

                    for (int j = 0; j < Data.Count; j++)
                    {
                        double[] sample = Data[j];
                        if (EuclideanDistance(centroid, sample) < bandwidth)
                        {
                            neighbors.Add(sample);
                            indexes.Add(j);
                        }

                        if (neighbors.Count == 0) continue;
                        double[] newCentroid = Average(neighbors);
                        for (int k = 0; k < neighbors.Count; k++)
                        {
                            newCentroids[indexes[k]] = newCentroid;
                        }
                    }
                });


                centroids = newCentroids;
                iteration++;
            }


            IList<double[]> result = GetUnique(centroids.Values.ToList());
            return result;
        }

        public IList<double[]> SolveSingleThread(int iterationCount = 1000, double bandwidth = 5)
        {
            IList<double[]> centroids = Data;
            int iteration = 0;

            while (iteration < iterationCount)
            {
                IList<double[]> newCentroids = centroids.Clone();

                for (int i = 0; i < centroids.Count; i++)
                {
                    double[] centroid = centroids[i];
                    IList<int> indexes = new List<int>();
                    IList<double[]> neighbors = new List<double[]>();

                    for (int j = 0; j < Data.Count; j++)
                    {
                        double[] sample = Data[j];
                        if (EuclideanDistance(centroid, sample) < bandwidth)
                        {
                            neighbors.Add(sample);
                            indexes.Add(j);
                        }

                        if(neighbors.Count == 0) continue;
                        double[] newCentroid = Average(neighbors);
                        for (int k = 0; k < neighbors.Count; k++)
                        {
                            newCentroids[indexes[k]] = newCentroid;
                        }
                    }
                }

                centroids = newCentroids;
                iteration++;
            }

            centroids = GetUnique(centroids);
            return centroids;
        }

        private double EuclideanDistance(double[] a, double[] b)
        {
            if (a.Length != b.Length)
            {
                throw new Exception("Vectors doesn't have same dimension.");
            }
            double distance = 0;
            for (int dim = 0; dim < a.Length; dim++)
            {
                distance += Math.Pow(a[dim] - b[dim], 2.0);
            }

            return Math.Sqrt(distance);
        }

        private double[] Average(IList<double[]> points)
        {
            if (points.Count == 0) return null;
            double[] sum = new double[points[0].Length];

            foreach (double[] point in points)
            {
                for (int i = 0; i < point.Length; i++)
                {
                    sum[i] += point[i];
                }
            }

            for (int i = 0; i < sum.Length; i++)
            {
                sum[i] = sum[i] / points.Count;
            }

            return sum;
        }

        private IList<double[]> GetUnique(IList<double[]> points)
        {
            IList<double[]> uniquePoints = new List<double[]>();

            foreach (var point in points)
            {
                if (!uniquePoints.Any(u => u.SequenceEqual(point)))
                {
                    uniquePoints.Add(point);
                }
            }

            return uniquePoints;
        }
    }
}
