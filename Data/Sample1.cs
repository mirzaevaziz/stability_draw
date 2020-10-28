using System.Collections.Generic;
using FinderOfStandarts.Models;

namespace FinderOfStandarts.Data
{
    class Sample1
    {
        public ObjectSet GetObjectSet(decimal classValue){
            var features = new List<Feature>();
            for (int i = 0; i < 3; i++)
            {
                features.Add(new Feature()
                {
                    // Index = i++,
                    IsActive = true,
                    IsContinuous = i != 2,
                    IsClass = i == 2,
                    Name = $"Ft {i:000}"
                });
            }

            var objects = new List<ObjectData>();
            int ind = 0;

            for (int x = 1; x <= 10; x++)
            {
                for (int y = 1; y <= 10; y++)
                {
                    var cl = 1;
                    if ((x > 5 && y <= 5) || (x <= 5 && y > 5))
                        cl = 2;
                    objects.Add(new ObjectData()
                    {
                        Data = new decimal[]{ x, y, cl },
                        Index = ind++
                    });
                }
            }

            var set = new ObjectSet(objects, features, classValue, $"Sample1");

            return set;
        }
    }
}