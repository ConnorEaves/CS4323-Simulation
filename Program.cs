using System;		// Console

namespace OSPA2 {
    
/* **** Program ***************************************************** *
 *  Author:
 *      Connor Eaves
 *
 *  Course:
 *      CS 4323 | 64361 Des & Impl Oper Sys I
 *
 *  Assignment:
 *      Simulation Project, Phase II
 *
 *  Date:
 *      11 / 23 / 2017
 *
 *  Global Variables:
 *      None
 *
 *  Description:
 *      Entry point
 *
 *  Critques:
 *      
 * ***************************************************************** */
    internal class Program {
        
/* **** Main ******************************************************** *
 *  Description:
 *      Entry point
 * ***************************************************************** */
        public static void Main (string[] args) {
            
            SystemSimulation simulation;
            
            // *** First Fit ***
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(
                "First-Fit Simulation (Compact every 250 VTU):");
            simulation =
                new SystemSimulation (
                    Memory.GetFirstMemoryBlock,
                    CompactionScenario.Every250VTU,
                    // Only show backing store on the first simulation
                    showBackstore: true); 
            simulation.Simulate (6000);
            
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(
                "First-Fit Simulation (Compact every 360 VTU):");
            simulation =
                new SystemSimulation (
                    Memory.GetFirstMemoryBlock,
                    CompactionScenario.Every360VTU);
            simulation.Simulate (6000);
            
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(
                "First-Fit Simulation (Compact on memory allocation failure):");
            simulation =
                new SystemSimulation (
                    Memory.GetFirstMemoryBlock,
                    CompactionScenario.OnMemRequestDenied);
            simulation.Simulate (6000);
            
            
            // *** Best Fit *** 
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(
                "Best-Fit Simulation (Compact every 250 VTU):");
            simulation =
                new SystemSimulation (
                    Memory.GetBestMemoryBlock,
                    CompactionScenario.Every250VTU);
            simulation.Simulate (6000);
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(
                "Best-Fit Simulation (Compact every 360 VTU):");
            simulation =
                new SystemSimulation (
                    Memory.GetBestMemoryBlock,
                    CompactionScenario.Every360VTU);
            simulation.Simulate (6000);
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(
                "Best-Fit Simulation (Compact on memory allocation failure):");
            simulation =
                new SystemSimulation (
                    Memory.GetBestMemoryBlock,
                    CompactionScenario.OnMemRequestDenied);
            simulation.Simulate (6000);
            
            
            // *** Worst Fit ***
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(
                "Worst-Fit Simulation (Compact every 250 VTU):");
            simulation =
                new SystemSimulation (
                    Memory.GetWorstMemoryBlock,
                    CompactionScenario.Every250VTU);
            simulation.Simulate (6000);
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(
                "Worst-Fit Simulation (Compact every 360 VTU):");
            simulation =
                new SystemSimulation (
                    Memory.GetWorstMemoryBlock,
                    CompactionScenario.Every360VTU);
            simulation.Simulate (6000);
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(
                "Worst-Fit Simulation (Compact on memory allocation failure):");
            simulation =
                new SystemSimulation (
                    Memory.GetWorstMemoryBlock,
                    CompactionScenario.OnMemRequestDenied);
            simulation.Simulate (6000);
        }
    }
}