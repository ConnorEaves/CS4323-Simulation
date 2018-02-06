using System.Collections.Generic;

namespace OSPA2 {
/* **** ProcessScheduler ******************************************** *
 *  Description:
 *      Round robin scheduler that keeps track of currently loaded jobs
 *      and time quantum state.
 *
 *  Critques:
 *      Circular Memory reference is clunky, but it works
 * ***************************************************************** */
    public class ProcessScheduler {
        private readonly List<Job> _processes;
        private int _currentJobIndex;

        private readonly int _timeQuantum;
        private int _timeQuantumCounter;

        public Memory Memory;
        
/* **** ProcessScheduler Constructor ******************************** *
 *  Description:
 *      Initialization
 * ***************************************************************** */
        public ProcessScheduler (int timeQuantum, Memory memory) {
            _timeQuantum = timeQuantum;
            _timeQuantumCounter = 0;
            
            _processes = new List<Job> ();
            _currentJobIndex = 0;

            Memory = memory;
        }

/* **** Process ***************************************************** *
 *  Description:
 *      Cycles through the processes in a round robin fashion. Returns
 *      a job when it completes, null otherwise.
 * ***************************************************************** */
        public Job Process (int time) {
            // Early out if we have no processes to work
            if (_processes.Count == 0) {
                return null;
            }
            
            // Progress time quantum
            _timeQuantumCounter++;
    
            Job currentJob = _processes[_currentJobIndex];
            // If we're just starting this job, set its start time
            if (currentJob.Duration == currentJob.CpuBurst) {
                currentJob.StartTime = time;
            }
            
            // If this job is complete
            if (currentJob.Duration == 0) {
                // Free it
                currentJob.CompletionTime = time;
                Memory.Free (currentJob);
                _processes.Remove (currentJob);
                
                // Reset the quantum
                _timeQuantumCounter = 0;
                
                // Progress to the next one
                _currentJobIndex = _currentJobIndex %
                                       _processes.Count;
                return currentJob;
            }

            // Make progress on current job
            currentJob.Duration--;

            // If the time quantum has expired
            if (_timeQuantumCounter == _timeQuantum) {
                // Move to next job and reset quantum counter
                if (_processes.Count > 0) {
                    _currentJobIndex = (_currentJobIndex + 1) %
                                           _processes.Count;
                } else {
                    // If there is nothing to do, magic number
                    _currentJobIndex = 0;
                }
                
                // Reset quantum
                _timeQuantumCounter = 0;
            }
            return null;
        }

/* **** Add ********************************************************* *
 *  Description:
 *      Inserts a job at the end of the round robin list such that all
 *      existing jobs will have a turn before the newly entered job
 * ***************************************************************** */
        public void Add (Job job) {
            // Nasty ternary, but its shorter
            _processes.Insert (
                _processes.Count == 0
                    ? _currentJobIndex
                    : _currentJobIndex++, job);
        }
    }
}