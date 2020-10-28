using System;
using System.Collections.Generic;
using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Algorithms
{
    class FindFeatureWeightByDominanceCriterion : Models.IAlgorithm
    { 
        public void Find(ObjectSet set, Models.MetricCalculateFunctionDelegate distFunc, System.IO.StreamWriter log)
        {
            log.WriteLine("====FindFeatureWeightByDominanceCriterion BEGIN====");
            for (int i = 0; i < set.Features.Count; i++)
            {
                var ft = set.Features[i];
                if (!(ft.IsActive)) continue;

                log.WriteLine($"{Environment.NewLine}Feature {ft.Name}");

                if (ft.IsContinuous)
                {
                    var result = Criterions.FirstCriterion.Find(set.Objects.Select(s => new Criterions.FirstCriterion.FirstCriterionParameter()
                    {
                        ObjectIndex = s.Index,
                        ClassValue = s[set.ClassFeatureIndex],
                        Distance = s[i]
                    }), set.ClassValue);

                    log.WriteLine($"\tFirst criterion {result}");
                }
                else
                {
                    var result = Criterions.NonContinuousFeatureCriterion.Find(set.Objects.Select(s => new Criterions.NonContinuousFeatureCriterion.NonContinuousFeatureCriterionParameter()
                    {
                        ObjectIndex = s.Index,
                        ClassValue = s[set.ClassFeatureIndex],
                        FeatureValue = s[i]
                    }), set.ClassValue);

                    log.WriteLine($"\tNonContinuousFeatureCriterion {result.Value}");
                }

                var scResult = Criterions.IntervalCriterion.Find(set.Objects.Select(s => new Criterions.IntervalCriterion.IntervalCriterionParameter()
                {
                    ObjectIndex = s.Index,
                    ClassValue = s[set.ClassFeatureIndex],
                    Distance = s[i]
                }), set.ClassValue);
                log.WriteLine($"\tIntervals:");

                log.WriteLine("\t\t" + String.Join($"{Environment.NewLine}\t\t", scResult));
                var fcParam = set.Objects.Select(s => new Criterions.FirstCriterion.FirstCriterionParameter()
                {
                    ObjectIndex = s.Index,
                    ClassValue = s[set.ClassFeatureIndex],
                    Distance = scResult.First(w => w.ObjectValueStart <= s[i] && s[i] <= w.ObjectValueEnd).FunctionValue
                }).ToList();

                var fcResult = Criterions.FirstCriterion.Find(fcParam, set.ClassValue);

                log.WriteLine($"\tFirst criterion value = {fcResult.Value: 0.000000}");
            }

            log.WriteLine("====FindFeatureWeightByDominanceCriterion END====");
        }
    }
}