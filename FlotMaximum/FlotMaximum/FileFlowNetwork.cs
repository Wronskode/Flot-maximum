namespace FlotMaximum;

public class FileFlowNetwork
{
    public String fileName;

    public FileFlowNetwork(String fileName)
    {
        this.fileName = fileName;
    }
    
    public Graph Generate ()
    {
        Graph graph = new(new List<(Vertex, Vertex, int)>(), []);
        graph.AdjVertices = new Dictionary<Vertex, HashSet<Vertex>>();
        graph.Edges = new Dictionary<(Vertex, Vertex), int>();

        try
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                // Lire la source
                Vertex source = new Vertex(reader.ReadLine().Trim()); // Source

                // Lire le puits
                Vertex puits = new Vertex(reader.ReadLine().Trim()); // Puits

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
                        Vertex v1 = new Vertex(parts[0]);
                        Vertex v2 = new Vertex(parts[1]);
                        int weight = int.Parse(parts[2]);

                        // Ajouter l'arête avec son poids
                        graph.AddEdge((v1, v2), weight);
                    }
                }
            }

            Console.WriteLine("Graph successfully loaded from file.");
            return new FlowNetwork(graph.Edges, source, puits, sourceVerticesList, puitsVerticesList, vertices);;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading graph from file: {ex.Message}");
        }
    }
    
}