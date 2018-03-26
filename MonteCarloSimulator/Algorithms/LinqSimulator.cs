using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonteCarloSimulator.Algorithms
{
    /// <summary>
    /// Simulates monte carlo algorithm using 
    /// a linq query over the list of clips to achieve randomness
    /// </summary>
    public class LinqSimulator : MonteCarloBase
    {
        private readonly Random _rnd = new Random();

        protected override void SimulateMonteCarlo(Clip[] sourceClips, CancellationToken cancelToken)
        {
            while(!cancelToken.IsCancellationRequested)
            {
                // Get a random sequence of clips. 
                // This is an incredibly bad and dangerous way of doing this
                var movieSequence = from clip in sourceClips
                                    orderby _rnd.Next()
                                    select clip;

                Clip prevClip = new Clip() { Id = -1, Principals = new int[0] } ;
                bool isCollision = false;
                foreach(var clip in movieSequence)
                {
                    if(clip.HasCommonPrincipal(prevClip))
                    {
                        isCollision = true;
                        break;
                    }
                    prevClip = clip;
                }
                RecordSimulationResult(isCollision);
            }
        }
    }
}
