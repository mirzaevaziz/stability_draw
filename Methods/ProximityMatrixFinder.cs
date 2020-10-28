using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Methods
{
    class ProximityMatrixFinder
    {
        public class ProximityMatrixFinderResult
        {
            public ProximityMatrixFinderResult()
            {
                Matrix = new Dictionary<Tuple<int, int>, decimal>();
                PairSequence = new List<Tuple<int, int>>();
            }

            public Dictionary<Tuple<int, int>, decimal> Matrix { get; set; }

            public List<Tuple<int, int>> PairSequence { get; set; }

            public override string ToString() {
                var sb = new StringBuilder();

                sb.AppendLine("Matrix is: ");
                foreach (var item in Matrix.Keys)
                {
                    sb.AppendLine($"[{item}] = {Matrix[item]}");
                }

                sb.AppendLine("Sequence is: ");
                foreach (var item in PairSequence)
                {
                    sb.AppendLine($"[{item}], ");
                }

                return sb.ToString();
            }
        }

        public static ProximityMatrixFinderResult Find(ObjectSet set)
        {
            var result = new ProximityMatrixFinderResult();

            Dictionary<int, Criterions.FirstCriterion.FirstCriterionResult> critResults = new Dictionary<int, Criterions.FirstCriterion.FirstCriterionResult>();
            for (int i = 0; i < set.Features.Count; i++)
            {
                var ft = set.Features[i];
                if (ft.IsClass || !ft.IsContinuous || !ft.IsActive) continue;

                critResults[i] = Criterions.FirstCriterion.Find(
                    set.Objects.Select(s => new Criterions.FirstCriterion.FirstCriterionParameter()
                    {
                        ClassValue = s[set.ClassFeatureIndex],
                        Distance = s[i],
                        ObjectIndex = s.Index
                    })
                    , set.ClassValue);
            }

            var classValues = set.Objects.Select(s => s[set.ClassFeatureIndex]).Distinct();
            var denominator = 0M;

            foreach (var cv in classValues)
            {
                var cnt = set.Objects.Count(w => w[set.ClassFeatureIndex] == cv);
                denominator += cnt * (set.Objects.Count - cnt);
            }

            denominator *= 2;

            for (int i = 0; i < set.Features.Count - 1; i++)
            {
                var fi = set.Features[i];
                if (!fi.IsActive || fi.IsClass) continue;

                for (int j = i + 1; j < set.Features.Count; j++)
                {
                    var fj = set.Features[j];
                    if (!fj.IsActive || fj.IsClass) continue;
                    decimal s = 0;
                    for (int a = 0; a < set.Objects.Count; a++)
                    {
                        decimal xai = 0;
                        if (fi.IsContinuous)
                        {
                            xai = (set.Objects[a][i] <= critResults[i].Distance) ? 0 : 1;
                        }
                        else
                        {
                            xai = set.Objects[a][i];
                        }

                        decimal xaj = 0;
                        if (fj.IsContinuous)
                        {
                            xaj = (set.Objects[a][j] <= critResults[j].Distance) ? 0 : 1;
                        }
                        else
                        {
                            xaj = set.Objects[a][j];
                        }

                        for (int b = 0; b < set.Objects.Count; b++)
                        {
                            if (a == b) continue;

                            decimal xbi = 0;
                            if (fi.IsContinuous)
                            {
                                xbi = (set.Objects[b][i] <= critResults[i].Distance) ? 0 : 1;
                            }
                            else
                            {
                                xbi = set.Objects[b][i];
                            }

                            decimal xbj = 0;
                            if (fj.IsContinuous)
                            {
                                xbj = (set.Objects[b][j] <= critResults[j].Distance) ? 0 : 1;
                            }
                            else
                            {
                                xbj = set.Objects[b][j];
                            }

                            decimal g = 0;
                            if (xai != xbi && xaj != xbj)
                            { g = 2; }
                            else if (xai == xbi || xaj == xbj)
                            { g = 1; }
                            decimal alpha = (set.Objects[a][set.ClassFeatureIndex] == set.Objects[b][set.ClassFeatureIndex]) ? 0 : 1;
                            s += alpha * g;
                        }
                    }

                    result.Matrix[Tuple.Create(i, j)] = s / denominator;
                }
            }
            var seenFeatures = new HashSet<int>();
            result.PairSequence = result.Matrix.OrderBy(o => o.Value).Select(s => s.Key).ToList();

            int pairCounter = 0;
            do
            {
                if (seenFeatures.Contains(result.PairSequence[pairCounter].Item1) || seenFeatures.Contains(result.PairSequence[pairCounter].Item2))
                {
                    result.PairSequence.RemoveAt(pairCounter);
                }
                else
                {
                    seenFeatures.Add(result.PairSequence[pairCounter].Item1);
                    seenFeatures.Add(result.PairSequence[pairCounter].Item2);
                    pairCounter++;
                }
            } while (pairCounter < result.PairSequence.Count);

            return result;
        }
    }


}