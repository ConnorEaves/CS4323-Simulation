namespace OSPA1 {
	
	/* **** Job ********************************************************************************************* *
	 *	Description:
	 *		Doubly linked-list node acting as a job container
	 * ************************************************************************************************************** */
	public class Job : MemoryBlock {
		
		// Never even used...
		public int Pid;
		
		// Counter until processing is completed
		public int Duration;

		public int ArrivalTime;
		
		// Populated by SystemSimulation
		public int StartTime;
		public int CompletionTime;
		
		/* **** Job Constructor ************************************************************************************* *
		 *	Description:
		 *		Initialization
		 * ********************************************************************************************************** */
		public Job (int pid, int size, int duration, int arrivalTime) : base (size) {
			Pid = pid;
			Duration = duration;
			ArrivalTime = arrivalTime;
		}
	}
}