using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdArgs = Args.Configuration.Configure<CmdArgs>().CreateAndBind(args);

            // Set up the simulation controller
            var controller = new SimulationController(cmdArgs.DataFilePath, Console.Out);
            controller.Run(TimeSpan.FromSeconds(cmdArgs.MaxTime));

            // Prompt to exit
            Console.WriteLine("Press return to exit...");
            Console.ReadLine();
        }


        public class CmdArgs
        {
           
            public CmdArgs()
            {
                MaxTime = 5;
                DataFilePath = @"Lab 08 - Monte Carlo\RealDataSets\Data-Set-1.csv";
            }

            [Description("The max time (in seconds) the simulation will run for. Must be between 1 and 3600.")]
            public int MaxTime { get; set; }

            [Description("The data file to use for simulating over. Must be a csv in form of 'clip,[principal1,principal2...]'.")]
            public string DataFilePath { get; set; }

        }
    }
}
