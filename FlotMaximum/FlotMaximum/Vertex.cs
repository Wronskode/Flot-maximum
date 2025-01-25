namespace FlotMaximum;

public class Vertex : ICloneable
{
    public HashSet<Vertex> Neighbors { get; set; } = new();
    public string id;
    public Vertex(HashSet<Vertex> neighbors, string id)
    {
        Neighbors = neighbors;
        this.id = id;
    }

    public Vertex(string id)
    {
        this.id = id;
    }

    public override string ToString()
    {
        return id.ToString();
    }

    public object Clone()
    {
        return new Vertex(Neighbors, id);
    }
}