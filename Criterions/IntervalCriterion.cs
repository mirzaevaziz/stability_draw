using System;
using System.Collections.Generic;
using System.Linq;

namespace FinderOfStandarts.Criterions
{
    class IntervalCriterion
    {
        public class IntervalCriterionParameter
        {
            public int ObjectIndex { get; set; }
            public decimal Distance { get; set; }
            public decimal ClassValue { get; set; }
        }

        public class IntervalCounter
        {
            public int Index { get; set; }
            public int ClassCount { get; set; }
            public int NonClassCount { get; set; }
            public decimal Value { get; internal set; }
        }

        public class IntervalCriterionResult
        {
            public int ObjectIndexStart { get; set; }
            public int ObjectIndexEnd { get; set; }
            public decimal ObjectValueStart { get; set; }
            public decimal ObjectValueEnd { get; set; }
            public decimal Value { get; set; }
            public int PositionIndexStart { get; internal set; }
            public int PositionIndexEnd { get; internal set; }
            public decimal FunctionValue { get; internal set; }

            public override string ToString()
            {
                return $"ObjectIndexStart = {ObjectIndexStart}, ObjectIndexEnd = {ObjectIndexEnd}, ObjectValueStart = {ObjectValueStart:0.000000}, ObjectValueEnd = {ObjectValueEnd:0.000000}, Value = {Value:0.000000}, FunctionValue = {FunctionValue:0.000000}{Environment.NewLine}";
            }
        }

        public static IEnumerable<IntervalCriterionResult> Find(IEnumerable<IntervalCriterionParameter> objects, decimal classValue)
        {
            var result = new List<IntervalCriterionResult>();

            int k = 0, ck = 0;

            var sorted = objects.OrderBy(o => o.Distance).Select(s => new IntervalCounter
            {
                Index = s.ObjectIndex,
                ClassCount = (s.ClassValue == classValue) ? ++k : k,
                NonClassCount = (s.ClassValue != classValue) ? ++ck : ck,
                Value = s.Distance
            }).ToList();

            Devide_to_Interval(sorted, 0, k + ck - 1, k, ck, result);

            return result;
        }

        static void Devide_to_Interval(IList<IntervalCounter> objects, int start_p, int end_p, decimal classCount, decimal nonClassCount, IList<IntervalCriterionResult> result)
        {
            decimal max = -1, tekushiy, fn = 0;
            int pos_st, pos_end;
            pos_st = start_p;
            pos_end = end_p;

            // if (pos_st == pos_end)
            // {
            //     result.Add(new IntervalCriterionResult()
            //     {
            //         ObjectIndexEnd = start_p,
            //         ObjectIndexStart = end_p,
            //         Value = 0
            //     });

            //     return;
            // }
            for (int i = start_p; i <= end_p; i++)
            {
                int k = 0, ck = 0;
                if (i > 0)
                {
                    k = objects[i - 1].ClassCount;
                    ck = objects[i - 1].NonClassCount;
                }
                if (i != start_p && objects[i].Value == objects[i - 1].Value) continue;
                for (int j = i; j <= end_p; j++)
                {
                    // System.Console.WriteLine($"[{objects[i].Value}, {objects[j].Value}]");
                    if (j < end_p && objects[j].Value == objects[j + 1].Value) continue;
       
                    tekushiy = Math.Abs((objects[j].ClassCount - k) / classCount - (objects[j].NonClassCount - ck) / nonClassCount);

                    // System.Console.WriteLine($"\t\t{Math.Round(tekushiy, 6)} > {Math.Round(max, 6)}\t\t {objects[j].ClassCount - k}, {objects[j].NonClassCount - ck}");

                    if (Math.Round(tekushiy, 6) >= Math.Round(max, 6))
                    {
                        max = tekushiy;
                        pos_st = i;
                        pos_end = j;
                        fn = ((objects[j].ClassCount - k) / classCount) / ((objects[j].ClassCount - k) / classCount + (objects[j].NonClassCount - ck) / nonClassCount);
                    }
                }
            }
            result.Add(new IntervalCriterionResult()
            {
                PositionIndexStart = pos_st,
                PositionIndexEnd = pos_end,
                ObjectIndexStart = objects[pos_st].Index,
                ObjectIndexEnd = objects[pos_end].Index,
                ObjectValueStart = objects[pos_st].Value,
                ObjectValueEnd = objects[pos_end].Value,
                Value = max,
                FunctionValue = fn
            });
            // System.Console.WriteLine($"\t\tMax={max} {objects[pos_st].Value}, {objects[pos_end].Value}");

            if (start_p < pos_st)
                Devide_to_Interval(objects, start_p, pos_st - 1, classCount, nonClassCount, result);

            if (pos_end < end_p)
                Devide_to_Interval(objects, pos_end + 1, end_p, classCount, nonClassCount, result);
        }

    }
}