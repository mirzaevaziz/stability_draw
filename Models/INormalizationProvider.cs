namespace FinderOfStandarts.Models
{
    internal interface INormalizationProvider{
        ObjectSet Normalize(ObjectSet set);
    }
}