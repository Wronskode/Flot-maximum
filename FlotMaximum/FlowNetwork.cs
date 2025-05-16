namespace FlotMaximum;
using System.Text;


public class FlowNetwork : Graph
{
    public Vertex Source { get; }
    public Vertex Puits { get; }
    public List<(Vertex, int)> SourceNeighbors { get; }
    public List<(Vertex, int)> PuitsNeighbors { get; }
    
    public Dictionary<Vertex, HashSet<Vertex>> InEdges { get; } = new();

    public FlowNetwork(IEnumerable<(Vertex, Vertex, int Value)> neighbors, Vertex source, Vertex puits, IEnumerable<(Vertex, int)> sourceNeighbors, IEnumerable<(Vertex, int)> puitsNeighbors, IEnumerable<Vertex> vertices) : base(neighbors, vertices)
    {
        Source = source;
        Puits = puits;
        SourceNeighbors = sourceNeighbors.ToList();
        PuitsNeighbors = puitsNeighbors.ToList();
        InitFlowNetwork();
    }
    
    public FlowNetwork(Dictionary<(Vertex, Vertex), int> neighbors, Vertex source, Vertex puits, IEnumerable<
        (Vertex, int)> sourceNeighbors, IEnumerable<(Vertex, int)> puitsNeighbors, IEnumerable<Vertex> vertices) : base(neighbors, vertices)
    {
        Source = source;
        Puits = puits;
        SourceNeighbors = sourceNeighbors.ToList();
        PuitsNeighbors = puitsNeighbors.ToList();
        InitFlowNetwork();
    }
    
    public FlowNetwork (Dictionary<(Vertex, Vertex), int> neighbors, Vertex source, Vertex puits, IEnumerable<Vertex> vertices) : base(neighbors, vertices)
    {
        Source = source;
        Puits = puits;
        SourceNeighbors = NeighborsRight(Source);
        PuitsNeighbors = NeighborsLeft(Puits);
        InitFlowNetwork();
    }

    private void InitFlowNetwork()
    {
        Dictionary<(Vertex, Vertex), int> newEdges = new(Edges);
        Dictionary<Vertex, HashSet<Vertex>> newAdjVertices = [];
        foreach (var vertex in AdjVertices)
        {
            newAdjVertices.Add(vertex.Key, vertex.Value);
        }
        newAdjVertices.TryAdd(Source, []);
        newAdjVertices.TryAdd(Puits, []);
        foreach (var (vertex, value) in SourceNeighbors)
        {
            newEdges.TryAdd((Source, vertex), value);
            newAdjVertices[Source].Add(vertex);
        }
        
        foreach (var (vertex, value) in PuitsNeighbors)
        {
            newEdges.TryAdd((vertex, Puits), value);
            newAdjVertices[vertex].Add(Puits);
        }

        Edges = newEdges;
        AdjVertices = newAdjVertices;
        
        foreach (var edge in Edges)
        {
            var u = edge.Key.Item1;
            var v = edge.Key.Item2;
            if (!InEdges.ContainsKey(v))
                InEdges[v] = [];
            InEdges[v].Add(u);
        }
    }

    public Flow FordFulkerson()
    {
        return GetMaxFlow(nf => nf.CheminDfs());
    }
    
    public Flow EdmondsKarp()
    {
        return GetMaxFlow(nf => nf.CheminBfs());
    }
    public Flow GetMaxFlow(Func<FlowNetwork, List<Vertex>> getPath)
    {
        Flow flot = new(Edges.ToDictionary(edge => edge.Key, _ => 0), Puits);
        FlowNetwork nf = GetResidualNetwork(flot);
        List<Vertex> chemin = getPath(nf);

        while (chemin.Count > 0)
        {
            int delta = int.MaxValue;
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var edge = (chemin[i], chemin[i + 1]);

                if (nf.Edges.TryGetValue(edge, out int capacity))
                {
                    delta = Math.Min(delta, capacity);
                }
            }
            
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var forwardEdge = (chemin[i], chemin[i + 1]);
                var backwardEdge = (chemin[i + 1], chemin[i]);

                if (Edges.ContainsKey(forwardEdge))
                {
                    flot.IncreaseFlow(forwardEdge, delta);
                }
                // else if (flot.FlowEdges.ContainsKey(backwardEdge))
                // {
                //     flot.DecreaseFlow(backwardEdge, delta);
                // }
                else
                {
                    flot.DecreaseFlow(backwardEdge, delta);
                }
            }
            
