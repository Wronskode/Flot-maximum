namespace FlotMaximum;

public class Flow(Dictionary<(Vertex, Vertex), int> flowEdges, Vertex puits)
{
    public Dictionary<(Vertex, Vertex), int> FlowEdges { get; } = flowEdges;

    public int Value
    {
        get
        {
            return FlowEdges.Where(edge => edge.Key.Item2 == puits).Sum(edge => edge.Value);
        }
    }
    public void IncreaseFlow((Vertex, Vertex) edge, int value)
    {
        FlowEdges[edge] += value;
    }
    
    public void DecreaseFlow((Vertex, Vertex) edge, int value)
    {
        FlowEdges[edge] -= value;
    }
}