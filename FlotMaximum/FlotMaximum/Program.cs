// See https://aka.ms/new-console-template for more information

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
FlowNetwork nf = new([
   (a, b, 12), (a, c, 10), (d, b, 7), (b, c, 9), (c, a, 4), (c, d, 14)], s, p, [(a, 16), (c, 13)], [(b, 20), (d, 4)]);

//FlowNetwork nf = new([
  //  (a,d, 13), (a, b, 8), (a, c, 10), (b,c,26), (c,d,20),
//(c,e,8),(c,f,24),(d,e,1),(d,b,2)], s, p, [(a, 38), (b, 1), (f, 2)], [(d, 7), (e, 7), (c, 1), (f, 27)]);
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
int i = 0;
foreach (var edge in val.Item1)
{
    Console.WriteLine(edge);
    i = val.Item2;
}
Console.WriteLine("Valeur du flot : " + i);