using System.Diagnostics;
using ScottPlot;

namespace FlotMaximum;

public static class Curve
{
    public static void CreateCurves(
        double density, int lowerBound, int upperBound, int nbIter,
        List<(string name, Func<FlowNetwork, double> solver)> solvers)
    {
        List<double> xs = [];
        Dictionary<string, List<double>> timings = new();
        foreach (var (name, _) in solvers)
        {
            timings[name] = new List<double>();
        }

        Plot myPlot = new();

        for (int i = 10; i < nbIter; i+=10)
        {
            Console.WriteLine("i = " + i);
            RandomFlowNetwork randomFlow = new(i, density);
            FlowNetwork nf = randomFlow.Generate(lowerBound, upperBound);
            Console.WriteLine("Généré avec " + nf.AdjVertices.Keys.Count + " sommets et " + nf.Edges.Count + " arêtes.");
            
            xs.Add(i);

            foreach (var (name, solver) in solvers)
            {
                Stopwatch sw = Stopwatch.StartNew();
                var result = solver(nf);
                sw.Stop();
                timings[name].Add(sw.Elapsed.TotalSeconds);
                Console.WriteLine($"{name} {result} in {sw.Elapsed.TotalSeconds} s");
            }
            
            foreach (var (name2, values) in timings)
            {
                var curve = myPlot.Add.Scatter(xs, values);
                curve.LegendText = name2;
            }

            myPlot.Axes.Bottom.Label.Text = "Number of nodes";
            myPlot.Axes.Left.Label.Text = "Seconds";
            myPlot.ShowLegend(Alignment.UpperLeft);
            myPlot.SavePng("benchmark.png", 800, 600);
            myPlot = new Plot();
        }
    }
}