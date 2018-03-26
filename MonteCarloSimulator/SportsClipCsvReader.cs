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
                             select new Clip()
                             {
                                 Id = clipId++,
                                 Principals = ((string[])c.Principals.Split(',')).Select((i) => int.Parse(i)).ToArray()
                             };


            return clipSource.ToArray();

        }
    }
}
