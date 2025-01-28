namespace FlotMaximum;

public class Vertex : ICloneable
{
    public string Id { get; }

    public Vertex(string id)
    {
        Id = id;
    }

    public override string ToString()
    {
        return Id;
    }
    public object Clone()
    {
        return new Vertex(Id);
    }
}