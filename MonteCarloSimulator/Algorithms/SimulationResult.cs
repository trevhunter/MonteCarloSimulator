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
        public ulong TotalSimulations;
        public ulong TotalCollisions;

        internal SimulationResult(ulong totalSimulations, ulong totalCollisions)
        {
            this.TotalSimulations = totalSimulations;
            this.TotalCollisions = totalCollisions;

        }

        
        public static SimulationResult operator +(SimulationResult item1, SimulationResult item2)
        {
            return new SimulationResult(item1.TotalSimulations + item2.TotalSimulations, item1.TotalCollisions + item2.TotalCollisions);
        }

        public decimal CollisionProbability
        {
            get
            {
                return (decimal)TotalCollisions / (decimal)TotalSimulations;
            }
        }
    }
}
