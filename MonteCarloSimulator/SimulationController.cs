using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            WriteHeader(maxDuration, _dataReader.SourceFile);

            // Get the data to act on
            Clip[] sourceClips = _dataReader.ReadClips();

            if(0 == sourceClips.Length)
            {
                throw new Exception("No data to read.");
            }

            // Determine if we can use a specialized or generic algorithm based on the bit size of 
            // all principals in the clips.
            Func<Algorithms.MonteCarloBase> factory;
            if (sourceClips[0].PrincipalBits.Length==3)
            {
                factory = () =>  new Algorithms.FisherYatesFastCompareSimulator(); 
            }
            else
            {
                factory = () => new Algorithms.FisherYatesInPlaceSimulator();
            }

            // Set up an instance per processor to run
            var simulators = new List<Tuple<Algorithms.MonteCarloBase, Task<Algorithms.SimulationResult>>>(Environment.ProcessorCount);
            DateTime startTime = DateTime.UtcNow;

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var simulator = factory();
                var task = simulator.StartSimulationAsync(sourceClips);
                simulators.Add(new Tuple<Algorithms.MonteCarloBase, Task<Algorithms.SimulationResult>>(simulator, task));
            }

            // add observers for every second
            using (var observer = new SimulationObserver(simulators.Select(i => i.Item1).ToList(), _outputStream, startTime))
            {
                observer.StartObserving();
                Thread.Sleep(maxDuration); // Wait for things to stop
            }

            List<Algorithms.SimulationResult> results = new List<Algorithms.SimulationResult>();
            foreach (var simulator in simulators)
            {
                simulator.Item1.StopSimulation();
                results.Add(simulator.Item2.Result);
            }

            WriteResult(results, (DateTime.UtcNow - startTime), _outputStream);
        }

        private void WriteHeader(TimeSpan maxDuration, string sourceFile)
        {
            _outputStream.WriteLine($"Beginning Monty Carlo Simulation for a maximum of {maxDuration.TotalSeconds} seconds using {Environment.ProcessorCount} processors.");
        }

        private static void WriteResult(List<Algorithms.SimulationResult> results, TimeSpan durationSinceStart, TextWriter outputStream)
        {
            Algorithms.SimulationResult overallResult = new Algorithms.SimulationResult(0, 0);
            foreach (var result in results)
            {
                overallResult += result;
            }

            outputStream.WriteLine(
                $"iterations={overallResult.TotalSimulations:n0}; goodLists={(overallResult.TotalSimulations - overallResult.TotalCollisions):n0}; collision probability={(overallResult.CollisionProbability):P5}; iteration/sec={(overallResult.TotalSimulations / durationSinceStart.TotalSeconds):n0}; total time={durationSinceStart.TotalSeconds:n}s");
        }

        private class SimulationObserver : IDisposable
        {

            private readonly System.Timers.Timer _timer;
            private bool _isDisposed = false;
            private IList<Algorithms.MonteCarloBase> _simulators;
            private TextWriter _outputStream;
            private DateTime _dateStartedUtc;

            public SimulationObserver(IList<Algorithms.MonteCarloBase> observedSimulators, TextWriter outputStream, DateTime dateStartedUtc)
            {
                _simulators = observedSimulators;
                _outputStream = outputStream;
                _dateStartedUtc = dateStartedUtc;
                _timer = new System.Timers.Timer(1000);
                _timer.Elapsed += ElapsedHander;
            }

            public void StartObserving()
            {
                _timer.Start();
            }

            private void ElapsedHander(object sender, System.Timers.ElapsedEventArgs e)
            {
                if (!_isDisposed)
                {
                    WriteCurrentState();
                }
            }

            private void WriteCurrentState()
            {
                var currentResults = from simulator in _simulators
                                     select simulator.CurrentResult;

                SimulationController.WriteResult(currentResults.ToList(), (DateTime.UtcNow - _dateStartedUtc), _outputStream);

            }

            public void Dispose()
            {
                _isDisposed = true;
                _timer.Stop();
                _timer.Dispose();
            }
        }

    }
}