using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonteCarloSimulator.Algorithms
{
    public class FisherYatesInPlaceSimulator : MonteCarloBase
    {
        private readonly Random _rnd = new Random();

        protected override void SimulateMonteCarlo(Clip[] sourceClips, CancellationToken cancelToken)
        {
            // Copy the array as we'll shuffle it
            int[] indicies = Enumerable.Range(0, sourceClips.Length).ToArray();
            int maxRand = sourceClips.Length - 1;

            while (!cancelToken.IsCancellationRequested)
            {
                // Fisher-Yates -like shuffle, with a random indexer rather than 
                // actually moving the items around
                int swapIndex = 0;
                bool isCollision = false;

                // Get a next random clip, then check to see if it clashes with previous clip
                for (int i = 0; i < sourceClips.Length; i++)
                {
                    if (i < maxRand)
                    {
                        swapIndex = _rnd.Next(i + 1, maxRand);
                        int temp = indicies[i];
                        indicies[i] = indicies[swapIndex];
                        indicies[swapIndex] = temp;
                    }

                    if (i > 0)
                    {
                        if (sourceClips[indicies[i]].HasCommonPrincipal(sourceClips[indicies[i - 1]]))
                        {
                            isCollision = true;
                            break;
                        }
                    }
                }
                RecordSimulationResult(isCollision);
            }
        }
    }
}
