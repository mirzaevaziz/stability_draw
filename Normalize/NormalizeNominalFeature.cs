using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Normalize
{
    class NormalizeNominalFeature
    {
        public ObjectSet Normalize(ObjectSet set)
        {
            for (int i = 0; i < set.Features.Count; i++)
            {
                if (set.Features[i].IsContinuous)
                    continue;

                var values = set.Objects.Select(s => s[i]).Distinct().OrderBy(o => o).ToList();

                for (int j = 0; j < set.Objects.Count; j++)
                {
                    var obj = set.Objects[j];
                    var val = values.IndexOf(obj[i]) + 1;
                    obj[i] = val;
                    // .Where(w => w[i] == values[j]).ToList().ForEach(item => item[i] = j + 1);
                }
            }

            return set;
        }
    }
}