using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Algorithms
{
    class FindInformativeFeaturesByPhiAndDominance01 : Models.IAlgorithm
    {
        public void Find(ObjectSet set, Models.MetricCalculateFunctionDelegate distFunc, System.IO.StreamWriter log)
        {
            log.WriteLine($"===={this.GetType().Name} BEGIN====");
            var intervals = new Dictionary<int, IEnumerable<Criterions.IntervalCriterion.IntervalCriterionResult>>();
            var objData = new decimal[set.Objects.Count, set.Features.Count];

            for (int i = 0; i < set.Features.Count; i++)
            {
                if (!set.Features[i].IsActive || set.Features[i].IsClass)
                    continue;
                log.WriteLine($"Finding intervals for {set.Features[i]}");
                if (!set.Features[i].IsContinuous)
                {
                    set.Objects.ForEach(item => objData[item.Index, i] = item[i]);
                }
                else
                {
                    var scResult = Criterions.IntervalCriterion.Find(set.Objects.Select(s => new Criterions.IntervalCriterion.IntervalCriterionParameter()
                    {
                        ObjectIndex = s.Index,
                        ClassValue = s[set.ClassFeatureIndex],
                        Distance = s[i]
                    }), set.ClassValue);
                    intervals.Add(i, scResult);
                    log.WriteLine($"\tIntervals:");

                    log.WriteLine("\t\t" + String.Join($"{Environment.NewLine}\t\t", scResult));

                    for (int intervalCounter = 0; intervalCounter < scResult.Count(); intervalCounter++)
                    {
                        var interval = scResult.ElementAt(intervalCounter);

                        set.Objects.Where(w => interval.ObjectValueStart <= w[i] && w[i] <= interval.ObjectValueEnd)
                            .ToList()
                            .ForEach(item => objData[item.Index, i] = interval.FunctionValue);
                    }
                }
            }
            Tuple<int, int> firstPairFeature = null; decimal? maxStability = null;

            for (int i = 0; i < set.Features.Count - 1; i++)
            {
                var fti = set.Features[i];

                if (!fti.IsActive || fti.IsClass) continue;

                for (int j = i + 1; j < set.Features.Count; j++)
                {
                    var ftj = set.Features[j];

                    if (!ftj.IsActive || ftj.IsClass) continue;

                    log.WriteLine($"Finding new intervals and stability for feature combination [{i}, {j}]");

                    // var result = Criterions.NonContinuousFeatureCriterion.Find(set.Objects.Select(s => new Criterions.NonContinuousFeatureCriterion.NonContinuousFeatureCriterionParameter()
                    // {
                    //     ObjectIndex = s.Index,
                    //     ClassValue = s[set.ClassFeatureIndex],
                    //     FeatureValue = objData[s.Index, i] * 10000000 + objData[s.Index, j]
                    // }), set.ClassValue);
                    // var ftij_stability = result.Value;

                    var newFeature = set.Objects.Select(s => new
                    {
                        ObjectIndex = s.Index,
                        ClassValue = s[set.ClassFeatureIndex],
                        Value = objData[s.Index, i] * 10000000 + objData[s.Index, j] //s[i] * 100000000 + s[j]
                    });
                    var ftij_stability = 0M;
                    var values = newFeature.Select(s => s.Value).Distinct().ToList();
                    var classObjects = newFeature.Where(w => w.ClassValue == set.ClassValue);
                    var nonClassObjects = newFeature.Where(w => w.ClassValue != set.ClassValue);
                    foreach (var val in values)
                    {
                        decimal c1 = classObjects.Count(w => w.Value == val);
                        decimal c2 = nonClassObjects.Count(w => w.Value == val);
                        var fc = (c1 / classObjects.Count()) / ((c1 / classObjects.Count()) + (c2 / nonClassObjects.Count()));
                        if (fc > 0.5M)
                            ftij_stability += fc * (c1 + c2);
                        else if (fc < 0.5M)
                            ftij_stability += (1 - fc) * (c1 + c2);
                    }

                    ftij_stability /= set.Objects.Count;

                    // log.WriteLine($"\t\tStability({i}) = {fti_stability:0.000000}, Stability({j}) = {ftj_stability:0.000000}, Stability({i}, {j}) = {ftij_stability:0.000000}");
                    log.WriteLine($"\t\tStability({i}, {j}) = {ftij_stability:0.000000}");
                    if (maxStability == null || maxStability < ftij_stability)
                    {
                        maxStability = ftij_stability;
                        firstPairFeature = new Tuple<int, int>(i, j);
                    }
                }
            }

            log.WriteLine($"\t=== MAX = {maxStability}, pair = {firstPairFeature} ");

            // var firstPairFeature = Methods.FindFirstPairFeatureForObjectSetByDominance.Find(set, distFunc, new HashSet<int>(), log);

            log.WriteLine(firstPairFeature);

            FindAllFeaturesByPhi.Find(set, distFunc, log, firstPairFeature);
            log.WriteLine($"===={this.GetType().Name} END====");
        }
    }
}