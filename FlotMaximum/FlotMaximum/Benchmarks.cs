using System.Text;
using BenchmarkDotNet.Attributes;

namespace FlotMaximum;

[MemoryDiagnoser]
public class Benchmarks
{
    [Params(5, 50, 500)]
    public static int n { get; set; }
    
    [Params(5, 50, 1000)]
    public static int m { get; set; }
    

    [Benchmark]
    public Flow EdmondsKarp()
    { 
        RandomFlowNetwork randomFlow = new(n, m);
        FlowNetwork nf = randomFlow.Generate();
        return nf.EdmondsKarp();
    }

    [Benchmark]
    public double Gurobi()
    {
        RandomFlowNetwork randomFlow = new(n, m);
        FlowNetwork nf = randomFlow.Generate();
        return PL.SolveWithGurobi(nf);
    }
}