using System.Collections.Generic;
using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Algorithms
{
    class FindInformativeFeaturesByPhi : Models.IAlgorithm
    {
        public void Find(ObjectSet set,
        Models.MetricCalculateFunctionDelegate distFunc,
        System.IO.StreamWriter log)
        {
            log.WriteLine("====FindInformativeFeaturesByPhi BEGIN====");
            var firstPairFeature = Methods.FindFirstPairFeatureForObjectSet.Find(set, distFunc, log);

            log.WriteLine(firstPairFeature);

            FindAllFeaturesByPhi.Find(set, distFunc, log, firstPairFeature);
            log.WriteLine("====FindInformativeFeaturesByPhi END====");
        }
    }
}