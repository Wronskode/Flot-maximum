using System.Diagnostics;
using BenchmarkDotNet.Running;
using FlotMaximum;

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
RandomFlowNetwork randomFlow = new(1000, 0.5);
FlowNetwork nf = randomFlow.Generate();
Console.WriteLine("Généré avec " + nf.AdjVertices.Keys.Count + " sommets et " +
                  nf.Edges.Count + " arêtes.");
Stopwatch sw = new();
sw.Start();
var maxFlow = nf.EdmondsKarp();
sw.Stop();
Console.WriteLine("Edmonds-Karp " + maxFlow.Value + " in " + sw.Elapsed);
sw.Reset();
sw.Start();
var gurobiValue = PL.SolveWithGurobi(nf);
sw.Stop();
Console.WriteLine("Gurobi-Solve " + gurobiValue + " in " + sw.Elapsed);
// sw.Reset();
// sw.Start();
// var plValue = (new PL(nf)).Resoudre();
// Console.WriteLine("PL-Solve " + plValue + " in " + sw.Elapsed);
//BenchmarkRunner.Run<Benchmarks>();