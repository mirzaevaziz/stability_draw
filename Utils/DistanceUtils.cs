using System.Collections.Generic;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Utils
{
    class DistanceUtils
    {
        public static decimal[,] FindAllDistance(ObjectSet set, Models.MetricCalculateFunctionDelegate distFunc)
        {
            decimal[,] dist = new decimal[set.Objects.Count, set.Objects.Count];

            for (int i = 0; i < set.Objects.Count; i++)
            {
                dist[i, i] = 0M;
                // System.Console.WriteLine($"Finding distance for object {i}");
                for (int j = i + 1; j < set.Objects.Count; j++)
                {
                    dist[i, j] = distFunc(set.Objects[i], set.Objects[j], set.Features);
                    dist[j, i] = dist[i, j];
                }
            }

            return dist;
        }

        public static decimal[,] FindAllDistance(ObjectSet set, Models.MetricCalculateFunctionDelegate distFunc, HashSet<int> excludedObjects)
        {
            if (excludedObjects.Count == 0)
                return FindAllDistance(set, distFunc);

            decimal[,] dist = new decimal[set.Objects.Count, set.Objects.Count];

            for (int i = 0; i < set.Objects.Count; i++)
            {
                if (excludedObjects.Contains(i))
                {
                    for (int j = 0; j < set.Objects.Count; j++)
                    {
                        dist[i, j] = -1;
                        dist[j, i] = -1;
                    }
                }
                else
                {
                    dist[i, i] = 0M;
                    // System.Console.WriteLine($"Finding distance for object {i}");
                    for (int j = i + 1; j < set.Objects.Count; j++)
                    {
                        if (excludedObjects.Contains(j))
                            continue;

                        dist[i, j] = distFunc(set.Objects[i], set.Objects[j], set.Features);
                        dist[j, i] = dist[i, j];
                    }
                }
            }

            return dist;
        }
    }
}