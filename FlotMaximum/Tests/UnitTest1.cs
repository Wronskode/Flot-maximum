using FlotMaximum;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        for (int i = 0; i < 1000; i++)
        {
            RandomFlowNetwork randomFlow = new(100, 200);
            var nf = randomFlow.Generate();
            var val = nf.FordFulkerson();
            var val2 = nf.EdmondsKarp();
            Assert.That(val.Item2, Is.EqualTo(val2.Item2));
        }
    }
}