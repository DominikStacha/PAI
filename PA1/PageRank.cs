using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PA1
{
    public class PageRank
    {
        public Dictionary<string, ISet<string>> Graph { get; }

        public PageRank(string graphFile)
        {
            string path = Path.GetFullPath(graphFile);
            Graph = this.ImportGraph(path);
        }

        private Dictionary<string, ISet<string>> GetPointingDict(Dictionary<string, ISet<string>> graph)
        {
            Dictionary<string, ISet<string>> pointingDict = new Dictionary<string, ISet<string>>();
            foreach (var e1 in graph)
            {
                pointingDict[e1.Key] = new HashSet<string>();
                foreach (var e2 in graph)
                {
                    if (e1.Key == e2.Key)
                    {
                        continue;
                    }

                    if (e2.Value.Any(edge => edge == e1.Key))
                    {
                        pointingDict[e1.Key].Add(e2.Key);
                    }
                }
            }

            return pointingDict;
        }

        public Dictionary<string, ISet<string>> ImportGraph(string path, char splitCharacter = ';')
        {
            Dictionary<string, ISet<string>> graph = new Dictionary<string, ISet<string>>();

            foreach (var row in File.ReadAllLines(path, Encoding.Default))
            {
                var nodes = row.Split(splitCharacter);
                graph[nodes[0]] = new HashSet<string>();
                for (int i = 1; i < nodes.Length; i++)
                {
                    graph[nodes[0]].Add(nodes[i]);
                }
            }

            return graph;
        }

        public Dictionary<string, decimal> Solve(int n = 4, decimal d = 0.85M)
        {
            bool converged = false;
            Dictionary<string, ISet<string>> pointingDict = GetPointingDict(Graph);
            ConcurrentDictionary<string, decimal> pageRankDict = new ConcurrentDictionary<string, decimal>();
            var nodeArray = Graph.Keys.ToArray();

            foreach (string key in Graph.Keys)
            {
                pageRankDict[key] = 1.0M / Graph.Count;
            }

            ConcurrentDictionary<string, decimal> pageRankDictLast = pageRankDict;

            List<Action> actions = new List<Action>();

            while (!converged)
            {
                pageRankDictLast = pageRankDict;
                pageRankDict = new ConcurrentDictionary<string, decimal>(pageRankDictLast);

                var threads = actions.Select(action => new Thread(() => action())).ToArray();

                Parallel.For(0, n, i =>
                {
                    int range = Graph.Count / 4;
                    int startIndex = i * range;
                    int endIndex = (i + 1) < n ? (i + 1) * range : Graph.Count;

                    for (int j = startIndex; j < endIndex; ++j)
                    {
                        string node = nodeArray[j];
                        decimal newPageRank = pointingDict[node].Aggregate(
                            0.0M,
                            (accumulator, node) => accumulator + pageRankDictLast[node] / Graph[node].Count
                        );

                        newPageRank = (1.0M - d) + d * newPageRank;
                        pageRankDict[node] = newPageRank;
                    }
                });

                foreach (var thread in threads)
                {
                    thread.Start();
                }

                foreach (var thread in threads)
                {
                    thread.Join();
                }
                converged = pageRankDict.All((entry) => entry.Value == pageRankDictLast[entry.Key]);
            }

            decimal sum = pageRankDict.Values.Sum();
            Dictionary<string, decimal> result = pageRankDict.ToDictionary(entry => entry.Key, entry => entry.Value / sum);
            return result;
        }

        public Dictionary<string, decimal> SolveSingleThread(decimal d = 0.85M)
        {
            bool converged = false;
            Dictionary<string, ISet<string>> pointingDict = GetPointingDict(Graph);
            Dictionary<string, decimal> pageRankDict = new Dictionary<string, decimal>();
            foreach (string key in Graph.Keys)
            {
                pageRankDict[key] = 1.0M / Graph.Count;
            }

            while (!converged)
            {
                Dictionary<string, decimal> pageRankDictLast =
                    pageRankDict.ToDictionary(entry => entry.Key, entry => entry.Value);

                foreach (var entry in Graph)
                {
                    decimal newPageRank = pointingDict[entry.Key].Aggregate(
                        0.0M,
                        (accumulator, node) => accumulator + pageRankDictLast[node] / Graph[node].Count
                    );

                    newPageRank = (1.0M - d) + d * newPageRank;
                    pageRankDict[entry.Key] = newPageRank;
                }

                converged = pageRankDict.All((entry) => entry.Value == pageRankDictLast[entry.Key]);
            }

            decimal sum = pageRankDict.Values.Sum();
            pageRankDict = pageRankDict.ToDictionary(entry => entry.Key, entry => entry.Value / sum);
            return pageRankDict;
        }
    }
}