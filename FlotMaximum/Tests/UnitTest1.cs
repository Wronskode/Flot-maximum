using FlotMaximum;

namespace Tests;

public class Tests
{
    Random random = new();
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
            Assert.That(val.Value, Is.EqualTo(val2.Value));
        }
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
            Assert.That(val.Value, Is.EqualTo(val2.Value));
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
            Assert.That(val.Value, Is.EqualTo(val2.Value));
        }
    }
    
    [Test]
    public void ConservationFlot()
    {
        for (int i = 0; i < 100; i++)
        {
            RandomFlowNetwork randomFlow = new(random.Next(2, 5000), random.Next(0, 10000));
            var nf = randomFlow.Generate();
            var val = nf.EdmondsKarp();
            foreach (var v in nf.AdjVertices.Keys)
            {
                if (v == nf.Source || v == nf.Puits) continue;
                int flotSortant = 0;
                foreach (var vv in nf.AdjVertices[v])
                {
                    flotSortant += val.FlowEdges[(v, vv)];
                }

                int flotEntrant = 0;
                foreach (var edge in val.FlowEdges)
                {
                    if (edge.Key.Item2 == v)
                       flotEntrant += edge.Value; 
                }
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
            var val = nf.EdmondsKarp();
            var somme1 = val.FlowEdges.Where(edge => edge.Key.Item2 == nf.Puits).Sum(edge => edge.Value);
            var somme2 = val.FlowEdges.Where(edge => edge.Key.Item1 == nf.Source).Sum(edge => edge.Value);
            Assert.That(somme1, Is.EqualTo(somme2));
        }
    }
    
}