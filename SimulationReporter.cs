using System;						// Console
using System.Collections.Generic;	// List<T>

namespace OSPA1 {
	
	/* **** SystemReporter ****************************************************************************************** *
	 *	Description:
	 *		Calculates and displays various statistics about the simulation	
	 * ************************************************************************************************************** */
	public class SimulationReporter {
		
		public List<Job> CompletedJobs;
		public int RejectedJobs;

		private readonly int _startTime;
		private readonly int _endTime;
		
		/* **** SystemReporter Constructor ************************************************************************** *
		 *	Description:
		 *		Builds the reporter
		 * ********************************************************************************************************** */
		public SimulationReporter (int startTime, int endTime) {
			_startTime = startTime;
			_endTime = endTime;
			
			CompletedJobs = new List<Job> ();
		}

		/* **** Report ********************************************************************************************** *
		 *	Description:
		 *		Prints relevant statistics to standard output
		 * ********************************************************************************************************** */
		public void Report (int time, Memory memory) {
			
			// Out of bounds check
			if (time < _startTime || time > _endTime) {
				return;
			}

			// Only print a time label if it will be relevant
			if (time % 200 == 0 || time % 300 == 0 || time % 500 == 0) {
				Console.WriteLine("\t--------------------------------------------------------------");
				Console.Write ("t=" + time + ":");
			}
			
			// a) Turnaround, Wait, and Processing time averages
			if (time == _endTime) {
				int turnaroundAccumulator = 0;
				int waitAccumulator = 0;
				int durationAccumulator = 0;
				
				foreach (Job j in CompletedJobs) {
					turnaroundAccumulator += (j.CompletionTime - j.ArrivalTime);
					waitAccumulator += (j.StartTime - j.ArrivalTime);
					durationAccumulator += (j.CompletionTime - j.StartTime);
				}
				
				Console.WriteLine ("\tAverage Turnaround Time:       " + turnaroundAccumulator / (float)CompletedJobs.Count);
				Console.WriteLine ("\tAverage Wait Time:             " + waitAccumulator       / (float)CompletedJobs.Count);
				Console.WriteLine ("\tAverage Processing Time:       " + durationAccumulator   / (float)CompletedJobs.Count);
			}
				
			// b) Memory utilization in bytes
			if (time % 500 == 0) {
				// Convert from memory slots into "bytes"
				Console.WriteLine ("\tUtilized Memory (in bytes):    " + memory.GetUsedMemorySlots () * 10 + "k");
			}
				
			// c) Hole count and average size
			if (time % 300 == 0) {
				int holeCount;
				float averageHoleSize;
				memory.GetMemoryHoleInfo (out holeCount, out averageHoleSize);
				
				Console.WriteLine ("\tHole Count:                    " + holeCount);
				Console.WriteLine ("\tAverage Hole Size:             " + averageHoleSize);
			}
				
			// d) Job count and average size
			if (time % 200 == 0) {
				int jobCount;
				float averageJobSize;
				memory.GetMemoryJobInfo (out jobCount, out averageJobSize);
				
				Console.WriteLine ("\tJob Count:                     " + jobCount);
				Console.WriteLine ("\tAverage Job Size:              " + averageJobSize);
			}
				
			// e) Rejected job count
			if (time % 1000 == 0) {
				Console.WriteLine ("\tRejected Jobs (last 1000 VTU): " + RejectedJobs);
				// Reset so we only show rejected jobs for the last 1000 VTU
				RejectedJobs = 0;
			}
		}
	}
}