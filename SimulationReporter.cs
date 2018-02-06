using System;                        // Console
using System.Collections.Generic;    // List<T>

namespace OSPA2 {
    
/* **** SystemReporter ********************************************** *
 *  Description:
 *      Calculates and displays various statistics about the simulation
 * ***************************************************************** */
    public class SimulationReporter {
        
        public List<Job> CompletedJobs;

        private readonly int _startTime;
        private readonly int _endTime;

        private float _averageUtilization;
        private float _averageFragmentation;
        private float _averageHoleSize;
        
/* **** SystemReporter Constructor ********************************** *
 *  Description:
 *      Builds the reporter
 * ***************************************************************** */
        public SimulationReporter (int startTime, int endTime) {
            _startTime = startTime;
            _endTime = endTime;
            
            CompletedJobs = new List<Job> ();
        }

/* **** Report ****************************************************** *
 *  Description:
 *      Prints relevant statistics to standard output
 * ***************************************************************** */
        public void Report (
            int time, 
            Memory memory, 
            Backstore backstore,
            bool displayBackstore = false) {
            
            // Out of bounds check
            if (time < _startTime || time > _endTime) {
                return;
            }
            
            // Only print a time label if it will be relevant
            if (displayBackstore) {
                if (time % 1000 == 0) {
                    Console.WriteLine(new string('-', 80));
                    Console.Write ("t=" + time + ":");
                }
            }
            
            // a) Turnaround, Wait, and Processing time averages
            if (time == _endTime) {
                int turnaroundAcc = 0;
                int waitAcc = 0;
                int durationAcc = 0;
                int sampleCount = ((_endTime - _startTime) / 100) + 1;
                
                foreach (Job j in CompletedJobs) {
                    turnaroundAcc += 
                        (j.CompletionTime - j.ArrivalTime);
                    waitAcc += 
                        (j.CompletionTime - j.CpuBurst - j.ArrivalTime);
                    durationAcc += 
                        (j.CompletionTime - j.StartTime);
                }
                
                // a)
                Console.WriteLine ("\tAverage Turnaround Time:\t" +
                    (turnaroundAcc / (float)CompletedJobs.Count)
                    .ToString("n2") + " VTU");
                Console.WriteLine ("\tAverage Wait Time:\t\t" +
                    (waitAcc / (float)CompletedJobs.Count)
                    .ToString("n2") + " VTU");
                Console.WriteLine ("\tAverage Processing Time:\t" +
                    (durationAcc / (float)CompletedJobs.Count)
                    .ToString("n2") + " VTU");
                
                // b)
                float averageUtilization =
                    _averageUtilization / sampleCount;
                Console.WriteLine ("\tAverage Utilized Memory:\t" +
                    averageUtilization.ToString("n2") + 
                    " bytes (" + 
                    (averageUtilization / 17.50f).ToString("n2") +
                    "%)");
                
                // c)
                Console.WriteLine ("\tAverage Fragmented Memory:\t" +
                    (_averageFragmentation / sampleCount)
                    .ToString("n2") + " bytes");
                
                // d)
                Console.WriteLine ("\tAverage Hole Size:\t\t" +
                    (_averageHoleSize / sampleCount)
                     .ToString("n2") + " bytes");
            }
                
            // Accumulate data without displaying
            if (time % 100 == 0) {
                // b) Memory utilization in bytes
                _averageUtilization +=
                    memory.GetUsedMemorySlots () * 10;
                
                // c) External Memory fragmentation
                _averageFragmentation +=
                    (1 - (memory.GetBiggestBlock ().Size / (float)memory.Size)) * 10;
                
                // d) Memory hole size
                _averageHoleSize +=
                    memory.GetMemoryHoleSize ();
            }

            if (displayBackstore) {
                // e) Backstore
                if (time % 1000 == 0) {
                    Console.WriteLine ("\tPending List:\t\t" +
                                       "Arrive\t" +
                                       "Size\t" +
                                       "CPU burst");
                    
                    foreach (Job j in backstore.Processes) {
                        Console.WriteLine ("\t\t\t\t" + j);
                    }
                }
            }
        }
    }
}