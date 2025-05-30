namespace FlotMaximum;

public class RandomFlowNetwork
{
    private int VertexNumber;
    private int EdgeNumber;
    private Random Random;
    public double density { get; }
    public RandomFlowNetwork(int vertexNumber, int edgeNumber)
    {
        VertexNumber = vertexNumber;
        EdgeNumber = edgeNumber;
        Random = new Random();
        density = 0;
    }
    
    public RandomFlowNetwork(int vertexNumber, double density)
    {
        if (vertexNumber <= 1) throw new ArgumentOutOfRangeException(nameof(vertexNumber), vertexNumber, "Vertex number must be greater than 1.");
        VertexNumber = vertexNumber;
        EdgeNumber = 0;
        Random = new Random();
        this.density = density;
    }

    public FlowNetwork Generate(int lowerBound = 1, int upperBound = 100)
    {
        Graph graph = new(new List<(Vertex, Vertex, int)>(), []);
        for (int i = 0; i < VertexNumber; i++)
        {
            var v = new Vertex(i.ToString());
            graph.AddVertex(v);
        }
        List<Vertex> vertices = graph.AdjVertices.Keys.ToList();
        int maxEdges = VertexNumber * (VertexNumber - 1);
        if (EdgeNumber > 0)
        {
            int min = Math.Min(EdgeNumber, maxEdges);
            while (graph.Edges.Count < min)
            {
                Vertex v1 = vertices[Random.Next(0, VertexNumber)];
                Vertex v2 = vertices[Random.Next(0, VertexNumber)];
                if (VertexNumber > 1)
                {
                    while (v1 == v2)
                    {
                        v2 = vertices[Random.Next(0, VertexNumber)];
                    }
                }
                graph.AddEdge((v1, v2), Random.Next(lowerBound, upperBound+1));
            }
        }
        else
        {
            foreach (Vertex v1 in vertices)
            {
                foreach (Vertex v2 in vertices)
                {
                    if (v1 != v2)
                    {
                        Random rand = new Random();
                        double randomValue = rand.NextDouble();
                        if (randomValue < density)
                        {
                            graph.AddEdge((v1, v2), Random.Next(lowerBound, upperBound + 1));
                        }
                    }

                }
            }
        }
        Vertex source = vertices[Random.Next(0, VertexNumber)];
        Vertex puits = vertices[Random.Next(0, VertexNumber)];
        if (vertices.Count > 1)
        {
            while (source == puits)
            {
                puits = vertices[Random.Next(0, VertexNumber)];
            }
        }
        List<(Vertex, int)> sourceVerticesList = graph.AdjVertices[source].Select(v => (v, Random.Next(lowerBound, upperBound+1))).ToList();
        List<(Vertex, int)> puitsVerticesList = graph.AdjVertices.Keys.Where(v => graph.AdjVertices[v].Contains(puits)).Select(v => (v, Random.Next(lowerBound, upperBound+1))).ToList();
        graph.RemoveVertex(source);
        graph.RemoveVertex(puits);
        return new FlowNetwork(graph.Edges, source, puits, sourceVerticesList, puitsVerticesList, vertices);
    }
}