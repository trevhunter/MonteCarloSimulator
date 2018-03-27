using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloSimulator.Algorithms
{
    public abstract class MonteCarloBase
    {
        private System.Threading.CancellationTokenSource _cancelTokenSource;


        private SimulationResult _result;
        public SimulationResult CurrentResult
        {
            get
            {
                return _result;
            }
        }

        public async Task<SimulationResult> StartSimulationAsync(Clip[] sourceClips)
        {
            // Error if running
            if (null != _cancelTokenSource)
            {
                throw new InvalidOperationException("Simulation already running. Please stop before restarting.");
            }

            _cancelTokenSource = new System.Threading.CancellationTokenSource();
            _result = new SimulationResult();

            return await Task.Run<SimulationResult>(() =>
            {
                SimulateMonteCarlo(sourceClips, _cancelTokenSource.Token);
                return this.CurrentResult;
            },
            _cancelTokenSource.Token);
        }

        public void StopSimulation()
        {
            _cancelTokenSource.Cancel();
            _cancelTokenSource.Token.WaitHandle.WaitOne();
            _cancelTokenSource = null;
        }
        
        public void RecordSimulationResult(bool wasCollision)
        {
            _result.TotalSimulations++;
            if(wasCollision)
            {
                _result.TotalCollisions++;
            }
        }


        protected abstract void SimulateMonteCarlo(Clip[] sourceClips, System.Threading.CancellationToken cancelToken);
    }
}
