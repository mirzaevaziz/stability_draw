using System.Collections.Generic;
using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Algorithms
{
    class FindInformativeFeaturesByPhiAndDominance : Models.IAlgorithm
    {
        public void Find(ObjectSet set, Models.MetricCalculateFunctionDelegate distFunc, System.IO.StreamWriter log)
        {
            log.WriteLine("====FindInformativeFeaturesByPhiAndDominance BEGIN====");
            var firstPairFeature = Methods.FindFirstPairFeatureForObjectSetByDominance.Find(set, distFunc, new HashSet<int>(), log);

            log.WriteLine(firstPairFeature);

            FindAllFeaturesByPhi.Find(set, distFunc, log, firstPairFeature);
            log.WriteLine("====FindInformativeFeaturesByPhiAndDominance END====");
        }
    }
}