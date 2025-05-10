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
var directoryPath = "../../../../Instances3/";
var di = new DirectoryInfo(directoryPath);
foreach (FileInfo file in di.GetFiles())
{
    file.Delete(); 
}
int i = 1;

List<int> tailles = new List<int> {30, 40, 50, 60, 70, 80, 90, 100, 120, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1200,1500,2000};
List<double> densitites = new List<double> {0.3, 0.5, 0.7, 0.9};
float tot = tailles.Count*densitites.Count*10;
float compteurTot = 0;

foreach (int n in tailles)
{
    foreach (double d in densitites)
    {
        i = 1;
        while (i <= 10)
        {
            RandomFlowNetwork randomFlow = new(n, d);
            FlowNetwork nf = randomFlow.Generate();
            //Console.WriteLine(nf);
            bool res = nf.IsConnected();
            //Console.WriteLine(res);
            if (res)
            {
                string fileName = directoryPath + $"inst{n}_{d}_{i}.txt";
                nf.CreateGraphWeightFile(fileName);
                //Console.WriteLine("Le fichier a été créé avec succès.");
                compteurTot++;
                Console.WriteLine("Bien créé : " + (compteurTot * 100f) / tot + "%");
                i += 1;
            }

            Console.WriteLine("\n");
        }
    }
}

string fileName2 = directoryPath+"inst5_0,4_1.txt";
FileFlowNetwork ffn = new(fileName2);
FlowNetwork flotAvecFile = ffn.Generate();
Console.WriteLine("Flot reconstruit par le fichier : ");
Console.WriteLine(flotAvecFile);

// sw.Reset();
// sw.Start();
// var plValue = (new PL(nf)).Resoudre();
// Console.WriteLine("PL-Solve " + plValue + " in " + sw.Elapsed);
//BenchmarkRunner.Run<Benchmarks>();