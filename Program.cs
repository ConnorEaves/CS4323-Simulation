using System;		// Console

namespace OSPA1 {
	
	/* **** Program ************************************************************************************************* *
	 *	Author:
	 * 		Connor Eaves
	 *
	 *	Course:
	 * 		CS 4323 | 64361 Des & Impl Oper Sys I
	 *
	 *	Assignment:
	 * 		Simulation Project, Phase I
	 *
	 *	Date:
	 * 		10 / 23 / 2017
	 *
	 * 	Global Variables:
	 * 		None
	 *
	 *	Description:
	 *		Entry point
	 *
	 *	Critques:
	 *		
	 * ************************************************************************************************************** */
	internal class Program {
		
		/* **** Main ************************************************************************************************ *
		 *	Description:
		 *		Entry point
		 * ********************************************************************************************************** */
		public static void Main (string[] args) {
			
			SystemSimulation simulation;
			
			Console.WriteLine("First-Fit Simulation:");
		
			// Use First-Fit placement strategy
			simulation = new SystemSimulation (Memory.GetBestMemoryBlock);
			simulation.Simulate (6000);
			
			Console.WriteLine("\nBest-Fit Simulation");
			// Use Best-Fit placement strategy
			simulation = new SystemSimulation (Memory.GetBestMemoryBlock);
			simulation.Simulate (6000);
			 
			Console.WriteLine ("\nWorst-Fit Simulation:");
			// Use Worst-Fit placement strategy
			simulation = new SystemSimulation (Memory.GetWorstMemoryBlock);
			simulation.Simulate (6000);
		}
	}
}