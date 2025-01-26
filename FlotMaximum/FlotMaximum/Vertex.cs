namespace FlotMaximum;

public class Vertex : ICloneable
{
    private string Id { get; set; }

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