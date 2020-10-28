using System.Collections.Generic;
using System.Linq;
using FinderOfStandarts.Metrics;

namespace FinderOfStandarts.Models
{
    class Sphere
    {
        public int? ObjectIndex { get; set; }
        public decimal? Radius { get; set; }
        public HashSet<int> Relatives { get; set; }
        public HashSet<int> Enemies { get; set; }
        public HashSet<int> Coverage { get; private set; }

        public Sphere()
        {
            Relatives = new HashSet<int>();
            Enemies = new HashSet<int>();
            Coverage = new HashSet<int>();
        }

        public override string ToString()
        {
            return $@"Sphere {ObjectIndex}: radius = {Radius}
               relatives = ({Relatives.Count}) {{{string.Join(", ", Relatives.OrderBy(o => o))}}}
               enemies= ({Enemies.Count}) {{{string.Join(", ", Enemies.OrderBy(o => o))}}}
               coverage= ({Coverage.Count}) {{{string.Join(", ", Coverage.OrderBy(o => o))}}}";
        }

        public static IEnumerable<Sphere> FindAll(ObjectSet set, decimal[,] dist, HashSet<int> excludedObjects, bool ShouldFindCoverage = true)
        {
            var result = new List<Sphere>();

            for (int i = 0; i < set.Objects.Count; i++)
            {
                if (excludedObjects?.Contains(i) == true)
                    continue;

                var sphere = new Sphere()
                {
                    ObjectIndex = i
                };
                bool isRadiusFound = false;

                for (int j = 0; j < set.Objects.Count; j++)
                {
                    if (excludedObjects?.Contains(j) == true)
                        continue;

                    if (set.Objects[i][set.ClassFeatureIndex] != set.Objects[j][set.ClassFeatureIndex] && (!isRadiusFound || sphere.Radius >= dist[i, j]))
                    {
                        isRadiusFound = true;
                        if (sphere.Radius != dist[i, j])
                        {
                            sphere.Enemies.Clear();
                        }
                        sphere.Radius = dist[i, j];
                        sphere.Enemies.Add(j);
                    }
                }

                for (int j = 0; j < set.Objects.Count; j++)
                {
                    if (excludedObjects?.Contains(j) == true)
                        continue;

                    if (set.Objects[i][set.ClassFeatureIndex] == set.Objects[j][set.ClassFeatureIndex] && sphere.Radius > dist[i, j])
                    {
                        sphere.Relatives.Add(j);
                    }
                }
                result.Add(sphere);
            }

            if (ShouldFindCoverage)
                foreach (var sphere in result)
                {
                    foreach (var enemyIndex in sphere.Enemies)
                    {
                        bool isRadiusFound = false;
                        decimal radius = 0M;

                        for (int j = 0; j < set.Objects.Count; j++)
                        {
                            if (excludedObjects?.Contains(j) == true || !sphere.Relatives.Contains(j))
                                continue;

                            if (set.Objects[enemyIndex][set.ClassFeatureIndex] != set.Objects[j][set.ClassFeatureIndex] && (!isRadiusFound || radius >= dist[enemyIndex, j]))
                            {
                                isRadiusFound = true;
                                if (radius != dist[enemyIndex, j])
                                {
                                    sphere.Coverage.Clear();
                                }
                                radius = dist[enemyIndex, j];
                                sphere.Coverage.Add(j);
                            }
                        }
                    }
                }

            return result;
        }
    }
}