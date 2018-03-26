using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloSimulator.Algorithms
{
    /// <summary>
    /// Holds the result of a MonteCarlo simulation run for sports clip collisions
    /// </summary>
    public struct SimulationResult
    {
        public int TotalSimulations;
        public int TotalCollisions;

        internal SimulationResult(int totalSimulations, int totalCollisions)
        {
            this.TotalSimulations = totalSimulations;
            this.TotalCollisions = totalCollisions;

        }

        
        public static SimulationResult operator +(SimulationResult item1, SimulationResult item2)
        {
            return new SimulationResult(item1.TotalSimulations + item2.TotalSimulations, item1.TotalCollisions + item2.TotalCollisions);
        }

        public double CollisionProbability
        {
            get
            {
                return (double)TotalCollisions / (double)TotalSimulations;
            }
        }
    }
}
