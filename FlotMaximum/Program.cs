using FlotMaximum;


// réseau de flot du TD de Bessy
/*Vertex a = new Vertex("a");
Vertex b = new Vertex("b");
Vertex c = new Vertex("c");
Vertex d = new Vertex("d");
Vertex e = new Vertex("e");
Vertex f = new Vertex("f");

Vertex s = new Vertex("s");
Vertex p = new Vertex("p");
FlowNetwork nf = new([
(a, b, 12), (a, c, 10), (d, b, 7), (b, c, 9), (c, a, 4), (c, d, 14)], s, p, [(a, 16), (c, 13)], [(b, 20), (d, 4)],
[a,b,c,d]);*/

//FlowNetwork nf = new([
//(a,d, 13), (a, b, 8), (a, c, 10), (b,c,26), (c,d,20),
//(c,e,8),(c,f,24),(d,e,1),(d,b,2)], s, p, [(a, 38), (b, 1), (f, 2)], [(d, 7), (e, 7), (c, 1), (f, 27)]);

// var distances = nf.BFS_GetLevels();
// foreach (KeyValuePair<Vertex, int> kvp in distances)
// {
//     Console.WriteLine($"Distance: {kvp.Key} - {kvp.Value}");
// }
// return;
const string instancesPath = "../../../../Instances/";
List<double> densities = [0.1, 0.5, 0.9];
void DeleteInstances(string path)
{
    var di = new DirectoryInfo(path);
    foreach (FileInfo file in di.GetFiles())
    {
        file.Delete(); 
    }
}

void CreateInstances(string instancesPath, List<double> densities)
{
    DeleteInstances(instancesPath);

    List<int> tailles = [10, 20, 50, 100, 200, 500, 1000];

    for (int n = 30; n <= 200; n+=10)
    {
        foreach (double d in densities)
        {
            var i = 1;
            while (i <= 3)
            {
                RandomFlowNetwork randomFlow = new(n, d);
                FlowNetwork nf = randomFlow.Generate();
                bool res = nf.IsConnected();
                if (res)
                {
                    string fileName = instancesPath + $"inst{n}_{d}_{i}.txt";
                    nf.CreateGraphWeightFile(fileName);
                    Console.WriteLine("Le fichier a été créé avec succès.");
                    i += 1;
                }
            }
        }
    }
}

//CreateInstances(instancesPath, densities);
// Ligne Gurobi à commenter si vous n'avez pas de licence
var solvers = new List<(string, Func<FlowNetwork, double>)>
{
    ("Ford-Fulkerson", nf => nf.FordFulkerson().Value),
    ("Edmonds-Karp", nf => nf.EdmondsKarp().Value),
    ("Dinic", nf => nf.Dinic().Value),
    ("Poussage-Réétiquetage", nf => nf.Push_Label().Value),
    ("Gurobi", nf => PL.SolveWithGurobi(nf)),
    ("SCIP", nf => PL.SolveWithOrTools(nf, "SCIP")),
    //("GLOP", nf => PL.SolveWithOrTools(nf, "GLOP")),
    //("CP-SAT", nf => PL.SolveWithOrTools(nf, "CP-SAT")),
};

foreach (var density in densities)
{
    Curve.CreateCurves(density, instancesPath, solvers);
}