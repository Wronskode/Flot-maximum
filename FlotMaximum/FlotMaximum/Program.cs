using System.Diagnostics;
using FlotMaximum;

Vertex a = new Vertex("a");
Vertex b = new Vertex("b");
Vertex c = new Vertex("c");
Vertex d = new Vertex("d");
Vertex e = new Vertex("e");
Vertex f = new Vertex("f");

Vertex s = new Vertex("s");
Vertex p = new Vertex("p");

//réseau de flot du TD de Bessy
FlowNetwork nf = new([
(a, b, 12), (a, c, 10), (d, b, 7), (b, c, 9), (c, a, 4), (c, d, 14)], s, p, [(a, 16), (c, 13)], [(b, 20), (d, 4)],
[a,b,c,d]);

//FlowNetwork nf = new([
//(a,d, 13), (a, b, 8), (a, c, 10), (b,c,26), (c,d,20),
//(c,e,8),(c,f,24),(d,e,1),(d,b,2)], s, p, [(a, 38), (b, 1), (f, 2)], [(d, 7), (e, 7), (c, 1), (f, 27)]);

RandomFlowNetwork randomFlow = new(3, 6);
//FlowNetwork nf = randomFlow.Generate();
foreach (var edge in nf.Edges)
{
    // edge.Key est un tuple (Vertex, Vertex) représentant l'arête
    // edge.Value est l'int représentant le poids de l'arête
    Console.WriteLine($"Arête: ({edge.Key.Item1}) -> ({edge.Key.Item2}), Poids: {edge.Value}");
}
Console.WriteLine("Généré avec " + nf.AdjVertices.Keys.Count + " sommets et " + (nf.Edges.Count + nf.SourceNeighbors.Count + nf.PuitsNeighbors.Count) + " arêtes.");
var startTime = Stopwatch.GetTimestamp();
var maxFlow = nf.EdmondsKarp();
Console.WriteLine("Edmonds-Karp " + maxFlow.Value);
Console.WriteLine("Elapsed : " + Stopwatch.GetElapsedTime(startTime));

List<List<Vertex>> l = nf.Bfs(nf.Source);
nf.RemoveEdgesBetweenLists(l);

var startTime2 = Stopwatch.GetTimestamp();
var maxFlow2 = nf.EdmondsKarp();
Console.WriteLine("Dinic " + maxFlow2.Value);
Console.WriteLine("Elapsed : " + Stopwatch.GetElapsedTime(startTime2));


