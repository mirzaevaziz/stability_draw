using System;
using System.Collections.Generic;
using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Metrics
{
    class Chebyshev : Models.IMetric
    {
        public decimal Calculate(ObjectData obj1, ObjectData obj2, List<Feature> features)
        {
            var result = 0M;

            for (int i = 0; i < features.Count; i++)
            {
                if (!features[i].IsActive || features[i].IsClass)
                    continue;
                if (result < Math.Abs(obj1[i] - obj2[i]))
                {
                    result = Math.Abs(obj1[i] - obj2[i]);
                }
            }

            return result;
        }

        public bool CanCalculate(List<Feature> features)
        {
            return !features.Any(a => !a.IsContinuous);
        }
    }
}