            nf = GetResidualNetwork(flot);
            chemin = getPath(nf);
        }
        return flot;
    }
    
    public Flow Dinic()
{
    Flow flot = new Flow(Edges.ToDictionary(edge => edge.Key, _ => 0), Puits);
    while (true)
    {
        FlowNetwork nf = GetResidualNetwork(flot);
        Dictionary<Vertex, int> levels = nf.BFS_GetLevels();
        if (!levels.ContainsKey(Puits))
        {
            break;
        }
        while (true)
        {
            List<Vertex> chemin = DFS_GetPathInLevelGraph(levels, nf);
            
            if (chemin.Count == 0)
            {
                break;
            }
            int delta = int.MaxValue;
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var edge = (chemin[i], chemin[i + 1]);
                if (nf.Edges.TryGetValue(edge, out int capacity))
                {
                    delta = Math.Min(delta, capacity);
                }
                else
                {
                    delta = 0;
                    Console.Error.WriteLine("Erreur: Arête du chemin non trouvée dans le graphe résiduel pendant le calcul de delta.");
                    break;
                }
            }
            if (delta == 0) { 
                Console.Error.WriteLine("Erreur: Delta calculé à 0, arrêt de la phase de flot bloquant.");
                break;
            }
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var u = chemin[i];
                var v = chemin[i + 1];
                var forwardEdge = (u, v);
                var backwardEdge = (v, u);
                
                if (Edges.ContainsKey(forwardEdge))
                {
                    flot.IncreaseFlow(forwardEdge, delta);
                }
                else
                {
                    flot.DecreaseFlow(backwardEdge, delta);
                }
            }
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var u = chemin[i];
                var v = chemin[i + 1];
                var forwardEdge = (u, v);
                var backwardEdge = (v, u);
                nf.Edges[forwardEdge] -= delta;
                if (nf.Edges.TryAdd(backwardEdge, 0))
                {
                    if (nf.AdjVertices.TryGetValue(v, out HashSet<Vertex>? value)) { value.Add(u); } else { nf.AdjVertices[v] = [u]; }
                }
                nf.Edges[backwardEdge] += delta;
            }
        }

    } 
    return flot;
}
    
    private List<Vertex> DFS_GetPathInLevelGraph(Dictionary<Vertex, int> levels, FlowNetwork currentResidualNetwork)
{
    Stack<(Vertex current, List<Vertex> path)> stack = new();
    HashSet<Vertex> visitedOnPath = new HashSet<Vertex>();
    
    var initialPath = new List<Vertex> { Source };
    stack.Push((Source, initialPath));
    visitedOnPath.Add(Source);
    while (stack.Count > 0)
    {
        var (u, currentPath) = stack.Pop();
        if (u.Equals(Puits))
        {
            return currentPath;
        }
        foreach (var v in currentResidualNetwork.AdjVertices[u])
        {
            if (currentResidualNetwork.Edges[(u, v)] <= 0) continue;
            if (levels.TryGetValue(v, out int value) && value == levels[u] + 1 && !visitedOnPath.Contains(v))
            {
                var newPath = new List<Vertex>(currentPath) { v };
                stack.Push((v, newPath));
                visitedOnPath.Add(v);
            }
        }
    }
    return new List<Vertex>();
}
    
    public Dictionary<Vertex, int> BFS_GetLevels()
    {
        Dictionary<Vertex, int> levels = new Dictionary<Vertex, int>();
        Queue<Vertex> queue = new Queue<Vertex>();

        levels[Source] = 0;
        queue.Enqueue(Source);

        while (queue.Count > 0)
        {
            Vertex u = queue.Dequeue();
            
            if (AdjVertices.TryGetValue(u, out var neighbors))
            {
                foreach (Vertex v in neighbors)
                {
                    if (!levels.ContainsKey(v))
                    {
                        levels[v] = levels[u] + 1;
                        queue.Enqueue(v);
                    }
                }
            }
        }
        return levels;
    }

    public FlowNetwork GetResidualNetwork(Flow  flot)
    {
        Dictionary<(Vertex, Vertex), int> newEdgesNf = [];
        foreach (var edge in Edges)
        {
            int capacity = edge.Value;
            int currentFlow = flot.FlowEdges[(edge.Key.Item1, edge.Key.Item2)];
            int remainFlow = capacity - currentFlow;
            if (!newEdgesNf.TryAdd((edge.Key.Item1, edge.Key.Item2), remainFlow))
            {
                newEdgesNf[(edge.Key.Item1, edge.Key.Item2)] += remainFlow;
            }
            if (currentFlow == 0 || edge.Key.Item2 == Puits || edge.Key.Item1 == Source) continue;
            var reverseEdge = (edge.Key.Item2, edge.Key.Item1);
            if (!newEdgesNf.TryAdd(reverseEdge, currentFlow))
            {
                newEdgesNf[reverseEdge] += currentFlow;
            }
        }
        newEdgesNf = newEdgesNf.Where(x => x.Value > 0).ToDictionary();
        var residuel = new FlowNetwork(newEdgesNf, Source, Puits, [], [], AdjVertices.Keys);
        return residuel;
    }

    public int GetResidualEdge(Flow flot, (Vertex, Vertex) edge)
    {
        return Edges[edge] - flot.FlowEdges[(edge.Item1, edge.Item2)];
    }
    
    // Trouver chemin avec parcours en profondeur (Ford-Fulkerson)
    public List<Vertex> CheminDfs()
    {
        Stack<Vertex> pile = [];
        Dictionary<Vertex, Vertex?> parent = [];
        HashSet<Vertex> marked = [];

        pile.Push(Source);
        marked.Add(Source);
        parent[Source] = null;

        while (pile.Count > 0)
        {
            var s = pile.Pop();
            if (s == Puits)
            {
                List<Vertex> path = [];
                for (var v = s; v != null; v = parent[v])
                {
                    path.Add(v);
                }
                path.Reverse();
                return path;
            }

            foreach (var t in AdjVertices[s])
            {
                if (!marked.Contains(t) &&
                    Edges.TryGetValue((s, t), out int capacity) && capacity > 0)
                {
                    pile.Push(t);
                    marked.Add(t);
                    parent[t] = s;
                }
            }
        }
        return [];
    }
    
    // Edmonds-Karp
    public List<Vertex> CheminBfs()
    {
        Queue<Vertex> queue = [];
        Dictionary<Vertex, Vertex?> parents = [];
    
        queue.Enqueue(Source);
        parents[Source] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current == Puits)
            {
                Stack<Vertex> stack = [];
                for (var v = current; v != null; v = parents[v])
                {
                    stack.Push(v);
                }
                return [..stack];
            }

            foreach (var t in AdjVertices[current])
            {
                if (!parents.ContainsKey(t) &&
                    Edges.TryGetValue((current, t), out int capacity) && capacity > 0)
                {
                    queue.Enqueue(t);
                    parents[t] = current;
                }
            }
        }
        return [];
    }
    
    public override object Clone()
    {
        return new FlowNetwork(Edges.Where(x => x.Value != 0).ToDictionary(),
            Source, Puits, 
            SourceNeighbors.Select(x => x), PuitsNeighbors.Select(x => x),
            AdjVertices.Keys.Select(x => x));
    }
    
    public List<Vertex> GetEntrant (Vertex vertex)
    {
        List<Vertex> entrant = new();
        foreach (var edge in Edges)
        {
            var (u, v) = edge.Key;
            if (v == vertex)
            {
                entrant.Add(u);
            }
        }

        return entrant;
    }

    public HashSet<Vertex> GetSortant(Vertex vertex)
    {
        return AdjVertices[vertex];
    }
    
    
    public bool IsConnected() {
        if (AdjVertices.Count == 0) return true; // Un graphe vide est considéré comme connexe

        var startVertex = Source;
        var visited = new HashSet<Vertex>();
        var queue = new Queue<Vertex>();

        // On démarre le BFS à partir du premier sommet
        queue.Enqueue(startVertex);
        visited.Add(startVertex);

        while (queue.Count > 0) {
            var vertex = queue.Dequeue();
            if (AdjVertices.TryGetValue(vertex, out var neighbors)) {
                foreach (var neighbor in neighbors) {
                    if (visited.Add(neighbor)) {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        // Vérifier si tous les sommets ont été visités
        if (visited.Count != AdjVertices.Count)
        {
            //Console.WriteLine("Source non connectée à tout les sommets");
            //Console.WriteLine("Exemple : [{0}]", string.Join(", ", unvisited.Select(v => v.ToString())));
            return false;
        }
        
        var goToPuits = new HashSet<Vertex> { Puits };
        foreach (Vertex v in AdjVertices.Keys.ToList())
        {
            bool check = false;
            startVertex = v;
            visited = new HashSet<Vertex>();
            queue = new Queue<Vertex>();

            queue.Enqueue(startVertex);
            visited.Add(startVertex);
            
            while (queue.Count > 0) {
                var vertex = queue.Dequeue();
                if (goToPuits.Contains(vertex))
                {
                    check = true;
                }
                if (AdjVertices.TryGetValue(vertex, out var neighbors)) {
                    foreach (var neighbor in neighbors) {
                        if (visited.Add(neighbor)) {
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            if (check == false)
            {
                Console.WriteLine(startVertex+" non connecté au puits");
                return false;
            }
        }
        
        return true;
    }

    
    public override string ToString()
    {
        StringBuilder output = new();
        output.Append("source : "+Source+", puits : "+Puits+"\n");
        foreach (Vertex vertex in AdjVertices.Keys)
        {
            var special = "";
            StringBuilder txt = new();
            foreach (var edge in Edges)
            {
                special = "";
                if (edge.Key.Item2 == vertex)
                {
                    if (edge.Key.Item1 == Puits)
                    {
                        special = "(p)";
                    }
                    if (edge.Key.Item1 == Source)
                    {
                        special = "(s)";
                    }
                    txt.Append(edge.Key.Item1 + special + ",");
                }
            }
            if (vertex == Puits)
            {
                special = "(p)";
            }
            if (vertex == Source)
            {
                special = "(s)";
            }

            if (txt.Length == 0)
            {
                continue;
            }
            txt.Length -= 1;
            txt.Append(" -> " + vertex+special);
            output.Append(txt + "\n");
        }
        return output.ToString();
    }

    public void CreateGraphFile(string filePath)
    {
        // Ouvre ou crée le fichier en écriture
        using StreamWriter writer = new StreamWriter(filePath);
        // Écrit la source
        writer.WriteLine(Source);

        // Écrit le puit
        writer.WriteLine(Puits);

        // Écrit les sommets et leurs voisins
        foreach (var vertex in AdjVertices.Keys)
        {
            if (vertex == Puits)
            {
                continue;
            }
            writer.Write($"{vertex}: "); // Affiche le sommet suivi de ": "

            // Récupère les voisins du sommet
            var neighbors = AdjVertices[vertex];

            // Si le sommet a des voisins, les afficher séparés par des virgules
            // Si le sommet n'a pas de voisins, afficher "aucun"
            writer.WriteLine(neighbors.Count > 0 ? string.Join(", ", neighbors) : "aucun");
        }
    }
    
    public void CreateGraphWeightFile(string filePath)
    {
        // Ouvre ou crée le fichier en écriture
        using StreamWriter writer = new StreamWriter(filePath);
        // Écrit la source
        writer.WriteLine(Source);

        // Écrit le puit
        writer.WriteLine(Puits);

        // Écrit les sommets et leurs voisins
        foreach (var edge in Edges)
        {
                
            var (v1, v2) = edge.Key;
            var weight = edge.Value;

            // Affiche "v1 v2 p" si l'arête existe
            writer.WriteLine($"{v1} {v2} {weight}");
        }
    }
    // Poussage-réétiquatage
    public void ReLabel(Vertex u, Dictionary<Vertex, int> hauteur, Flow flot)
    {
        int m = AdjVertices.Count;
        foreach (var t in AdjVertices[u])
        {
            if (GetResidualEdge(flot, (u, t)) > 0)
            {
                m = Math.Min(hauteur[t], m);
            }
        }

        if (u != Puits)
        {
            hauteur[u] = m + 1;
        }
    }
    
    public void Push(Vertex u, Vertex v, Dictionary<Vertex, int> excedent, Flow flot)
    {
        var m = Math.Min(GetResidualEdge(flot, (u, v)), excedent[u]);
        if (m == 0) return;
        excedent[u] -= m;
        excedent[v] += m;
        flot.IncreaseFlow((u,v),m);
        flot.DecreaseFlow((v,u),m);
    }

    public Flow Push_Label()
    {
        // Initialisation
        Dictionary<Vertex, int> hauteur = new();
        Dictionary<Vertex, int> excedent = new();
        Queue<Vertex> sommetActifs = [];
        foreach (var s in AdjVertices.Keys)
        {
            hauteur.Add(s,0);
            excedent.Add(s,0);
            if (s == Source)
            {
                hauteur[Source]=AdjVertices.Count;
            }
            //if (s != Source && s != Puits)
            {
                foreach (var u in AdjVertices[s])
                {
                    if (!AdjVertices[u].Contains(s))
                    {
                        AddEdge((u,s), 0);
                    }
                }
            }
        }
        Flow flot = new(Edges.ToDictionary(edge => edge.Key, _ => 0), Puits);
        foreach (var s in AdjVertices[Source])
        {
            excedent[s] = Edges[(Source,s)];
            sommetActifs.Enqueue(s);
            flot.IncreaseFlow((Source,s),Edges[(Source,s)]);
        }
        
        // Corps de la fonction
        HashSet<Vertex> enFile =
        [
            Source,
            Puits
        ];
        int compteur = AdjVertices.Count * AdjVertices.Count;
        while (sommetActifs.Count > 0 && compteur > 0)
        {
            var s = sommetActifs.Dequeue();
            if (excedent[s] > 0)
            {
                ReLabel(s, hauteur, flot);
                if (enFile.Add(s))
                {
                    sommetActifs.Enqueue(s);
                }
            }
            foreach (var t in AdjVertices[s])
            {
                ReLabel(t, hauteur, flot);
                if (hauteur[s] - hauteur[t] == 1 && GetResidualEdge(flot, (s, t))>0)
                {
                    Push(s,t, excedent, flot);
                    if (excedent[t] > 0 && enFile.Add(t))
                    {
                        sommetActifs.Enqueue(t);
                    }
                }
            }
            if (excedent[s] > 0)
            {
                ReLabel(s, hauteur, flot);
                if (enFile.Add(s))
                {
                    sommetActifs.Enqueue(s);
                }
            }

            if (sommetActifs.Count == 0)
            {
                compteur--;
                foreach (var t in AdjVertices.Keys)
                {
                    if (excedent[t] > 0 && hauteur[t] <AdjVertices.Count && t != Puits)
                    {
                        sommetActifs.Enqueue(t);
                    }
                }
            }
        }
        return flot;
    }
}