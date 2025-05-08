using FlotMaximum;

namespace Tests;

public class Tests
{
    private readonly Random random = new();
    [SetUp]
    public void Setup()
    {}

    [Test]
    public void Test1()
    {
        for (int i = 0; i < 1000; i++)
        {
            RandomFlowNetwork randomFlow = new(100, 200);
            var nf = randomFlow.Generate();
            var val = nf.FordFulkerson();
            var val2 = nf.EdmondsKarp();
            var val3 = nf.Dinic();
            var val4 = nf.Push_Label();
            Assert.That(val.Value, Is.EqualTo(val2.Value));
            Assert.That(val.Value, Is.EqualTo(val3.Value));
            Assert.That(val.Value, Is.EqualTo(val4.Value));
        }

        Console.WriteLine("Gagné");
    }
    
    [Test]
    public void Test2()
    {
        for (int i = 0; i < 10; i++)
        {
            RandomFlowNetwork randomFlow = new(500, 1000);
            var nf = randomFlow.Generate();
            var val = nf.FordFulkerson();
            var val2 = nf.EdmondsKarp();
            var val3 = nf.Dinic();
            var val4 = nf.Push_Label();
            Assert.That(val.Value, Is.EqualTo(val2.Value));
            Assert.That(val.Value, Is.EqualTo(val3.Value));
            Assert.That(val.Value, Is.EqualTo(val4.Value));
        }
    }
    
    [Test]
    public void Test3()
    {
        for (int i = 0; i < 10000; i++)
        {
            RandomFlowNetwork randomFlow = new(10, 30);
            var nf = randomFlow.Generate();
            var val = nf.FordFulkerson();
            var val2 = nf.EdmondsKarp();
            var val3 = nf.Dinic();
            var val4 = nf.Push_Label();
            Assert.That(val.Value, Is.EqualTo(val2.Value));
            Assert.That(val.Value, Is.EqualTo(val3.Value));
            Assert.That(val.Value, Is.EqualTo(val4.Value));
        }
    }
    
    [Test]
    public void Test4()
    {
        for (int i = 0; i < 1000; i++)
        {
            RandomFlowNetwork randomFlow = new(random.Next(2, 200), random.Next(2, 1000));
            var nf = randomFlow.Generate();
            var val = nf.FordFulkerson();
            var val2 = nf.EdmondsKarp();
            var val3 = PL.SolveWithGurobi(nf);
            var val4 = PL.SolveWithOrTools(nf, "GLOP");
            var val5 = nf.Dinic();
            var val6 = nf.Push_Label();
            Assert.Multiple(() =>
            {
                Assert.That(val.Value, Is.EqualTo(val2.Value));
                Assert.That(val.Value, Is.EqualTo(val5.Value));
                Assert.That(val.Value, Is.EqualTo(val6.Value));
                Assert.That(Math.Abs(val2.Value - val3), Is.LessThan(1e-5));
                Assert.That(Math.Abs(val3 - val4), Is.LessThan(1e-5));
            });
        }
    }
    
    [Test]
    public void ConservationFlot()
    {
        for (int i = 0; i < 100; i++)
        {
            RandomFlowNetwork randomFlow = new(random.Next(2, 5000), random.Next(0, 10000));
            var nf = randomFlow.Generate();
            var val = nf.Dinic();
            foreach (var v in nf.AdjVertices.Keys)
            {
                if (v == nf.Source || v == nf.Puits) continue;
                int flotSortant = nf.AdjVertices[v].Sum(vv => val.FlowEdges[(v, vv)]);

                int flotEntrant = val.FlowEdges.Where(edge => edge.Key.Item2 == v).Sum(edge => edge.Value);
                Assert.That(flotSortant, Is.EqualTo(flotEntrant));
            }
        }
    }
    
    [Test]
    public void ValeurFlot()
    {
        for (int i = 0; i < 100; i++)
        {
            RandomFlowNetwork randomFlow = new(random.Next(2, 5000), random.Next(0, 10000));
            var nf = randomFlow.Generate();
            var val = nf.Dinic();
            var somme1 = val.FlowEdges.Where(edge => edge.Key.Item2 == nf.Puits).Sum(edge => edge.Value);
            var somme2 = val.FlowEdges.Where(edge => edge.Key.Item1 == nf.Source).Sum(edge => edge.Value);
            Assert.That(somme1, Is.EqualTo(somme2));
        }
    }
    
    [Test]
    public void Capacite()
    {
        for (int i = 0; i < 1000; i++)
        {
            RandomFlowNetwork randomFlow = new(random.Next(2, 5000), random.Next(0, 10000));
            var nf = randomFlow.Generate();
            var val = nf.Dinic();
            foreach (var edge in nf.Edges)
            {
                Assert.That(edge.Value, Is.GreaterThanOrEqualTo(val.FlowEdges[edge.Key]));
            }
        }
    }
    
    [Test]
    public void Gurobi()
    {
        for (int i = 0; i < 100; i++)
        {
            RandomFlowNetwork randomFlow = new(random.Next(2, 5000), random.Next(0, 10000));
            var nf = randomFlow.Generate();
            var val = nf.Dinic();
            var guro = PL.SolveWithGurobi(nf);
            Assert.That(Math.Abs(val.Value-guro), Is.LessThan(1e-5));
        }
    }
    [Test]
    public void Scip()
    {
        for (int i = 0; i < 100; i++)
        {
            RandomFlowNetwork randomFlow = new(random.Next(2, 5000), random.Next(0, 10000));
            var nf = randomFlow.Generate();
            var val = nf.Dinic();
            var scip = PL.SolveWithOrTools(nf, "SCIP");
            Assert.That(Math.Abs(val.Value-scip), Is.LessThan(1e-5));
        }
    }
    
}