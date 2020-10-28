using System;
using System.Collections.Generic;
using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Methods
{
    class FindFirstPairFeatureForObjectSet
    {
        public static Tuple<int, int> Find(ObjectSet set, Models.MetricCalculateFunctionDelegate distFunc, System.IO.StreamWriter log)
        {
            if (set is null)
            {
                throw new System.ArgumentNullException(nameof(set));
            }

            if (distFunc is null)
            {
                throw new System.ArgumentNullException(nameof(distFunc));
            }

            log.WriteLine("====FindFirstPairFeatureForObjectSet BEGIN====");

            List<int> deactivatedFeatures = new List<int>();
            for (int i = 0; i < set.Features.Count; i++)
            {
                if (!set.Features[i].IsActive)
                    deactivatedFeatures.Add(i);
            }

            set.Features.ForEach(i => i.IsActive = false);

            int? maxRelativesSum = null;
            Tuple<int, int> result = null;
            for (int i = 0; i < set.Features.Count - 1; i++)
            {
                if (set.Features[i].IsClass || deactivatedFeatures.Contains(i))
                    continue;

                set.Features[i].IsActive = true;
                for (int j = i + 1; j < set.Features.Count; j++)
                {
                    if (set.Features[j].IsClass || deactivatedFeatures.Contains(j))
                        continue;

                    set.Features[j].IsActive = true;

                    log.WriteLine($"Finding feature max of [{i},{j}]");

                    decimal[,] dist = Utils.DistanceUtils.FindAllDistance(set, distFunc);

                    var spheres = Sphere.FindAll(set, dist, new HashSet<int>(), false);
                    // if (!spheres.Any(w => w.Relatives.Count == 0))
                    {
                        System.IO.File.WriteAllText("out.txt", string.Join("\n", spheres.Where(w => w.Relatives.Count > 0)));
                        var relativesSum = spheres.Sum(s => s.Relatives.Count);
                        if (!maxRelativesSum.HasValue || maxRelativesSum < relativesSum)
                        {
                            maxRelativesSum = relativesSum;
                            result = new Tuple<int, int>(i, j);
                        }
                        log.WriteLine($"\t\tcurrent = {relativesSum}, max = {maxRelativesSum}");
                    }
                    set.Features[j].IsActive = false;
                }
                set.Features[i].IsActive = false;
            }

            for (int i = 0; i < set.Features.Count; i++)
            {
                if (deactivatedFeatures.Contains(i))
                    continue;

                set.Features[i].IsActive = true;
            }
            log.WriteLine($"First pair is: {result}");
            log.WriteLine("====FindFirstPairFeatureForObjectSet END====");

            return result;
        }
    }
}