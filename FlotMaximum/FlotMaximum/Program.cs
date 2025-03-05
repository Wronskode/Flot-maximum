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
int i = 1;
int n = 5;
while(i<=0) {
    RandomFlowNetwork randomFlow = new(n, 0.5);
    FlowNetwork nf = randomFlow.Generate();
    Console.WriteLine(nf);
    bool res = nf.IsConnected();
    Console.WriteLine(res);
    if (res)
    {
        string fileName = $"C:\\Users\\adrie\\Desktop\\Master\\S8\\TER\\Instances\\inst{n}_{i}.txt";
        nf.CreateGraphWeightFile(fileName);
        Console.WriteLine("Le fichier a été créé avec succès.");
        i += 1;
    }
    Console.WriteLine("\n");
}

string fileName2 = $"C:\\Users\\adrie\\Desktop\\Master\\S8\\TER\\Instances\\inst5_1.txt";
FileFlowNetwork ffn = new(fileName2);
FlowNetwork flotAvecFile = ffn.Generate();
Console.WriteLine("Flot reconstruit par le fichier : ");
Console.WriteLine(flotAvecFile);

// sw.Reset();
// sw.Start();
// var plValue = (new PL(nf)).Resoudre();
// Console.WriteLine("PL-Solve " + plValue + " in " + sw.Elapsed);
//BenchmarkRunner.Run<Benchmarks>();