using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloSimulator
{
    /// <summary>
    /// Reads sports clip data from a csv file
    /// </summary>
    public class SportsClipCsvReader
    {
        private readonly string _sourceFile;

        public string SourceFile
        {
            get
            {
                return _sourceFile;
            }
        }

        public SportsClipCsvReader(string dataFile)
        {
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException($"Cannot fine file '{dataFile}'");
            }
            _sourceFile = dataFile;
        }


        public Clip[] ReadClips()
        {

            int clipId = 0;

            var config = new CsvHelper.Configuration.Configuration() { HasHeaderRecord = false };
            var reader = new CsvHelper.CsvReader(System.IO.File.OpenText(_sourceFile), config, false);
            var anonType = new { Name = "", Principals = "" };
            var clipSource = from c in reader.GetRecords(anonType)
                             let principalArray = ((string[])c.Principals.Split(',')).Select((i) => int.Parse(i)).ToArray()
                             select new Clip()
                             {
                                 Id = clipId++,
                                 Principals = principalArray
                             };

            var clipArray = clipSource.ToArray();

            int maxPrincipalId = clipArray.SelectMany(c => c.Principals).Max();

            for (int i = 0; i < clipArray.Length; i++)
            {
                clipArray[i].PrincipalBits = GetPrincipalBits(clipArray[i].Principals, maxPrincipalId);
            }

            return clipArray;

        }

        private Int64[] GetPrincipalBits(int[] principals, int maxPrincipalId)
        {
            // The storage we need is 1 bit per item. So if the max principal id = 200, we need at least 200 bits
            int numElements = (int)Math.Ceiling(((double)maxPrincipalId / 64));
            //System.Collections.BitArray bits = new System.Collections.BitArray(maxPrincipalId-1);
            Int64[] bits = new Int64[numElements];

            for (int i = 0; i < principals.Length; i++)
            {
                int maskIndex = ((int)Math.Ceiling((double)principals[i] / 64)) - 1;
                Int64 bitMask = (Int64)Math.Pow(2, (principals[i] % 64)-1);
                bits[maskIndex] = bits[maskIndex] | bitMask;
            }
            return bits;
        }
    }
}
