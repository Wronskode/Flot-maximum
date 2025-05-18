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
        VertexNumber = vertexNumber;
        EdgeNumber = 0;
        Random = new Random();
        this.density = density;
    }

    public FlowNetwork Generate()
    {
        Graph graph = new(new List<(Vertex, Vertex, int)>(), []);
        for (int i = 0; i < VertexNumber; i++)
        {
            var v = new Vertex(i.ToString());
            graph.AddVertex(v);
        }
        List<Vertex> vertices = graph.AdjVertices.Keys.ToList();
        
        if (EdgeNumber > 0)
        {
            int min = Math.Min(EdgeNumber, VertexNumber*(VertexNumber + 1)/2);
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
                graph.AddEdge((v1, v2), Random.Next(0, EdgeNumber));
            }
        }
        else
        {
            int edges = (int) (density * (VertexNumber * (VertexNumber - 1)));
            foreach (Vertex v1 in graph.AdjVertices.Keys.ToList())
            {
                foreach (Vertex v2 in graph.AdjVertices.Keys.ToList())
                {
                    if (v1 != v2)
                    {
                        Random rand = new Random();
                        double randomValue = rand.NextDouble();
                        if (randomValue < density)
                        {
                            graph.AddEdge((v1, v2), Random.Next(0, edges));
                        }
                    }
                    
                }
            }
            //int edges = (int) (density * (VertexNumber * (VertexNumber + 1) / 2));
            //while (graph.Edges.Count < edges)
            //{
            //    Vertex v = vertices[Random.Next(0, VertexNumber)];
            //    Vertex u = vertices[Random.Next(0, VertexNumber)];
            //    if (VertexNumber > 1)
            //    {
            //        while (v == u)
            //        {
            //            u = vertices[Random.Next(0, VertexNumber)];
            //        }
            //    }
            //    graph.AddEdge((u, v), Random.Next(0, edges));
            //}
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
        List<(Vertex, int)> sourceVerticesList = graph.AdjVertices[source].Select(v => (v, Random.Next(0, VertexNumber))).ToList();
        List<(Vertex, int)> puitsVerticesList = graph.AdjVertices.Keys.Where(v => graph.AdjVertices[v].Contains(puits)).Select(v => (v, Random.Next(0, VertexNumber))).ToList();
        graph.RemoveVertex(source);
        graph.RemoveVertex(puits);
        return new FlowNetwork(graph.Edges, source, puits, sourceVerticesList, puitsVerticesList, vertices);
    }
}