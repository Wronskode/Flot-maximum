using FlotMaximum;


// réseau de flot du TD de Bessy
//FlowNetwork nf = new([
//(a, b, 12), (a, c, 10), (d, b, 7), (b, c, 9), (c, a, 4), (c, d, 14)], s, p, [(a, 16), (c, 13)], [(b, 20), (d, 4)],
//[a,b,c,d]);

//FlowNetwork nf = new([
//(a,d, 13), (a, b, 8), (a, c, 10), (b,c,26), (c,d,20),
//(c,e,8),(c,f,24),(d,e,1),(d,b,2)], s, p, [(a, 38), (b, 1), (f, 2)], [(d, 7), (e, 7), (c, 1), (f, 27)]);

var instancesPath = "../../../../Instances/";

List<double> densities = new List<double> {0.1, 0.5, 0.9};
var di = new DirectoryInfo(instancesPath);
foreach (FileInfo file in di.GetFiles())
{
    file.Delete(); 
}
int i;

List<int> tailles = new List<int> {10, 20, 50, 100, 200, 500, 1000};

for (int n = 10; n < 250; n+=10)
{
    foreach (double d in densities)
    {
        i = 1;
        while (i <= 3)
        {
            RandomFlowNetwork randomFlow = new(n, d);
            FlowNetwork nf = randomFlow.Generate();
            bool res = true;
            if (res)
            {
                string fileName = instancesPath + $"inst{n}_{d}_{i}.txt";
                nf.CreateGraphWeightFile(fileName);
                Console.WriteLine("Le fichier a été créé avec succès.");
                i += 1;
            }

            Console.WriteLine("\n");
        }
    }
}



var solvers = new List<(string, Func<FlowNetwork, double>)>
{
    ("Ford-Fulkerson", nf => nf.FordFulkerson().Value),
    ("Edmonds-Karp", nf => nf.EdmondsKarp().Value),
    ("Gurobi", nf => PL.SolveWithGurobi(nf)),
    ("OrTools", nf => (new PL(nf)).Resoudre())
};

foreach (var density in densities)
{
    Curve.CreateCurves(density, instancesPath, solvers);
}