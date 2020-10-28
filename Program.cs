using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FinderOfStandarts.Models;

namespace pca_experiment1
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            // var dir = new System.IO.DirectoryInfo("Result files");
            // var sb = new StringBuilder();
            // sb.AppendLine("Name & Stability & Noisy objects & Standart objects & Groups");
            // foreach (var fi in dir.EnumerateFiles())
            // {
            //     using (StreamReader sr = new StreamReader(fi.FullName))
            //     {
            //         string line;
            //         while (!sr.EndOfStream)
            //         {
            //             line = sr.ReadLine();
            //             if (sr.Peek() == -1)
            //             {
            //                 Console.WriteLine(line);
            //                 sb.AppendLine(line);
            //             }
            //         }
            //     }
            // }
            // File.WriteAllText(Path.Combine(dir.FullName, "Summary.txt"), sb.ToString());
            // System.Console.WriteLine(sb.ToString());
            
            var distFuncs = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where(x => typeof(IMetric).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

            var normProviders = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where(x => typeof(INormalizationProvider).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

            var algorithms = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where(x => typeof(IAlgorithm).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

            do
            {
                decimal classValue = 1;
                IMetric distFunc = null;
                while (distFunc == null)
                {
                    System.Console.WriteLine("Please, choose metric (default = 0):");
                    int i = 0;
                    distFuncs.ForEach(provider => System.Console.WriteLine($"\t{i++:000}: {provider.Name}"));
                    int.TryParse(Console.ReadLine(), out i);
                    if (i >= 0 && i < distFuncs.Count)
                    {
                        distFunc = (IMetric)Activator.CreateInstance(distFuncs[i]);

                        System.Console.WriteLine($"Metric is {distFuncs[i].Name}");
                    }
                }

                INormalizationProvider normProvider = null;
                while (normProvider == null)
                {
                    System.Console.WriteLine("Please, choose metric (default = 0):");
                    int i = 0;
                    System.Console.WriteLine("\t - :No normalization");
                    normProviders.ForEach(provider => System.Console.WriteLine($"\t{i++:000}: {provider.Name}"));
                    var ans = Console.ReadLine();
                    if (int.TryParse(ans, out i) && i >= 0 && i < normProviders.Count)
                    {
                        normProvider = (INormalizationProvider)Activator.CreateInstance(normProviders[i]);
                    }
                    else if (ans == "-")
                    {
                        break;
                    }
                }
                var dir = new System.IO.DirectoryInfo("Result files");
                if (!dir.Exists)
                    dir.Create();


                var set = new FinderOfStandarts.Data.Sample1().GetObjectSet(classValue);

                var fileName = $"Log - {DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss")} {set.Name} class-{classValue} metric - {distFunc.GetType().Name} normalization - {normProvider?.GetType().Name ?? "No normalization"}.txt";

                using (var log = new System.IO.StreamWriter(System.IO.Path.Combine(dir.Name, fileName)))
                {
                    log.WriteLine(set);
                    log.WriteLine($"Class value = {classValue}");
                    log.WriteLine($"Metric = {distFunc.GetType().Name}");

                    if (normProvider != null)
                    {
                        set = normProvider.Normalize(set);
                    }
                    log.WriteLine($"Normalized by - {normProvider?.GetType().Name ?? "No normalization"}");
                    if (normProvider != null)
                    {
                        set = normProvider.Normalize(set);
                    }

                    var result = FinderOfStandarts.Algorithms.FindStandarts.Find(set, distFunc.Calculate, log);
                    log.WriteLine($"Name & Stability & Noisy objects & Standart objects & Groups");
                    log.WriteLine($"{set.Name} & {result.Stability:0.00000} & {result.ExcludedObjects.Count} & {result.Standarts.Count} & {result.Groups.Count}");
                }

                System.Console.WriteLine($"Result in file \"{fileName}\".");

                System.Console.WriteLine("For quit type q or Q:");
            } while (Console.ReadLine().ToLower() != "q");
        }
    }
}
