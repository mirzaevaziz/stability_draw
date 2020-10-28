namespace FinderOfStandarts.Models
{
    internal interface IAlgorithm
    {
        void Find(ObjectSet set, Models.MetricCalculateFunctionDelegate distFunc, System.IO.StreamWriter log);
    }
}