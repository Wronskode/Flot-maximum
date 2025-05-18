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
void DeleteInstances(string path)
{
    var di = new DirectoryInfo(path);
    foreach (FileInfo file in di.GetFiles())
    {
        file.Delete(); 
    }
}

List<int> tailles = new List<int> { 30, 40, 50, 60, 70, 80 , 90, 100, 120, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1200,1500,2000};
List<double> densities = new List<double> {0.3, 0.5, 0.7, 0.9};
int numberOfInstances = 5;
float tot = tailles.Count*densities.Count*numberOfInstances;
float compteurTot = 0;


void CreateInstances(string instancesPath, List<int> tailles, List<double> densities, int numberOfInstances)
{
    //DeleteInstances(instancesPath);

    foreach (int n in tailles)
    {
        foreach (double d in densities)
        {
            int i = 1;
            while (i <= numberOfInstances)
            {
                RandomFlowNetwork randomFlow = new(n, d);
                FlowNetwork nf = randomFlow.Generate();
                int ar = nf.GetNombreAretes();
                //Console.WriteLine(nf);
                bool res = nf.IsConnected();
                //Console.WriteLine(res);
                if (res)
                {
                    string fileName = instancesPath + $"inst{n}_{d}_{i}.txt";
                    nf.CreateGraphWeightFile(fileName);
                    //Console.WriteLine("Le fichier a été créé avec succès.");
                    compteurTot++;
                    Console.WriteLine("Bien créé | arêtes : "+ar+"/"+d * (n * (n - 1)) + "   et "+ (compteurTot * 100f) / tot + "%");
                    i += 1;
                }

                Console.WriteLine("\n");
            }
        }
    }
}

CreateInstances(instancesPath, tailles, densities, numberOfInstances);
