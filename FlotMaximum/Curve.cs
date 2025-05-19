using System.Diagnostics;
using System.Text.RegularExpressions;
using ScottPlot;

namespace FlotMaximum;

public static class Curve
{
    public static void CreateCurves(double density, string directoryPath, List<(string name, Func<FlowNetwork, double> solver)> solvers, int nMax)
    {
        var di = new DirectoryInfo(directoryPath);
        Dictionary<int, Dictionary<string, List<double>>> timingData = new();
        List<double> xs = new();
        var files = di.GetFiles().Where(file =>
        {
            Match dens = Regex.Match(file.Name, @"inst.*_(.*)_");
            if (!dens.Success)
            {
                return false;
            }
            string densityStr = dens.Groups[1].Value;
            return Math.Abs(Convert.ToDouble(densityStr) - density) < 1e-3;
        }).OrderBy(file =>
        {
            Match match = Regex.Match(file.Name, @"^inst(\d+)");
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        });
        
        foreach (var file in files)
        {
            Match match = Regex.Match(file.Name, @"^inst(\d+)");
            if (match.Success && int.Parse(match.Groups[1].Value) > nMax)
            {
                return;
            }
            FileFlowNetwork ffn = new(directoryPath + file.Name);
            FlowNetwork nf = ffn.Generate();
            int nodeCount = nf.AdjVertices.Count;
            
            if (!timingData.TryGetValue(nodeCount, out Dictionary<string, List<double>>? value))
            {
                value = new Dictionary<string, List<double>>();
                timingData[nodeCount] = value;
                foreach (var (name, _) in solvers)
                {
                    timingData[nodeCount][name] = new List<double>();
                }
                xs.Add(nodeCount);
            }
            foreach (var (name, solver) in solvers)
            {
                var nf2 = (FlowNetwork)nf.Clone(); // Important si on modifie le réseau de flot
                Stopwatch sw = Stopwatch.StartNew();
                var result = solver(nf2);
                sw.Stop();
                value[name].Add(sw.Elapsed.TotalSeconds);
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
            myPlot.Legend.FontName = Fonts.Serif;
            myPlot.SavePng("../../../../courbes/benchmark" + Math.Round(density, 2) + "_" +nMax+ ".png", 1500, 1000);
        }
    }
}