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
                var relativeCount = spheres.First(w => w.ObjectIndex == candidate).Relatives.Where(w => !excludedObjects.Contains(w)).Count();
                if (relativeCount == 0){
                    log.WriteLine(spheres.First(w => w.ObjectIndex == candidate));
                }
                var enemyCount = spheres.Count(w => !excludedObjects.Contains(w.ObjectIndex.Value) && w.Enemies.Contains(candidate));

                var classCount = set.Objects.Count(w => w[set.ClassFeatureIndex] == set.Objects[candidate][set.ClassFeatureIndex] && !excludedObjects.Contains(w.Index));

                var nonclassCount = set.Objects.Count(w => w[set.ClassFeatureIndex] != set.Objects[candidate][set.ClassFeatureIndex] && !excludedObjects.Contains(w.Index));
                if (enemyCount / (decimal)nonclassCount >= relativeCount / (decimal)classCount)
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