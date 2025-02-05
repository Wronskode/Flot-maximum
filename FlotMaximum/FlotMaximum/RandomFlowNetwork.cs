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
        Random = new Random();
    }

    public FlowNetwork Generate()
    {
        Graph graph = new(new List<(Vertex, Vertex, int)>(), []);
        HashSet<Vertex> sourceVertices = [];
        HashSet<Vertex> puitsVertices = [];
        for (int i = 0; i < VertexNumber; i++)
        {
            var v = new Vertex(i.ToString());
            graph.AddVertex(v);
        }
        List<Vertex> vertices = graph.AdjVertices.Keys.ToList();
        for (int i = 0; i < EdgeNumber; i++)
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
            graph.AddEdge((v1, v2), Random.Next(0, EdgeNumber));
        }
        
        // Je mets un nombre d'arêtes au pif entre la source/puits et le reste du graphe (environ sqrt(n))
        for (int i = 0; i < Math.Floor(Math.Sqrt(VertexNumber))+1; i++)
        {
            Vertex v1 = vertices[Random.Next(0, VertexNumber)];
            Vertex v2 = vertices[Random.Next(0, VertexNumber)];
            sourceVertices.Add(v1);
            puitsVertices.Add(v2);
        }
        List<(Vertex, int)> sourceVerticesList = new(sourceVertices.Select(v => (v, Random.Next(0, EdgeNumber))));
        List<(Vertex, int)> puitsVerticesList = new(puitsVertices.Select(v => (v, Random.Next(0, EdgeNumber))));
        return new FlowNetwork(graph.Edges, new Vertex("s"), new Vertex("p"), sourceVerticesList, puitsVerticesList, vertices);
    }
}