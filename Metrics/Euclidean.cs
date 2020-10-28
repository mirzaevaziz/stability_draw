using System;
using System.Linq;
using System.Collections.Generic;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Metrics
{
    class Euclidean : Models.IMetric
    {
        public decimal Calculate(ObjectData obj1, ObjectData obj2, List<Feature> features)
        {
            var result = 0M;

            for (int i = 0; i < features.Count; i++)
            {
                if (!features[i].IsActive || features[i].IsClass)
                    continue;
                if (features[i].IsContinuous)
                {
                    var r = obj1[i] - obj2[i];
                    result += r * r;
                }
                else if (obj1[i] != obj2[i])
                    throw new NotImplementedException();
            }

            return (decimal)Math.Sqrt((double)result);
        }

        public bool CanCalculate(List<Feature> features)
        {
            return features.All(f => f.IsClass || (f.IsContinuous && f.IsActive));
        }
    }
}