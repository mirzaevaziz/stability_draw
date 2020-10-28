using System.Collections.Generic;

namespace FinderOfStandarts.Models
{
    internal delegate decimal MetricCalculateFunctionDelegate(ObjectData obj1, ObjectData obj2, List<Feature> features);

    internal interface IMetric
    {
        decimal Calculate(ObjectData obj1, ObjectData obj2, List<Feature> features);
        bool CanCalculate(List<Feature> features);
    }
}