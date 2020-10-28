using System.Linq;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Normalize
{
    class NormalizeByFirstCriterion : Models.INormalizationProvider
    {
        public ObjectSet Normalize(ObjectSet set)
        {
            for (int i = 0; i < set.Features.Count; i++)
            {
                if (!set.Features[i].IsContinuous)
                    continue;

                decimal? max_val = null;
                decimal? min_val = null;

                foreach (var item in set.Objects)
                {
                    if (!max_val.HasValue || max_val < item[i])
                        max_val = item[i];
                    if (!min_val.HasValue || min_val > item[i])
                        min_val = item[i];
                }

                if (!max_val.HasValue || !min_val.HasValue || max_val == min_val)
                    continue;

                var critResult = Criterions.FirstCriterion.Find(set.Objects.Select(s => new Criterions.FirstCriterion.FirstCriterionParameter()
                {
                    ClassValue = s[set.ClassFeatureIndex],
                    Distance = s[i],
                    ObjectIndex = s.Index
                }), set.ClassValue);

                foreach (var item in set.Objects)
                {
                    item[i] = critResult.Value * (item[i] - critResult.Distance) / (max_val.Value - critResult.Distance);
                }
            }

            return set;
        }
    }
}