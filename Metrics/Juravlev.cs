using System;
using System.Collections.Generic;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Metrics
{   
    class Juravlev : Models.IMetric
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
                    if (r < 0) r *= -1;

                    result += r;
                }
                else if (obj1[i] != obj2[i])
                    result += 1;
            }

            return result;
        }

        public bool CanCalculate(List<Feature> features)
        {
            return true;
        }
    }
}