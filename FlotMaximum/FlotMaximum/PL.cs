using Google.OrTools.LinearSolver;
using Gurobi;

namespace FlotMaximum;

public class PL
{
    public static double SolveWithOrTools(FlowNetwork flowNetwork, string solverName)
    {
        Solver solver = Solver.CreateSolver(solverName);
        Dictionary<(Vertex, Vertex), Variable> variablesDic = new Dictionary<(Vertex, Vertex), Variable>();
        foreach (var edge in flowNetwork.Edges)
        {
            var (u, v) = edge.Key; // u et v sont les sommets
            Variable var = solver.MakeNumVar(0.0, double.PositiveInfinity, $"x_{u.ToString()}_{v.ToString()}");
            variablesDic[(u, v)] = var;
        }
        
        foreach (var edge in flowNetwork.Edges)
        {
            var (u, v) = edge.Key; // u et v sont les sommets
            int poids = edge.Value;
            Constraint constraint = solver.MakeConstraint(0, poids);
            constraint.SetCoefficient(variablesDic[(u, v)], 1);
        }
        
        foreach (Vertex v in flowNetwork.AdjVertices.Keys)
        {
            if (v == flowNetwork.Puits || v == flowNetwork.Source)
            {
                continue;
            }
    
            Constraint constraint = solver.MakeConstraint(0, 0);

            if (flowNetwork.InEdges.TryGetValue(v, out var entrant))
            {
                foreach (Vertex a in entrant)
                {
                    if (variablesDic.TryGetValue((a, v), out var varAv))
                    {
                        constraint.SetCoefficient(varAv, 1);
                    }
                }
            }
    
            var sortant = flowNetwork.GetSortant(v);
            foreach (Vertex b in sortant)
            {
                if (variablesDic.TryGetValue((v, b), out var varVb))
                {
                    constraint.SetCoefficient(varVb, -1);
                }
            }
        }
        
        Objective objectif = solver.Objective();
        
        foreach (Vertex a in flowNetwork.GetSortant(flowNetwork.Source))
        {
            objectif.SetCoefficient(variablesDic[(flowNetwork.Source,a)], 1);
        }
        objectif.SetMaximization();
        solver.Solve();
        return solver.Objective().Value();
    }
    
    
    public void AfficherSysteme(Solver solver)
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

    public static double SolveWithGurobi(FlowNetwork nf)
    {
        GRBEnv env = new GRBEnv(true);
        env.Set("OutputFlag", "0");
        env.Start();
        GRBModel model = new GRBModel(env);
        Dictionary<(Vertex, Vertex), GRBVar> vars = new();
        foreach (var edge in nf.Edges)
        {
            vars[(edge.Key.Item1, edge.Key.Item2)] = model.AddVar(0.0, edge.Value, 0.0, GRB.CONTINUOUS, $"x{edge.Key.Item1}_{edge.Key.Item2}");
        }
        foreach (var vertex in nf.AdjVertices)
        {
            if (vertex.Key == nf.Source || vertex.Key == nf.Puits) continue;
            GRBLinExpr lexpr = new GRBLinExpr(0);
            foreach (var v in vertex.Value) 
            {
                lexpr.AddTerm(1.0, vars[(vertex.Key, v)]);
            }
            if (nf.InEdges.TryGetValue(vertex.Key, out var edge))
                foreach (var v in edge)
                {
                    lexpr.AddTerm(-1.0, vars[(v, vertex.Key)]);
                }
            model.AddConstr(lexpr, GRB.EQUAL, 0.0, "c");
        }

        GRBLinExpr expr = new GRBLinExpr(0);
        foreach (var v in nf.AdjVertices[nf.Source])
        {
            expr.AddTerm(1.0, vars[(nf.Source, v)]);
        }
        model.SetObjective(expr, GRB.MAXIMIZE);
        model.Optimize();
        return model.ObjVal;
    }
}