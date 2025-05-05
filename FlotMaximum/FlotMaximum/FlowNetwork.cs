namespace FlotMaximum;
using System.Text;


public class FlowNetwork : Graph
{
    public Vertex Source { get; }
    public Vertex Puits { get; }
    public List<(Vertex?, int)> SourceNeighbors { get; }
    public List<(Vertex?, int)> PuitsNeighbors { get; }
    
    public Dictionary<Vertex, HashSet<Vertex>> InEdges { get; } = new();

    public FlowNetwork(IEnumerable<(Vertex?, Vertex?, int Value)> neighbors, Vertex source, Vertex puits, IEnumerable<(Vertex?, int)> sourceNeighbors, IEnumerable<(Vertex?, int)> puitsNeighbors, IEnumerable<Vertex> vertices) : base(neighbors, vertices)
    {
        Source = source;
        Puits = puits;
        SourceNeighbors = sourceNeighbors.ToList();
        PuitsNeighbors = puitsNeighbors.ToList();
        InitFlowNetwork();
    }
    
    public FlowNetwork(Dictionary<(Vertex, Vertex), int> neighbors, Vertex source, Vertex puits, IEnumerable<
        (Vertex?, int)> sourceNeighbors, IEnumerable<(Vertex?, int)> puitsNeighbors, IEnumerable<Vertex> vertices) : base(neighbors, vertices)
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
        SourceNeighbors = this.neighborsRight(Source);
        PuitsNeighbors = this.neighborsLeft(Puits);
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
            if (vertex is null) continue;
            newEdges.TryAdd((Source, vertex), value);
            newAdjVertices[Source].Add(vertex);
        }
        
        foreach (var (vertex, value) in PuitsNeighbors)
        {
            if (vertex is null) continue;
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
            HashSet<Vertex> test = new();
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
                else if (flot.FlowEdges.ContainsKey(backwardEdge))
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
                else if (Edges.ContainsKey(backwardEdge))
                {
                    flot.DecreaseFlow(backwardEdge, delta);
                }
                else
                {
                    Console.Error.WriteLine($"Erreur: Ni l'arête {forwardEdge} ni l'arête inverse {backwardEdge} n'existent dans le graphe original.");
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
                    if (nf.AdjVertices.TryGetValue(v, out HashSet<Vertex>? value)) { value.Add(u); } else { nf.AdjVertices[v] = new HashSet<Vertex>{ u }; }
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
        foreach (var edge in currentResidualNetwork.Edges.Where(kvp => kvp.Key.Item1.Equals(u) && kvp.Value > 0))
        {
            Vertex v = edge.Key.Item2;
            if (levels.TryGetValue(v, out int value) && value == levels[u] + 1 && !visitedOnPath.Contains(v))
            {
                var newPath = new List<Vertex>(currentPath);
                newPath.Add(v);
                stack.Push((v, newPath));
                visitedOnPath.Add(v);
                 if (levels.TryGetValue(v, out int value2) && value2 == levels[u] + 1 && !currentPath.Contains(v))
                 {
                    var newPath2 = new List<Vertex>(currentPath);
                    newPath2.Add(v);
                    stack.Push((v, newPath2));
                 }
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
        return new FlowNetwork(Edges.Select(x => (x.Key.Item1.Clone() as Vertex, x.Key.Item2.Clone() as Vertex, x.Value)),
            Source.Clone() as Vertex ?? throw new InvalidOperationException(), Puits.Clone() as Vertex ?? throw new InvalidOperationException(), 
            SourceNeighbors.Select(x => (x.Item1?.Clone() as Vertex, x.Item2)), PuitsNeighbors.Select(x => (x.Item1?.Clone() as Vertex, x.Item2)),
            AdjVertices.Keys);
    }
    
    public List<Vertex> GetEntrant (Vertex vertex)
    {
        List<Vertex> entrant = new();
        foreach (var edge in this.Edges)
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
        return this.AdjVertices[vertex];
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
                    if (!visited.Contains(neighbor)) {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        // Vérifier si tous les sommets ont été visités
        if (visited.Count != AdjVertices.Count)
        {
            var allVertices = AdjVertices.Keys.ToList();  // Récupère tous les sommets du graphe
            var unvisited = allVertices.Except(visited).ToList();  // Récupère ceux qui ne sont pas dans 'visited'
            //Console.WriteLine("Source non connectée à tout les sommets");
            //Console.WriteLine("Exemple : [{0}]", string.Join(", ", unvisited.Select(v => v.ToString())));
            return false;
        };
        
        var goToPuits = new HashSet<Vertex>();
        goToPuits.Add(Puits);
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
                        if (!visited.Contains(neighbor)) {
                            visited.Add(neighbor);
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
        String special = "";
        StringBuilder output = new();
        output.Append("source : "+Source+", puits : "+Puits+"\n");
        foreach (Vertex vertex in AdjVertices.Keys)
        {
            special = "";
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
        using (StreamWriter writer = new StreamWriter(filePath))
        {
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
                if (neighbors.Count > 0)
                {
                    writer.WriteLine(string.Join(", ", neighbors));
                }
                else
                {
                    // Si le sommet n'a pas de voisins, afficher "aucun"
                    writer.WriteLine("aucun");
                }
            }
        }
    }
    
    public void CreateGraphWeightFile(string filePath)
    {
        // Ouvre ou crée le fichier en écriture
        using (StreamWriter writer = new StreamWriter(filePath))
        {
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
    }
    // Poussage-réétiquatage
    public void ReLabel(Vertex u, Dictionary<Vertex, int> hauteur, Flow flot)
    {
        int m = AdjVertices.Count;;
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
        Queue<Vertex> sommet_actifs = [];
        foreach (var (s,t) in AdjVertices)
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
                    if (! AdjVertices[u].Contains(s))
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
            sommet_actifs.Enqueue(s);
            flot.IncreaseFlow((Source,s),Edges[(Source,s)]);
        }
        
        // Corps de la fonction
        HashSet<Vertex> enFile = new();
        enFile.Add(Source);
        enFile.Add(Puits);
        int compteur = AdjVertices.Count * AdjVertices.Count;
        while (sommet_actifs.Count > 0 && compteur > 0)
        {
            var s = sommet_actifs.Dequeue();
            if (excedent[s] > 0)
            {
                ReLabel(s, hauteur, flot);
                if (enFile.Add(s))
                {
                    sommet_actifs.Enqueue(s);
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
                        sommet_actifs.Enqueue(t);
                    }
                }
            }
            if (excedent[s] > 0)
            {
                ReLabel(s, hauteur, flot);
                if (enFile.Add(s))
                {
                    sommet_actifs.Enqueue(s);
                }
            }

            if (sommet_actifs.Count == 0)
            {
                compteur--;
                foreach (var t in AdjVertices)
                {
                    if (excedent[t.Key] > 0 && hauteur[t.Key] <AdjVertices.Count && t.Key != Puits)
                    {
                        sommet_actifs.Enqueue(t.Key);
                    }
                }
            }
        }
        return flot;
    }
}