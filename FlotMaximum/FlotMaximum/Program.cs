using System.Diagnostics;
using BenchmarkDotNet.Running;
using FlotMaximum;
using ScottPlot;


// réseau de flot du TD de Bessy
//FlowNetwork nf = new([
//(a, b, 12), (a, c, 10), (d, b, 7), (b, c, 9), (c, a, 4), (c, d, 14)], s, p, [(a, 16), (c, 13)], [(b, 20), (d, 4)],
//[a,b,c,d]);

//FlowNetwork nf = new([
//(a,d, 13), (a, b, 8), (a, c, 10), (b,c,26), (c,d,20),
//(c,e,8),(c,f,24),(d,e,1),(d,b,2)], s, p, [(a, 38), (b, 1), (f, 2)], [(d, 7), (e, 7), (c, 1), (f, 27)]);


RandomFlowNetwork randomFlow = new(5, 0.5);
FlowNetwork nf = randomFlow.Generate();
Console.WriteLine(nf);
Console.WriteLine("\n"+nf.IsConnected());
// sw.Reset();
// sw.Start();
// var plValue = (new PL(nf)).Resoudre();
// Console.WriteLine("PL-Solve " + plValue + " in " + sw.Elapsed);
//BenchmarkRunner.Run<Benchmarks>();