using System;
using System.Collections.Generic;
using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Methods
{
    class NoisyObjectsFinder
    {
        public static HashSet<int> Find(
            ObjectSet set,
            IEnumerable<Sphere> spheres,
            decimal[,] distances,
            HashSet<int> excludedObjects, System.IO.StreamWriter log)
        {
            var result = new HashSet<int>();

            var candidates = spheres.SelectMany(s => s.Enemies).Distinct();

            foreach (var candidate in candidates)
            {
                var radius = spheres.Where(w => !excludedObjects.Contains(w.ObjectIndex.Value) && w.Enemies.Contains(candidate)).Max(m => m.Radius);



                var relativeCount = 0;
                var enemyCount = 0;

                for (int i = 0; i < Math.Sqrt(distances.Length); i++)
                {
                    if (distances[candidate, i] < radius)
                    {
                        if (set.Objects[candidate][set.ClassFeatureIndex] == set.Objects[i][set.ClassFeatureIndex])
                            relativeCount++;
                        else
                            enemyCount++;
                    }
                }

                

                // if (relativeCount == 0)
                // {
                //     log.WriteLine(spheres.First(w => w.ObjectIndex == candidate));
                // }
                // var enemyCount = spheres.Count(w => !excludedObjects.Contains(w.ObjectIndex.Value) && w.Enemies.Contains(candidate));

                decimal classCount = set.Objects.Count(w => w[set.ClassFeatureIndex] == set.Objects[candidate][set.ClassFeatureIndex] && !excludedObjects.Contains(w.Index));

                decimal nonclassCount = set.Objects.Count(w => w[set.ClassFeatureIndex] != set.Objects[candidate][set.ClassFeatureIndex] && !excludedObjects.Contains(w.Index));

                if (enemyCount / nonclassCount >= relativeCount / classCount)
                {
                    log.WriteLine($@"Removed noisy object {candidate}: EnemyCount={enemyCount}
                    RelativeCount={relativeCount}
                    ClassObjectsCount={classCount}
                    NonClassObjectsCount={nonclassCount}
                    ");
                    result.Add(candidate);
                }
            }

            return result;
        }
    }
}