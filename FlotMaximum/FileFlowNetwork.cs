namespace FlotMaximum;

public class FileFlowNetwork
{
    private readonly string FileName;
    private static readonly char[] Separator = [' '];

    public FileFlowNetwork(string fileName)
    {
        FileName = fileName;
    }
    
    public FlowNetwork Generate ()
    {
        Vertex s;
        Vertex p;
        List<Vertex> vertices = [];
        
        Graph graph = new(new List<(Vertex, Vertex, int)>(), []);
        using (var reader = new StreamReader(FileName))
        {
            // Lire la source
            s = new Vertex(reader.ReadLine().Trim()); // Source
            // Lire le puits
            p = new Vertex(reader.ReadLine().Trim()); // Puits
            
            vertices.Add(s);
            vertices.Add(p);
            
            // Lire les arêtes et leur poids
            while (reader.ReadLine() is { } line)
            {
                // Ignorer les lignes vides
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Séparer le sommet et ses voisins
                var parts = line.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    Vertex? v1 = vertices.FirstOrDefault(v => v.Id == parts[0]);
                    Vertex? v2 = vertices.FirstOrDefault(v => v.Id == parts[1]);
                    if (v1 == null)
                    {
                        v1 = new Vertex(parts[0]);
                        vertices.Add(v1);
                    }
                    if (v2 == null)
                    {
                        v2 = new Vertex(parts[1]);
                        vertices.Add(v2);
                    }
                    int weight = int.Parse(parts[2]);
                    // Ajouter l'arête avec son poids
                    graph.AddEdge((v1, v2), weight);
                }
            }
        }
            
        Console.WriteLine("Graph successfully loaded from file.");
        return new FlowNetwork(graph.Edges, s, p, vertices);
    }
    
}