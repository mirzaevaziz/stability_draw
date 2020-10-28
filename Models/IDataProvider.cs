namespace FinderOfStandarts.Models
{

    internal interface IObjectSetProvider
    {
        ObjectSet GetObjectSet(decimal classValue);
    }
}