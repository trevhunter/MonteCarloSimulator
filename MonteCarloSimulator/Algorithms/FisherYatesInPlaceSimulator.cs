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
            // Use an a array of indicies so we don't need to shuffle the actual clips
            // Also make a copy of the clips so processors don't need to share cache (don't know if this is a thing or not)
            Clip[] localClips = (Clip[])sourceClips.Clone();
            int[] indicies = Enumerable.Range(0, localClips.Length).ToArray();
            int maxIndex = localClips.Length - 1;

            while (!cancelToken.IsCancellationRequested)
            {
                // Fisher-Yates -like shuffle, with a random indexer rather than 
                // actually moving the items around
                int swapIndex = 0;
                bool isCollision = false;

                // Get a next random clip, then check to see if it clashes with previous clip
                for (int i = 0; i <= maxIndex; i++)
                {
                    if (i < maxIndex)
                    {
                        // only swap things above the current position into the current position
                        // because if we swapped things lower than the current position, we'd
                        // invalidate the checks that determine if the previous clips have a 
                        // collision.
                        swapIndex = _rnd.Next(i + 1, maxIndex); 
                        int temp = indicies[i];
                        indicies[i] = indicies[swapIndex];
                        indicies[swapIndex] = temp;
                    }

                    if (i > 0)
                    {
                        if (localClips[indicies[i]].HasCommonPrincipal(localClips[indicies[i - 1]]))
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
