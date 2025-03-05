using System.Runtime.Intrinsics;

namespace FlotMaximum;

public class FileFlowNetwork
{
    public String fileName;

    public FileFlowNetwork(String fileName)
    {
        this.fileName = fileName;
    }
    
    public FlowNetwork Generate ()
    {
        Vertex s;
        Vertex p;
        List<Vertex> vertices = new List<Vertex>();
        
        Graph graph = new(new List<(Vertex, Vertex, int)>(), []);
        using (StreamReader reader = new StreamReader(fileName))
        {
            // Lire la source
            s = new Vertex(reader.ReadLine().Trim()); // Source
            // Lire le puits
            p = new Vertex(reader.ReadLine().Trim()); // Puits
            
            vertices.Add(s);
            vertices.Add(p);
            
            // Lire les arêtes et leur poids
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Ignorer les lignes vides
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Séparer le sommet et ses voisins
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    Vertex v1 = vertices.FirstOrDefault(v => v.Id == parts[0]);
                    Vertex v2 = vertices.FirstOrDefault(v => v.Id == parts[1]);
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
        int VertexNumber = graph.AdjVertices.Count;
        Random r = new Random();
        List < (Vertex, int) > sourceVerticesList = graph.neighborsRight(s);
        List<(Vertex, int)> puitsVerticesList = graph.neighborsLeft(p);
        return new FlowNetwork(graph.Edges, s, p, vertices);;
    }
    
}