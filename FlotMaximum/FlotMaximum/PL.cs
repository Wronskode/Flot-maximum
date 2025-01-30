using Google.OrTools.LinearSolver;
using OperationsResearch;

namespace FlotMaximum;

public class PL
{
    public Solver solver;
    public Variable[] variables;
    public Int32[] contraintes;

    public PL(Int32 nbVariables, Int32[][] A, String symbol, Int32[] B)
    {
        variables = new Variable[nbVariables];
        for (int i = 0; i < nbVariables; i++)
        {
            // Création de la variable x_i avec borne inférieure 0 et borne supérieure infinie
            variables[i] = solver.MakeNumVar(0.0, double.PositiveInfinity, $"x{i + 1}");
        }

    }

}