// See https://aka.ms/new-console-template for more information

using FlotMaximum;

Vertex a = new Vertex("a");
Vertex c = new Vertex("c");
Vertex b = new Vertex("b");
Vertex d = new Vertex("d");


Vertex s = new Vertex("s");
Vertex p = new Vertex("p");

// réseau de flot du TD de Bessy
FlowNetwork nf = new([(a, c, 10), (c, a, 4), (a, b, 12), (b, c, 9), (c, d, 14), (d, b, 7)], s, p, [(a, 16), (c, 13)], [(b, 20), (d, 4)]);
foreach (var edge in nf.Edges)
{
    Console.WriteLine(edge);
}
Console.WriteLine("\n\n");
Dictionary<(Vertex, Vertex), int> flot = new();
foreach (var edge in nf.NewEdges.Keys)
{
    flot.Add(edge, 0);
}
//FlowNetwork ne = nf.GetGraphResiduel(flot);

/*foreach (var edge in nf.NewEdges)
{
    Console.WriteLine(edge);
}*/
//
// foreach (var v in ne.Chemin())
// {
//     Console.WriteLine(v.id);
// }
var val = nf.FordFulkerson();
foreach (var edge in val.Item1)
{
    Console.WriteLine(edge + " " + val.Item2);
}