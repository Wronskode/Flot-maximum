using Google.OrTools.LinearSolver;
using Google.Protobuf.WellKnownTypes;
using OperationsResearch;

namespace FlotMaximum;

public class PL
{
    public Solver solver;
    public Variable[] variables;
    public Constraint[] constraints;

    public PL(int nbVariables, List<List<int>> A, List<String> symbol, List<int> B, List<int> obj, bool maximiser)
    {
        solver = Solver.CreateSolver("SCIP");
        
        variables = new Variable[nbVariables];
        int nbContrainte = B.Count;
        constraints = new Constraint[nbContrainte];
        for (int i = 0; i < nbVariables; i++)
        {
            // Création de la variable x_i avec borne inférieure 0 et borne supérieure infinie
            variables[i] = solver.MakeNumVar(0.0, double.PositiveInfinity, $"x{i + 1}");
        }
        
        for (int i = 0; i < nbContrainte; i++)
        {
            if (symbol[i] == "<=")
            {
                constraints[i]= solver.MakeConstraint(double.NegativeInfinity, B[i]);
            }
            if (symbol[i] == "=")
            {
                constraints[i] = solver.MakeConstraint(B[i], B[i]);
            }
            if (symbol[i] == ">=")
            {
                constraints[i] = solver.MakeConstraint(B[i], double.PositiveInfinity);
            }
            
            for (int j = 0; j < nbVariables; j++)
            {
                constraints[i].SetCoefficient(variables[j], A[i][j]);
            }
        }
        
        Objective objectif = solver.Objective();
        for (int i = 0; i < variables.Length; i++)
        {
            objectif.SetCoefficient(variables[i], obj[i]);
        }
        if (maximiser)
        {
            objectif.SetMaximization();
        }
        else
        {
            objectif.SetMinimization();
        }
    }
    
    public void Resoudre()
    {
        Solver.ResultStatus statut = solver.Solve();

        if (statut == Solver.ResultStatus.OPTIMAL)
        {
            Console.WriteLine("Solution optimale trouvée !");
            for (int i = 0; i < variables.Length; i++)
            {
                //Console.WriteLine($"{variables[i].Name()} = {variables[i].SolutionValue()}");
            }
            Console.WriteLine($"Valeur optimale de la fonction objectif : {solver.Objective().Value()}");
        }
        else
        {
            Console.WriteLine("Pas de solution optimale trouvée.");
        }
    }

    public PL(FlowNetwork flowNetwork)
    {
        solver = Solver.CreateSolver("SCIP");
        
        int nbVariables = flowNetwork.Edges.Count;
        int nbContraintes = nbVariables+flowNetwork.AdjVertices.Count;
        variables = new Variable[nbVariables];
        constraints = new Constraint[nbContraintes];
        
        Dictionary<(Vertex, Vertex), Variable> variablesDic = new Dictionary<(Vertex, Vertex), Variable>();
        

        int var_i = 0;
        foreach (var edge in flowNetwork.Edges)
        {
            var (u, v) = edge.Key; // u et v sont les sommets
            int poids = edge.Value;
            Variable var = solver.MakeNumVar(0.0, double.PositiveInfinity, $"x_{u.ToString()}_{v.ToString()}");
            variablesDic[(u, v)] = var;
            variables[var_i] = var;
            var_i += 1;
        }
        
        var_i = 0;
        foreach (var edge in flowNetwork.Edges)
        {
            var (u, v) = edge.Key; // u et v sont les sommets
            int poids = edge.Value;
            Constraint constraint = solver.MakeConstraint(0, poids);
            constraint.SetCoefficient(variablesDic[(u, v)], 1);
            constraints[var_i] = constraint;
            var_i += 1;
        }
        
        foreach (Vertex v in flowNetwork.AdjVertices.Keys)
        {
            if (v == flowNetwork.Puits || v == flowNetwork.Source)
            {
                continue;
            }
            List<Vertex> entrant = flowNetwork.getEntrant(v);
            List<Vertex> sortant = flowNetwork.getSortant(v);

            // Création de la contrainte somme x_av - somme x_vb = 0
            Constraint constraint = solver.MakeConstraint(0, 0); 

            // Ajout des flux entrants (x_av, a ∈ entrant) avec coefficient +1
            foreach (Vertex a in entrant)
            {
                constraint.SetCoefficient(variablesDic[(a, v)], 1);
            }

            // Ajout des flux sortants (x_vb, b ∈ sortant) avec coefficient -1
            foreach (Vertex b in sortant)
            {
                    constraint.SetCoefficient(variablesDic[(v, b)], -1);
            }
            
            constraints[var_i] = constraint;
            var_i += 1;
        }
        
        Objective objectif = solver.Objective();
        
        foreach (Vertex a in flowNetwork.getSortant(flowNetwork.Source))
        {
            objectif.SetCoefficient(variablesDic[(flowNetwork.Source,a)], 1);
        }
        objectif.SetMaximization();
        
    }
    
    
    public void AfficherSysteme()
    {
        Console.WriteLine("=== Système de Programmation Linéaire ===\n");

        // Affichage des variables
        Console.WriteLine("Variables :");
        for (int i = 0; i < solver.NumVariables(); i++)
        {
            Variable var = solver.Variable(i);
            Console.WriteLine($"  {var.Name()} ∈ [{var.Lb()}, {var.Ub()}]");
        }
        Console.WriteLine();

        // Affichage des contraintes
        Console.WriteLine("Contraintes :");
        for (int i = 0; i < solver.NumConstraints(); i++)
        {
            Constraint constraint = solver.Constraint(i);
            List<string> termes = new List<string>();

            for (int j = 0; j < solver.NumVariables(); j++)
            {
                Variable var = solver.Variable(j);
                double coeff = constraint.GetCoefficient(var);
                if (coeff != 0)
                {
                    termes.Add($"{(coeff >= 0 ? "+" : "")}{coeff} * {var.Name()}");
                }
            }

            // Construction de l'affichage de la contrainte
            string contrainteStr = string.Join(" ", termes);
            Console.WriteLine($"  {contrainteStr} ∈ [{constraint.Lb()}, {constraint.Ub()}]");
        }
        Console.WriteLine();

        // Affichage de la fonction objectif
        Console.WriteLine("Fonction Objectif :");
        List<string> objTermes = new List<string>();

        for (int i = 0; i < solver.NumVariables(); i++)
        {
            Variable var = solver.Variable(i);
            double coeff = solver.Objective().GetCoefficient(var);
            if (coeff != 0)
            {
                objTermes.Add($"{(coeff >= 0 ? "+" : "")}{coeff} * {var.Name()}");
            }
        }

        string objStr = string.Join(" ", objTermes);
        string sens = solver.Objective().Maximization() ? "MAX" : "MIN";
        Console.WriteLine($"  {sens} {objStr}");

        Console.WriteLine("\n========================================\n");
    }


}