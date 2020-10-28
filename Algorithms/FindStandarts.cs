using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinderOfStandarts.Methods;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Algorithms
{
    public class FindStandartsResult
    {
        public HashSet<int> Standarts { get; set; }
        public List<HashSet<int>> Groups { get; set; }
        public HashSet<int> ExcludedObjects { get; internal set; }
        public decimal Stability { get; internal set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Standart finding result:\n");

            if (Groups != null)
            {
                sb.AppendLine($"\tGroups ({Groups.Count}):");
                foreach (var group in Groups)
                {
                    sb.AppendLine($"\t\t {{ {string.Join(", ", group.OrderBy(o => o))} }}");
                }
            }

            if (Standarts != null)
            {
                sb.AppendLine($"\tStandarts ({Standarts.Count}):{{ {string.Join(", ", Standarts.OrderBy(o => o))} }}");
            }

            sb.AppendLine($"\tStability = {Stability}");

            return sb.ToString();
        }
    }
    class FindStandarts
    {
        public static FindStandartsResult Find(ObjectSet set, Models.MetricCalculateFunctionDelegate distFunc, System.IO.StreamWriter log)
        {
            log.WriteLine("====FindStandarts BEGIN====");

            // System.Console.WriteLine($"Enabled features: {string.Join(", ", set.Features.Where(w => w.IsActive))}");

            decimal[,] dist = Utils.DistanceUtils.FindAllDistance(set, distFunc);

            // for (int i = 0; i < set.Objects.Count; i++)
            // {
            //     System.Console.WriteLine($"{dist[0, i]:0.00000}");
            // }

            var excludedObjects = new HashSet<int>();

            IEnumerable<Sphere> spheres;
            spheres = Sphere.FindAll(set, dist, excludedObjects);
            log.WriteLine($"Enemy objects {string.Join(", ", spheres.SelectMany(s => s.Enemies).Distinct().OrderBy(o => o))}");
            log.WriteLine($"Coverage objects {string.Join(", ", spheres.SelectMany(s => s.Coverage).Distinct().OrderBy(o => o))}");
            var noisyObjects = NoisyObjectsFinder.Find(set, spheres, dist, excludedObjects, log);
            excludedObjects.UnionWith(noisyObjects);
            spheres = Sphere.FindAll(set, dist, excludedObjects);

            foreach (var item in spheres)
            {
                log.WriteLine(item);
            }

            log.WriteLine($"Enemy objects {string.Join(", ", spheres.SelectMany(s => s.Enemies).Distinct().OrderBy(o => o))}");
            log.WriteLine($"Coverage objects {string.Join(", ", spheres.SelectMany(s => s.Coverage).Distinct().OrderBy(o => o))}");

            log.WriteLine($"Removed {excludedObjects.Count} {{{string.Join(", ", excludedObjects.OrderBy(o => o))}}} noisy objects");

            var result = new FindStandartsResult();
            result.Groups = Methods.AcquaintanceGrouping.Find(set, spheres, excludedObjects);

            log.WriteLine($"Groups count = {result.Groups.Count}");
            foreach (var group in result.Groups)
            {
                log.WriteLine($"Group: {{{string.Join(", ", group.OrderBy(o => o))}}}");
            }
            result.ExcludedObjects = excludedObjects;
            result.Standarts = Methods.StandartObjectsFinder.Find(set, result.Groups, spheres, excludedObjects, dist);

            log.WriteLine($"Standarts ({result.Standarts.Count}): {{{string.Join(", ", result.Standarts.OrderBy(o => o))}}}");

            if (result.Standarts.Count != 0)
            {
                result.Stability = ((set.Objects.Count() - noisyObjects.Count) / (decimal)set.Objects.Count()) * ((set.Objects.Count() - noisyObjects.Count) / (decimal)result.Standarts.Count);
                log.WriteLine($"Stability = {result.Stability}");
            }


            // return result;
            log.WriteLine("====FindStandarts END====");
            return result;
        }
    }
}