namespace OSPA2 {
    
/* **** Job ********************************************************* *
 *  Description:
 *      Doubly linked-list node acting as a job container
 * ***************************************************************** */
    public class Job : MemoryBlock {
        
        // Never even used...
        public int Pid;

        public int CpuBurst;
        
        // Counter until processing is completed
        public int Duration;

        public int ArrivalTime;
        
        // Populated by SystemSimulation
        public int StartTime;
        public int CompletionTime;
        
/* **** Job Constructor ********************************************* *
 *  Description:
 *      Initialization
 * ***************************************************************** */
        public Job (
            int pid,
            int size,
            int cpuBurst,
            int arrivalTime): base (size) {
            Pid = pid;
            CpuBurst = cpuBurst;
            Duration = cpuBurst;
            ArrivalTime = arrivalTime;
        }
        
        public override string ToString () {
            return ArrivalTime + "\t" +
                   Size + "\t" +
                   CpuBurst;
        }
    }
}