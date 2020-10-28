using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Algorithms
{
    class FindAllFeaturesByPhi
    {
        public static void Find(ObjectSet set, MetricCalculateFunctionDelegate distFunc, StreamWriter log, Tuple<int, int> firstPairFeature)
        {
            //Turn on only first pair feature and remember not active features
            List<int> deactivatedFeatures = new List<int>();
            for (int i = 0; i < set.Features.Count; i++)
            {
                if (!set.Features[i].IsActive)
                    deactivatedFeatures.Add(i);
                else
                    set.Features[i].IsActive = set.Features[i].IsClass || i == firstPairFeature.Item1 || i == firstPairFeature.Item2;
            }

            var prevPhiList = Methods.ObjectsPhiFinder.Find(set, distFunc);
            int? maxF = null;
            decimal maxR = 0;
            IEnumerable<Methods.ObjectsPhiFinder.ObjectsPhiFinderResult> maxPhiList = null;
            do
            {
                for (int i = 0; i < set.Features.Count; i++)
                {
                    if (set.Features[i].IsActive || deactivatedFeatures.Contains(i))
                        continue;
                    set.Features[i].IsActive = true;
                    log.WriteLine($"Finding phi for feature {i}");
                    var phiList = Methods.ObjectsPhiFinder.Find(set, distFunc);
                    if (phiList.Count() == 0)
                        continue;
                    set.Features[i].IsActive = false;
                    var R = phiList.Count(w => w.Value >= prevPhiList.FirstOrDefault(f => f.ObjectIndex == w.ObjectIndex).Value) / (decimal)set.Objects.Count;
                    log.WriteLine($"\t\tR = {R}, MaxR = {maxR}");
                    if (R > 0.5M && (maxPhiList == null || maxR <= R))
                    {
                        maxPhiList = phiList;
                        maxR = R;
                        maxF = i;
                        if (R == 1) break;
                    }
                }
                if (!maxF.HasValue || set.Features[maxF.Value].IsActive)
                    break;
                set.Features[maxF.Value].IsActive = true;
                log.WriteLine($"\tFound {maxF}");
            } while (true);
            log.WriteLine("Features:");

            for (int i = 0; i < set.Features.Count; i++)
            {
                var ft = set.Features[i];
                if (ft.IsActive || ft.IsClass)
                    log.WriteLine($"{i}. {ft}");
            }
        }
    }
}