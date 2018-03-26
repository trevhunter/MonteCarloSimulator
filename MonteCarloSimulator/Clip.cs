using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloSimulator
{
    /// <summary>
    /// Represents a single clip with one or more principal
    /// </summary>
    public struct Clip
    {
        public int Id { get; set; }

        public int[] Principals { get; set; }

        /// <summary>
        /// Checks to see if this clip has a principal in common with another principal
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCommonPrincipal(Clip otherClip)
        {
            for (int i = 0; i < this.Principals.Length; i++)
            {
                for (int j = 0; j < otherClip.Principals.Length; j++)
                {
                    if (this.Principals[i] == otherClip.Principals[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
