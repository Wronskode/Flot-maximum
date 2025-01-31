using Google.OrTools.LinearSolver;
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
                Console.WriteLine($"{variables[i].Name()} = {variables[i].SolutionValue()}");
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
        int nbContraintes = flowNetwork.Edges.Count*2;
        variables = new Variable[nbVariables];
        constraints = new Constraint[nbContraintes];
        

        int var_i = 0;
        foreach (var edge in flowNetwork.Edges)
        {
            var (u, v) = edge.Key; // u et v sont les sommets
            int poids = edge.Value;
            variables[var_i] = solver.MakeNumVar(0.0, double.PositiveInfinity, $"x_{u.ToString()}_{v.ToString()}");
            var_i += 1;
        }
        
        foreach (var edge in flowNetwork.Edges)
        {
            var (u, v) = edge.Key; // u et v sont les sommets
            int poids = edge.Value;
            constraints[i]= solver.MakeConstraint(double.NegativeInfinity, flowNetwork.Edges);
            var_i += 1;
        }
        
        for (int i = 0; i < nbVariables; i++)
        {
            constraints[i]= solver.MakeConstraint(double.NegativeInfinity, flowNetwork.Edges);
            Constraint constraint = solver.MakeConstraint(3, 3);
        }
        
    }


}