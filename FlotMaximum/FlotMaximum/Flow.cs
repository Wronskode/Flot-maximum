namespace FlotMaximum;

public class Flow
{
    public Dictionary<(Vertex, Vertex), int> FlowEdges { get; }
    private Vertex puits;

    public Flow(Dictionary<(Vertex, Vertex), int> flowEdges, Vertex puits)
    {
       FlowEdges = new(flowEdges); 
       this.puits = puits;
    }
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