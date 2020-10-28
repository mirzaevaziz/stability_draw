using System.Collections.Generic;
using System.IO;
using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Methods
{
    class StandartObjectsFinder
    {
        public static HashSet<int> Find(ObjectSet set,
                                           IEnumerable<HashSet<int>> groups,
                                           IEnumerable<Sphere> spheres,
                                           HashSet<int> excludedObjects,
                                           decimal[,] distances)
        {
            var objects = groups.SelectMany(s => s).ToHashSet<int>().Except(excludedObjects);
            var standartObjects = spheres.Where(w => objects.Contains(w.ObjectIndex.Value))
                                        .OrderBy(o => o.Radius)
                                        .Select(s => new
                                        {
                                            ObjectIndex = s.ObjectIndex.Value,
                                            Radius = s.Radius,
                                        }).ToList();
            // System.Console.WriteLine("Finding standart objects...");
            foreach (var group in groups.OrderByDescending(o => o.Count))
            {
                var candidates = standartObjects.Where(w => group.Contains(w.ObjectIndex))
                                                .OrderBy(o => o.Radius).ToArray();

                foreach (var candidate in candidates)
                {
                    // System.Console.WriteLine($"Candidate for deleting is {candidate:000}");
                    standartObjects.Remove(candidate);
                    foreach (var obj in objects)
                    {
                        decimal? minDistance = null;
                        bool isWrongRecognition = false;
                        foreach (var st in standartObjects)
                        {
                            var dist = distances[obj, st.ObjectIndex] / st.Radius;
                            if (!minDistance.HasValue || minDistance > dist)
                            {
                                minDistance = dist;
                                isWrongRecognition = set.Objects[st.ObjectIndex][set.ClassFeatureIndex] != set.Objects[obj][set.ClassFeatureIndex];
                            }
                            else if (dist == minDistance)
                            {
                                isWrongRecognition = isWrongRecognition || set.Objects[st.ObjectIndex][set.ClassFeatureIndex] != set.Objects[obj][set.ClassFeatureIndex];
                            }
                        }

                        if (isWrongRecognition)
                        {
                            standartObjects.Add(candidate);
                            // System.Console.WriteLine($"Candidate for deleting {candidate:000} NOT DELETED");
                            break;
                        }
                    }
                }
            }
            return standartObjects.Select(s => s.ObjectIndex).ToHashSet<int>();
        }
    }
}