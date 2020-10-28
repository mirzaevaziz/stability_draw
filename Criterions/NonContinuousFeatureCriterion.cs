using System.Collections.Generic;
using System.Linq;

namespace FinderOfStandarts.Criterions
{
    public class NonContinuousFeatureCriterion
    {
        public class NonContinuousFeatureCriterionParameter
        {
            public int ObjectIndex { get; set; }
            public decimal FeatureValue { get; set; }
            public decimal ClassValue { get; set; }
        }

        public class NonContinuousFeatureCriterionResult
        {
            public decimal Value { get; set; }
        }

        public static NonContinuousFeatureCriterionResult Find(IEnumerable<NonContinuousFeatureCriterionParameter> objects, decimal classValue)
        {
            var result = new NonContinuousFeatureCriterionResult();

            var classCount = objects.Count(w => w.ClassValue == classValue);
            var nonClassCount = objects.Count(w => w.ClassValue != classValue);

            var pList = objects.Select(s => s.FeatureValue).Distinct().ToList();
            decimal sum = 0;
            decimal sum2 = 0;
            foreach (var p in pList)
            {
                var g1r = objects.Count(w => w.FeatureValue == p && w.ClassValue == classValue);
                var g2r = objects.Count(w => w.FeatureValue == p && w.ClassValue != classValue);

                sum += g1r * g2r;

                sum2 += g1r * (g1r - 1) + g2r * (g2r - 1);
            }

            var lambda_r = 1 - sum / (classCount * nonClassCount);

            decimal d1r = 0;
            decimal d2r = 0;
            if (pList.Count > 2)
            {
                var l1r = objects.Where(w => w.ClassValue == classValue).Select(s => s.FeatureValue).Distinct().Count();
                var l2r = objects.Where(w => w.ClassValue != classValue).Select(s => s.FeatureValue).Distinct().Count();
                d1r = (classCount - l1r + 1) * (classCount - l1r);
                d2r = (nonClassCount - l2r + 1) * (nonClassCount - l2r);
            }
            else
            {
                d1r = classCount * (classCount - 1);
                d2r = nonClassCount * (nonClassCount - 1);
            }

            decimal betta_r = 0;

            if (d1r + d2r > 0)
            {

                betta_r = sum2 / (d1r + d2r);
            }

            result.Value = betta_r * lambda_r;

            return result;
        }
    }
}