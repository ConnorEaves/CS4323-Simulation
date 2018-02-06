using System;		// Func
using System.Linq;	// Enumerable.Join

namespace OSPA1 {
	
	/* **** Memory ************************************************************************************************** *
	 *	Description:
	 *		Holds the state of fragmentation and utiliztation of memory
	 *
	 *	Critque:
	 *		Implemented as a linked list. All placement strategies have an insert complexity of O(n)
	 * ************************************************************************************************************** */
	public class Memory {
		
		private MemoryBlock _head;
		private MemoryBlock _tail;

		/* **** Memory Constructor ********************************************************************************** *
		 *	Description:
		 *		Initialize the doubly linked list
		 * ********************************************************************************************************** */
		public Memory (int size) {
			_head = _tail = new MemoryBlock (size);
		}

		/* **** Allocate ******************************************************************************************** *
		 *	Description:
		 *		Attempts to add a job to the memory structure, returning true if the allocation was successful, and
		 *			false otherwise
		 * ********************************************************************************************************** */
		public bool Allocate (Job job, Func<Memory, Job, MemoryBlock> placementStrategy) {
			
			// Get appropriate position to allocate new job
			MemoryBlock emptyBlock = placementStrategy (this, job);
			
			// If there is no valid location
			if (emptyBlock == null) {
				return false;
			}
			
			// Insert the job in the correct position
			Allocate (job, emptyBlock);
			return true;
		}

		/* **** Allocate ******************************************************************************************** *
		 *	Description:
		 *		Perfoms linked-list insertion of a Job
		 * ********************************************************************************************************** */
		private void Allocate (MemoryBlock jobBlock, MemoryBlock emptyBlock) {
			
			jobBlock.Prev = emptyBlock.Prev;

			if (emptyBlock == _head) {
				_head = jobBlock;
			} else {
				jobBlock.Prev.Next = jobBlock;
			}
			
			if (jobBlock.Size == emptyBlock.Size) {
				jobBlock.Next = emptyBlock.Next;
				if (emptyBlock == _tail) {
					_tail = jobBlock;
				} else {
					jobBlock.Next.Prev = jobBlock;
				}
			} else {
				jobBlock.Next = emptyBlock;
				emptyBlock.Prev = jobBlock;
				emptyBlock.Size -= jobBlock.Size;
			}
		}

		/* **** Free ************************************************************************************************ *
		 *	Description:
		 *		Perfoms linked-list deletion of a Job
		 * ********************************************************************************************************** */
		public void Free (Job job) {
			
			// Create a "proxy" memory block
			MemoryBlock freeBlock = new MemoryBlock (job.Size);
			
			
			if (job.Next != null) {
				freeBlock.Next = job.Next;
				freeBlock.Next.Prev = freeBlock;
			}
			
			if (job.Prev != null) {
				freeBlock.Prev = job.Prev;
				freeBlock.Prev.Next = freeBlock;
			}
			
			if (job == _head) {
				_head = freeBlock;
			}
			
			if (job == _tail) {
				_tail = freeBlock;
			}
			Coalesce(freeBlock);
		}

		private void Coalesce (MemoryBlock block) {
			if (block.Prev != null && !(block.Prev is Job)) {
				block.Size += block.Prev.Size;
				block.Prev = block.Prev.Prev;
				if (block.Prev != null) {
					block.Prev.Next = block;
				} else {
					_head = block;
				}
			}
			if (block.Next != null && !(block.Next is Job)) {
				block.Size += block.Next.Size;
				block.Next = block.Next.Next;
				if (block.Next != null) {
					block.Next.Prev = block;
				} else {
					_tail = block;
				}
			}
		}

		/* **** GetUsedMemorySlots ********************************************************************************** *
		 *	Description:
		 *		Retrieves the number of individual slots that are being used
		 * ********************************************************************************************************** */
		public int GetUsedMemorySlots () {
			
			int usedSlots = 0;
			
			MemoryBlock walker = _head;
			while (walker != null) {
				// Determine if this MemoryBlock is a job or not
				usedSlots += (walker is Job) ? walker.Size : 0;
				walker = walker.Next;
			}
			return usedSlots;
		}

