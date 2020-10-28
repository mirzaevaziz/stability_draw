using System.Collections.Generic;
using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Methods
{
    class ObjectsPhiFinder
    {
        public class ObjectsPhiFinderResult
        {
            public int ObjectIndex { get; set; }
            public decimal Value { get; set; }
        }
        public static IEnumerable<ObjectsPhiFinderResult> Find(ObjectSet set, Models.MetricCalculateFunctionDelegate distFunc)
        {
            var result = new List<ObjectsPhiFinderResult>();

            var dist = Utils.DistanceUtils.FindAllDistance(set, distFunc);

            foreach (var obj in set.Objects)
            {
                var param = set.Objects.Select(s => new Criterions.FirstCriterion.FirstCriterionParameter()
                {
                    ClassValue = s.Data[set.ClassFeatureIndex],
                    ObjectIndex = s.Index,
                    Distance = dist[obj.Index, s.Index]

                });

                // if (param.Any(a => a.Distance == 0M && obj[set.ClassFeatureIndex] != a.ClassValue))
                // {
                //    return new List<ObjectsPhiFinderResult>();
                // }

                var firstCriterionValue = Criterions.FirstCriterion.Find(
                    param,
                    obj.Data[set.ClassFeatureIndex]
                );

                // Find how many obj's friends in [c1,c2]
                var classCountInInterval = param.Count(w => w.Distance <= firstCriterionValue.Distance && w.ClassValue == obj.Data[set.ClassFeatureIndex]);
                // Find how many obj's enemies in [c1,c2]
                var nonClassCountInInterval = param.Count(w => w.Distance <= firstCriterionValue.Distance && w.ClassValue != obj.Data[set.ClassFeatureIndex]);
                // Find how many obj's friends
                decimal classCount = param.Count(w => w.ClassValue == obj.Data[set.ClassFeatureIndex]);
                // Find how many obj's enemies
                decimal nonClassCount = param.Count(w => w.ClassValue != obj.Data[set.ClassFeatureIndex]);

                var o1 = classCountInInterval / classCount;
                var o2 = nonClassCountInInterval / nonClassCount;
                result.Add(new ObjectsPhiFinderResult()
                {
                    ObjectIndex = obj.Index,
                    Value = o1 * (1 - o2)
                });
            }

            return result;
        }
    }
}