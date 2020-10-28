using System;
using System.Collections.Generic;
using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Methods
{
    class FindFirstPairFeatureForObjectSetByDominance
    {
        public static Tuple<int, int> Find(ObjectSet set, Models.MetricCalculateFunctionDelegate distFunc, HashSet<int> exclededObjects, System.IO.StreamWriter log)
        {
            if (set is null)
            {
                throw new System.ArgumentNullException(nameof(set));
            }

            if (distFunc is null)
            {
                throw new System.ArgumentNullException(nameof(distFunc));
            }

            List<int> deactivatedFeatures = new List<int>();
            for (int i = 0; i < set.Features.Count; i++)
            {
                if (!set.Features[i].IsActive)
                    deactivatedFeatures.Add(i);
            }

            set.Features.ForEach(i => i.IsActive = false);

            decimal classCount = set.Objects.Count(w => !exclededObjects.Contains(w.Index) && w[set.ClassFeatureIndex] == set.ClassValue);
            decimal nonClassCount = set.Objects.Count(w => !exclededObjects.Contains(w.Index) && w[set.ClassFeatureIndex] != set.ClassValue);

            decimal? maxSum = null;
            Tuple<int, int> result = null;
            for (int i = 0; i < set.Features.Count - 1; i++)
            {
                if (set.Features[i].IsClass || deactivatedFeatures.Contains(i))
                    continue;

                set.Features[i].IsActive = true;
                for (int j = i + 1; j < set.Features.Count; j++)
                {
                    if (set.Features[j].IsClass || deactivatedFeatures.Contains(j))
                        continue;

                    set.Features[j].IsActive = true;

                    log.WriteLine($"Finding feature max of [{i},{j}]");

                    decimal[,] dist = Utils.DistanceUtils.FindAllDistance(set, distFunc);

                    decimal Rxab = 0;
                    for (int objCounter = 0; objCounter < set.Objects.Count; objCounter++)
                    {
                        if (exclededObjects.Contains(objCounter)) continue;

                        decimal k = 0, ck = 0, max = 0, objectClassValue = set.Objects[objCounter][set.ClassFeatureIndex];

                        var distList = set.Objects.Select(s => new
                        {
                            s.Index,
                            Distance = dist[objCounter, s.Index],
                            ClassValue = s[set.ClassFeatureIndex]
                        }).OrderBy(o => o.Distance).ToList();

                        for (int sObjCounter = 0; sObjCounter < set.Objects.Count; sObjCounter++)
                        {
                            if (exclededObjects.Contains(distList[sObjCounter].Index)) continue;

                            if (distList[sObjCounter].Distance == 0M && distList[sObjCounter].ClassValue != objectClassValue)
                                break;

                            if (distList[sObjCounter].ClassValue == objectClassValue)
                            {
                                k++;
                            }
                            else
                            {
                                ck++;
                            }
                            if (sObjCounter < set.Objects.Count - 1 && distList[sObjCounter].Distance == distList[sObjCounter + 1].Distance) continue;

                            decimal val = -1;
                            if (objectClassValue == set.ClassValue)
                            {
                                val = ((k / classCount) / ((k / classCount) + (ck / nonClassCount))) * (k / classCount);
                            }
                            else
                            {
                                val = ((ck / nonClassCount) / ((k / classCount) + (ck / nonClassCount))) * (ck / nonClassCount);
                            }

                            if (max <= val)
                            {
                                max = val;
                            }
                        }
                        Rxab += max;
                    }

                    log.WriteLine($"Current = {Rxab}; max = {maxSum}");

                    if (!maxSum.HasValue || maxSum < Rxab)
                    {
                        maxSum = Rxab;
                        result = new Tuple<int, int>(i, j);
                    }
                    set.Features[j].IsActive = false;
                }
                set.Features[i].IsActive = false;
            }

            for (int i = 0; i < set.Features.Count; i++)
            {
                if (deactivatedFeatures.Contains(i))
                    continue;

                set.Features[i].IsActive = true;
            }

            return result;
        }
    }
}