		/* **** GetMemoryHoleInfo *********************************************************************************** *
		 *	Description:
		 *		Retrieves the number and average size of empty MemoryBlocks
		 * ********************************************************************************************************** */
		public void GetMemoryHoleInfo (out int holeCount, out float averageHoleSize) {
			
			// Required for out params
			holeCount = 0;
			averageHoleSize = 0;
			
			MemoryBlock walker = _head;
			while (walker != null) {
				// Python has spoiled me with 'is not'
				if (!(walker is Job)) {
					holeCount++;
					averageHoleSize += walker.Size;
				}
				walker = walker.Next;
			}
			
			averageHoleSize /= (float)holeCount;
		}
		
		/* **** GetMemoryJobInfo ************************************************************************************ *
		 *	Description:
		 *		Retrieves the number and average size of allocated Jobs
		 * ********************************************************************************************************** */
		public void GetMemoryJobInfo (out int jobCount, out float averageJobSize) {
			
			// Required for out params
			jobCount = 0;
			averageJobSize = 0;
			
			MemoryBlock walker = _head;
			while (walker != null) {
				if (walker is Job) {
					jobCount++;
					averageJobSize += walker.Size;
				}
				walker = walker.Next;
			}

			averageJobSize /= (float)jobCount;
		}
		
		/* **** GetFirstMemoryBlock ********************************************************************************* *
		 *	Description:
		 *		Retrieves the "first" free block large enough to accept the Job
		 *
		 *	Critque:
		 *		Only checks from front to back. Could keep reference of the first block large enough to accept the
		 *			smallest possible job (Would ignore any severe fragmentation at front).
		 * ********************************************************************************************************** */
		public static MemoryBlock GetFirstMemoryBlock (Memory memory, Job job) {
			
			MemoryBlock walker = memory._head;
			while (walker != null) {
				if (!(walker is Job) && job.Size <= walker.Size) {
					return walker;
				}
				walker = walker.Next;
			}
			return null;
		}
		
		/* **** GetBestMemoryBlock ********************************************************************************** *
		 *	Description:
		 *		Retrives the smallest free block large enough to accept the Job
		 *
		 *	Critque:
		 *		Only checks from front to back. Storing empty blocks in a tree may cut down on computation. However,
		 *			given the size of n and the extra implementation, it was deemed not necessary.
		 * ********************************************************************************************************** */
		public static MemoryBlock GetBestMemoryBlock (Memory memory, Job job) {
			
			MemoryBlock walker = memory._head;
			MemoryBlock best = null;
			while (walker != null) {
				if (!(walker is Job) && job.Size <= walker.Size && walker.Size < (best ?? new MemoryBlock (int.MaxValue)).Size) {
					best = walker;
				}
				walker = walker.Next;
			}
			return best;
		}
		
		/* **** GetWorstMemoryBlock ********************************************************************************** *
		 *	Description:
		 *		Retrives the largest free block large enough to accept the Job
		 *
		 *	Critque:
		 *		Only checks from front to back. Storing empty blocks in a tree may cut down on computation. However,
		 *			given the size of n and the extra implementation, it was deemed not necessary.
		 * ********************************************************************************************************** */
		public static MemoryBlock GetWorstMemoryBlock (Memory memory, Job job) {
			
			MemoryBlock walker = memory._head;
			MemoryBlock worst = null;
			while (walker != null) {
				if (!(walker is Job) && job.Size <= walker.Size && walker.Size > (worst ?? new MemoryBlock (1)).Size) {
					worst = walker;
				}
				walker = walker.Next;
			}
			return worst;
		}
		
		/* **** ToString ******************************************************************************************** *
		 *	Description:
		 *		Returns a string representing memory in the form:
		 *			|---|@@@@|---|--|
		 *		where - represents an empty block and @ represents an allocated Job
		 * ********************************************************************************************************** */
		public override string ToString () {
			
			string memoryString = "";
			
			MemoryBlock walker = _head;
			while (walker != null) {
				
				// Test if walker is a Job or MemoryBlock
				string printText = (walker is Job) ? "@" : "-";
				string blockString = "|";
				
				blockString = blockString + string.Join("", Enumerable.Repeat (printText, walker.Size));
				memoryString = memoryString + blockString;
				
				walker = walker.Next;
			}
			memoryString += "|";
			return memoryString;
		}
	}
}