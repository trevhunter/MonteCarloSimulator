using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonteCarloSimulator.Algorithms
{
    public class FisherYatesShuffleSimulator : MonteCarloBase
    {
        private readonly Random _rnd = new Random();

        protected override void SimulateMonteCarlo(Clip[] sourceClips, CancellationToken cancelToken)
        {

            // Copy the array as we'll shuffle it
            Clip[] clips = (Clip[])sourceClips.Clone();
            int maxRand = clips.Length - 1;

            while (!cancelToken.IsCancellationRequested)
            {

                // Fisher-Yates shuffle
                for (int i = sourceClips.Length - 1; i >=1; i--)
                {
                    int swapIndex = _rnd.Next(0, maxRand);

                    Clip temp = clips[i];
                    clips[i] = clips[swapIndex];
                    clips[swapIndex] = temp;

                }

                bool isCollision = false;
                // Check for clashes
                for (int i = 1; i < clips.Length; i++)
                {
                    if (clips[i].HasCommonPrincipal(clips[i - 1]))
                    {
                        isCollision = true;
                        break;
                    }
                }
                RecordSimulationResult(isCollision);
            }
        }
    }
}
