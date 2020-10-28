using System.Collections.Generic;
using System.Linq;

namespace FinderOfStandarts.Models
{
    class ObjectSet
    {
        public List<ObjectData> Objects { get; set; }
        public List<Feature> Features { get; set; }
        public decimal ClassValue { get; set; }
        public string Name { get; }
        public int ClassFeatureIndex { get; set; }

        public ObjectSet(List<ObjectData> objects, List<Feature> features, decimal classValue, string name = "New object set")
        {
            if (!(objects?.Count > 0))
                throw new System.ArgumentException("Objects weren't given.");

            if (!(features?.Count > 0))
                throw new System.ArgumentException("Features weren't given.");
            foreach (var item in objects)
            {
                if (features.Count != item.Data.Length)
                    throw new System.ArgumentException($"Length of objects #{item.Index} columns doesn't match to features length.");
            }

            Objects = objects;
            Features = features;
            ClassValue = classValue;
            Name = name;
            ClassFeatureIndex = -1;
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].IsClass)
                {
                    if (ClassFeatureIndex > -1)
                        throw new System.ArgumentException("Class feature was given more than once.");
                    ClassFeatureIndex = i;
                }
            }
            if (ClassFeatureIndex == -1)
                throw new System.ArgumentException("Class feature wasn't given.");
        }

        public IEnumerable<ObjectData> ClassObjects { get { return Objects.Where(w => w[ClassFeatureIndex] == ClassValue); } }
        public IEnumerable<ObjectData> NonClassObjects { get { return Objects.Where(w => w[ClassFeatureIndex] != ClassValue); } }
        public IEnumerable<decimal> GetClassValues()
        {
            return Objects.Select(s => s[ClassFeatureIndex]).Distinct();
        }

        public override string ToString()
        {
            return $@"ObjectSet = ""{Name}""
               , Objects count = {Objects.Count} 
               , Class value = {ClassValue}
               , Class objects = {ClassObjects.Count()}
               , Non Class objects = {NonClassObjects.Count()}
               , ClassFeatureIndex = {ClassFeatureIndex}";
        }
    }
}