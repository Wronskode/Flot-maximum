using System.Diagnostics;
using BenchmarkDotNet.Running;
using FlotMaximum;
using ScottPlot;

Vertex a = new Vertex("a");
Vertex b = new Vertex("b");
Vertex c = new Vertex("c");
Vertex d = new Vertex("d");
Vertex e = new Vertex("e");
Vertex f = new Vertex("f");

Vertex s = new Vertex("s");
Vertex p = new Vertex("p");

// réseau de flot du TD de Bessy
//FlowNetwork nf = new([
//(a, b, 12), (a, c, 10), (d, b, 7), (b, c, 9), (c, a, 4), (c, d, 14)], s, p, [(a, 16), (c, 13)], [(b, 20), (d, 4)],
//[a,b,c,d]);

//FlowNetwork nf = new([
//(a,d, 13), (a, b, 8), (a, c, 10), (b,c,26), (c,d,20),
//(c,e,8),(c,f,24),(d,e,1),(d,b,2)], s, p, [(a, 38), (b, 1), (f, 2)], [(d, 7), (e, 7), (c, 1), (f, 27)]);
Curve.CreateCurves(
    density: 0.3, lowerBound: 1, upperBound: 10, nbIter: 800,
    solvers: new List<(string, Func<FlowNetwork, double>)>
    {
        ("Ford-Fulkerson", nf => nf.FordFulkerson().Value),
        ("Edmonds-Karp", nf => nf.EdmondsKarp().Value),
        ("Gurobi", nf => PL.SolveWithGurobi(nf)),
        ("OrTools", nf => (new PL(nf)).Resoudre())  
    }
);
