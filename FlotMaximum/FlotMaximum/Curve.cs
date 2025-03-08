using System.Diagnostics;
using ScottPlot;

namespace FlotMaximum;

public static class Curve
{
    public static void CreateCurves(
        double density, int lowerBound, int upperBound, int step, int minNodes, int maxNodes,
        List<(string name, Func<FlowNetwork, double> solver)> solvers)
    {
        List<double> xs = [];
        Dictionary<string, List<double>> timings = new();
        foreach (var (name, _) in solvers)
        {
            timings[name] = new List<double>();
        }

        Plot myPlot = new();

        for (int i = minNodes; i <= maxNodes; i+=step)
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
                Console.WriteLine($"{name} {result} in {sw.Elapsed.TotalSeconds}s");
            }
            
            foreach (var (name2, values) in timings)
            {
                var curve = myPlot.Add.Scatter(xs, values);
                curve.LegendText = name2;
            }
            
            myPlot.Axes.Bottom.Label.Text = "Number of nodes";
            myPlot.Axes.Left.Label.Text = "Seconds";
            myPlot.ShowLegend(Alignment.UpperLeft);
            myPlot.Legend.FontSize = 20;
            myPlot.SavePng("../../../../courbes/benchmark"+Math.Round(density, 2)+".png", 1500, 1000);
            myPlot = new Plot();
        }
    }

    public static void CreateCurves(double density, string directoryPath, List<(string name, Func<FlowNetwork, double> solver)> solvers)
    {
        var di = new DirectoryInfo(directoryPath);
        Dictionary<int, Dictionary<string, List<double>>> timingData = new();
        List<double> xs = new();
        var files = di.GetFiles().Where(file =>
        {
            string fileName = file.Name;
            string densityStr = "";
            int i = 1;
            while (fileName[fileName.IndexOf('_') + i] != '_')
            {
                densityStr += fileName[fileName.IndexOf('_') + i];
                i++;
            }
            return Convert.ToDouble(densityStr) == density;
        }).OrderBy(file =>
        {
            FileFlowNetwork ffn = new(directoryPath + file.Name);
            FlowNetwork nf = ffn.Generate();
            return nf.AdjVertices.Count;
        });
        
        foreach (var file in files)
        {
            FileFlowNetwork ffn = new(directoryPath + file.Name);
            FlowNetwork nf = ffn.Generate();
            int nodeCount = nf.AdjVertices.Count;
            
            if (!timingData.ContainsKey(nodeCount))
            {
                timingData[nodeCount] = new Dictionary<string, List<double>>();
                foreach (var (name, _) in solvers)
                {
                    timingData[nodeCount][name] = new List<double>();
                }
                xs.Add(nodeCount);
            }
            
            foreach (var (name, solver) in solvers)
            {
                Stopwatch sw = Stopwatch.StartNew();
                var result = solver(nf);
                sw.Stop();
                timingData[nodeCount][name].Add(sw.Elapsed.TotalSeconds);
                Console.WriteLine($"{name} {result} in {sw.Elapsed.TotalSeconds}s for {nodeCount} nodes");
            }
            Plot myPlot = new Plot();
            foreach (var (name, _) in solvers)
            {
                List<double> avgTimes = timingData.Keys.Select(nc => timingData[nc][name].Average()).ToList();
                var curve = myPlot.Add.Scatter(xs, avgTimes);
                curve.LegendText = name;
            }
            myPlot.Axes.Bottom.Label.Text = "Number of nodes";
            myPlot.Axes.Left.Label.Text = "Seconds";
            myPlot.ShowLegend(Alignment.UpperLeft);
            myPlot.Legend.FontSize = 20;
            myPlot.SavePng("../../../../courbes/benchmark" + Math.Round(density, 2) + ".png", 1500, 1000);
        }
    }
}