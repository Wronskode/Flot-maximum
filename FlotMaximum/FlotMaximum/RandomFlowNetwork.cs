namespace FlotMaximum;

public class RandomFlowNetwork
{
    private int VertexNumber;
    private int EdgeNumber;
    private Random Random;
    public RandomFlowNetwork(int vertexNumber, int edgeNumber)
    {
        VertexNumber = vertexNumber;
        EdgeNumber = edgeNumber;
        Random = new();
        
    }

    public FlowNetwork Generate()
    {
        Graph graph = new(new List<(Vertex, Vertex, int)>(), []);
        HashSet<Vertex> sourceVertices = new();
        HashSet<Vertex> puitsVertices = new();
        for (int i = 0; i < VertexNumber; i++)
        {
            graph.AddVertex(new Vertex(i.ToString()));
        }

        for (int i = 0; i < EdgeNumber; i++)
        {
            Vertex v1 = graph.Vertices[Random.Next(0, VertexNumber)];
            Vertex v2 = graph.Vertices[Random.Next(0, VertexNumber)];
            while (v1 == v2)
            {
                v2 = graph.Vertices[Random.Next(0, VertexNumber)];
            }
            graph.AddEdge((v1, v2), Random.Next(0, EdgeNumber));
        }

        for (int i = 0; i < Math.Floor(Math.Sqrt(VertexNumber))+1; i++)
        {
            Vertex v1 = graph.Vertices[Random.Next(0, VertexNumber)];
            Vertex v2 = graph.Vertices[Random.Next(0, VertexNumber)];
            sourceVertices.Add(v1);
            puitsVertices.Add(v2);
        }
        List<(Vertex, int)> sourceVerticesList = new(sourceVertices.Select(v => (v, Random.Next(0, EdgeNumber))));
        List<(Vertex, int)> puitsVerticesList = new(puitsVertices.Select(v => (v, Random.Next(0, EdgeNumber))));
        var edgesList = graph.Edges.Select(v => (v.Key.Item1, v.Key.Item2, v.Value));
        return new FlowNetwork(edgesList, new Vertex("s"), new Vertex("p"), sourceVerticesList, puitsVerticesList, graph.Vertices);
    }
}