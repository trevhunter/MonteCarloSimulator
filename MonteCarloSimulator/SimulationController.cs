using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MonteCarloSimulator
{
    internal class SimulationController
    {

        private readonly TextWriter _outputStream;
        private readonly SportsClipCsvReader _dataReader;


        public SimulationController(string dataFile, TextWriter outputStream)
        {
            _outputStream = outputStream;
            _dataReader = new SportsClipCsvReader(dataFile);
        }


        public void Run(TimeSpan maxDuration)
        {

            if (maxDuration.TotalSeconds < 1 || maxDuration.TotalHours > 1)
            {
                throw new ArgumentOutOfRangeException("Maximum time must be between 1 second and 1 hour.");
            }

            // Get the data to act on
            Clip[] sourceClips = _dataReader.ReadClips();

            // Set up an instance per processor to run
            var simulators = new List<Tuple<Algorithms.MonteCarloBase, Task<Algorithms.SimulationResult>>>(Environment.ProcessorCount);

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var simulator = new Algorithms.FisherYatesInPlaceSimulator();
                var task = simulator.StartSimulationAsync(sourceClips);
                simulators.Add(new Tuple<Algorithms.MonteCarloBase, Task<Algorithms.SimulationResult>>(simulator, task));
            }

            Thread.Sleep(maxDuration);

            Algorithms.SimulationResult overallResult = new Algorithms.SimulationResult(0, 0);
            foreach (var simulator in simulators)
            {
                simulator.Item1.StopSimulation();
                var result = simulator.Item2.Result;
                overallResult += result;
            }

            _outputStream.WriteLine($"iterations = {overallResult.TotalSimulations:n0}; goodLists = {(overallResult.TotalSimulations - overallResult.TotalCollisions):n0}; collision probability ={((float)overallResult.TotalCollisions / (float)overallResult.TotalSimulations):P5}; iteration/sec = {(overallResult.TotalSimulations / maxDuration.TotalSeconds):n0}");

        }



    }
}