namespace OSPA1 {
	
	/* **** MemoryBlock ********************************************************************************************* *
	 *	Description:
	 *		Doubly linked-list node acting as a memory block container
	 * ************************************************************************************************************** */
	public class MemoryBlock {
		
		// Size in slots
		public int Size;
		
		// Doubly linked-list node
		public MemoryBlock Prev;
		public MemoryBlock Next;
		
		/* **** MemoryBlock Constructor ***************************************************************************** *
		 *	Description:
		 *		Initialization
		 * ********************************************************************************************************** */
		public MemoryBlock (int size) {
			Size = size;
			Prev = Next = null;
		}
	}
}