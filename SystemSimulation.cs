using System;						// Random
using System.Collections.Generic;	// List<T>, Queue<T>

namespace OSPA1 {
	
	/* **** SystemSimulation **************************************************************************************** *
	 *	Description:
	 *		Runs and records a simulation	
	 * ************************************************************************************************************** */
	public class SystemSimulation {
		
		// RNGesus
		private readonly Random _random;
		
		private readonly Memory _memory;
		private readonly Func<Memory, Job, MemoryBlock> _placementStrategy;
		private readonly Queue<Job> _jobQueue;
		private readonly SimulationReporter _simulationReporter;
		
		private Job _currentJob;

		/* **** SystemSimulation Constructor ************************************************************************ *
		 *	Description:
		 *		Builds the simulation object and initializes required components
		 * ********************************************************************************************************** */
		public SystemSimulation (Func<Memory, Job, MemoryBlock> placementStrategy) {
			
			// Auto-seeded to something related to current system time.
			_random = new Random ();
			
			// Initialized to 175 "spaces":
			// 2000k bytes - 250k bytes for OS = 1750k bytes
			// 750k bytes / 10k job size "resolution" = 175 memory slots
			_memory = new Memory (175);
			_placementStrategy = placementStrategy;
			_jobQueue = new Queue<Job> ();
			
			Console.Write("\r" + _memory.ToString());
			System.Threading.Thread.Sleep(1);
			// The limits of our reporting for this simulation
			//_simulationReporter = new SimulationReporter (1000, 5000);
		}
		
		/* **** Simulate ******************************************************************************************** *
		 *	Description:
		 *		Runs the simulation. (Main loop)
		 * ********************************************************************************************************** */
		public void Simulate (int duration) {
			
			int timeToNextJob = 0;
			for (int time = 0; time <= duration; time++) {
				
				// Call job generator and reset generation countdown timer
				if (timeToNextJob-- == 0) {
					GenerateNewJob (time);
					timeToNextJob = _random.Next (1, 11);
				}

				// Process jobs
				ProcessRunningJob (time, _jobQueue, _memory);
				
				// Output when necessary
				_simulationReporter.Report (time, _memory);
			}
		}

		/* **** GenerateNewJob ************************************************************************************** *
		 *	Description:
		 *		Generates a new job and requests it be allocated to memory
		 * ********************************************************************************************************** */
		public void GenerateNewJob (int time) {
			
			Job job = new Job (time, _random.Next(5,31), _random.Next(5, 61), time);
			// Try to allocate to memory
			if (_memory.Allocate (job, _placementStrategy)) {
				_jobQueue.Enqueue (job);
			}
			else {
				_simulationReporter.RejectedJobs++;
			}
		}

		/* **** ProcessRunningJob *********************************************************************************** *
		 *	Description:
		 *		Processes the current running job by decreasing its durability. If there is no current running job,
		 *		attemps to pull one from the job queue.
		 * ********************************************************************************************************** */
		private void ProcessRunningJob (int time, Queue<Job> jobQueue, Memory memory) {
			
			// If we're currently working on a job
			if (_currentJob != null) {
				// Do some processing
				_currentJob.Duration--;

				// If the current job is completed
				if (_currentJob.Duration == 0) {
					
					// Free it from memory
					memory.Free (_currentJob);
					_currentJob.CompletionTime = time;
					_simulationReporter.CompletedJobs.Add (_currentJob);
					_currentJob = null;
				}
			} else {
				// If there are jobs waiting to be worked
				if (jobQueue.Count > 0) {
					
					// Grab one and do some processing
					_currentJob = jobQueue.Dequeue ();
					_currentJob.StartTime = time;
					_currentJob.Duration--;
				}
			}
		}
	}
}