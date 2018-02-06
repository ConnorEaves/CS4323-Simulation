using System;		// Func
using System.Linq;	// Enumerable.Join

namespace OSPA2 {
    
/* **** Memory ****************************************************** *
 *  Description:
 *      Holds the state of fragmentation and utiliztation of memory
 *
 *  Critque:
 *      Implemented as a linked list. All placement strategies have an
 *      insert complexity of O(n)
 * ***************************************************************** */
    public class Memory {

        public int Size;
        
        private MemoryBlock _head;
        private MemoryBlock _tail;
        
        private readonly Func<Memory, Job, MemoryBlock>
            _placementStrategy;

        public ProcessScheduler ProcessScheduler;
        public Backstore Backstore;

/* **** Memory Constructor ****************************************** *
 *  Description:
 *      Initialize the doubly linked list
 * ***************************************************************** */
        public Memory (
            int size,
            Func<Memory, Job, MemoryBlock> placementStrategy) {
            Size = size;
            _head = _tail = new MemoryBlock (size);
            _placementStrategy = placementStrategy;
        }

/* **** Allocate **************************************************** *
 *  Description:
 *      Attempts to add a job to the memory structure, returning true
 *      if the allocation was successful, and false otherwise
 * ***************************************************************** */
        public bool Allocate (
            Job job) {
            
            // Get appropriate position to allocate new job
            MemoryBlock emptyBlock = _placementStrategy (this, job);
            
            // If there is no valid location_processScheduler
            if (emptyBlock == null) {
                return false;
            }
            
            // Insert the job in the correct position
            Allocate (job, emptyBlock);
            return true;
        }

/* **** Allocate **************************************************** *
 *  Description:
 *      Perfoms linked-list insertion of a Job
 * ***************************************************************** */
        private void Allocate (
            MemoryBlock jobBlock, 
            MemoryBlock emptyBlock) {
            
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

/* **** Free ******************************************************** *
 *  Description:
 *      Replaces a Job with a new MemoryBlock of equal size
 * ***************************************************************** */
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

            // Load more jobs if we can
            LoadWaitingJobs ();
            
            // Coalesce
            Coalesce ();
            
            // Try loading stuff again
            LoadWaitingJobs ();
        }

/* **** Coalesce **************************************************** *
 *  Description:
 *      Combines free MemoryBlocks on either side of the given block
 * ***************************************************************** */
        public void Coalesce (MemoryBlock block) {
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
        
/* **** Coalesce **************************************************** *
 *  Description:
 *      Coalesces all free memory blocks
 * ***************************************************************** */
        public void Coalesce () {
            MemoryBlock walker = _head;
            while (walker != null) {
                if (!(walker is Job)) {
                    Coalesce(walker);
                }
                walker = walker.Next;
            }
        }

/* **** Compact ***************************************************** *
 *  Description:
 *      Compacts all free memory blocks to the "tail" end of memory
 * ***************************************************************** */
        public void Compact () {
            // Single block, already fully compacted
            if (_head == _tail) {
                return;
            }
            int compactionAcc = 0;
            MemoryBlock walker = _head;
            // Visit every block
            while (walker != null) {
                if (!(walker is Job)) {
                    // Accumulate free block sizes
                    compactionAcc += walker.Size;
                    // Delete free blocks
                    if (walker == _head) {
                        _head = walker.Next;
                        _head.Prev = null;
                    } else if (walker == _tail) {
                        _tail = walker.Prev;
                        _tail.Next = null;
                    } else {
                        walker.Next.Prev = walker.Prev;
                        walker.Prev.Next = walker.Next;
                    }
                }
                walker = walker.Next;
            }
            // If no free blocks were found, nothing need be done
            if (compactionAcc == 0) {
                return;
            }
            
            // Add a new large block to the tail end of memory
            MemoryBlock compactedMemoryBlock =
                new MemoryBlock (compactionAcc);
            compactedMemoryBlock.Prev = _tail;
            _tail.Next = compactedMemoryBlock;
            _tail = compactedMemoryBlock;
            
            // Load more jobs if we can
            LoadWaitingJobs ();
        }

        

/* **** GetUsedMemorySlots ****************************************** *
 *  Description:
 *      Retrieves the number of individual slots that are being used
 * ***************************************************************** */
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

/* **** GetMemoryHoleSize ****************************************** *
 *  Description:
 *      Returns the average size of all memory holes
 * ***************************************************************** */
        public float GetMemoryHoleSize () {
            int holeSize = 0;
            int holeCount = 0;
            MemoryBlock walker = _head;
            while (walker != null) {
                if (!(walker is Job)) {
                    holeSize += walker.Size;
                    ++holeCount;
                }
                walker = walker.Next;
            }
            if (holeCount == 0) {
                return 0;
            }
            return holeSize / (float)holeCount;
        }

/* **** GetBiggestBlock ********************************************* *
 *  Description:
 *      Retrieves the largest free memory block
 * ***************************************************************** */
        public MemoryBlock GetBiggestBlock () {
            MemoryBlock walker = _head;
            MemoryBlock largest = _head;
            while (walker != null) {
                if (!(walker is Job)) {
                    // Looking exclusively for largest block
                    if (walker.Size > largest.Size) {
                        largest = walker;
                    }
                }
                walker = walker.Next;
            }
            return largest;
        }
        
/* **** GetFirstMemoryBlock ***************************************** *
 *  Description:
 *      Retrieves the "first" free block large enough to accept the Job
 *
 *  Critque:
 *      Only checks from front to back. Could keep reference of the
 *      first block large enough to accept the smallest possible job
 *      (Would ignore any severe fragmentation at front).
 * ***************************************************************** */
        public static MemoryBlock GetFirstMemoryBlock (
            Memory memory,
            Job job) {
            
            MemoryBlock walker = memory._head;
            while (walker != null) {
                if (!(walker is Job) && job.Size <= walker.Size) {
                    return walker;
                }
                walker = walker.Next;
            }
            return null;
        }
        
/* **** GetBestMemoryBlock ****************************************** *
 *  Description:
 *      Retrives the smallest free block large enough to accept the Job
 *
 *  Critque:
 *      Only checks from front to back. Storing empty blocks in a tree
 *      may cut down on computation. However, given the size of n and
 *      the extra implementation, it was deemed not necessary.
 * ***************************************************************** */
        public static MemoryBlock GetBestMemoryBlock (
            Memory memory, 
            Job job) {
            
            MemoryBlock walker = memory._head;
            MemoryBlock best = null;
            while (walker != null) {
                if (!(walker is Job) &&
                    job.Size <= walker.Size &&
                    walker.Size <
                    (best ?? new MemoryBlock (int.MaxValue)).Size) {
                    best = walker;
                }
                walker = walker.Next;
            }
            return best;
        }
        
/* **** GetWorstMemoryBlock ***************************************** *
 *  Description:
 *      Retrives the largest free block large enough to accept the Job
 *
 *  Critque:
 *      Only checks from front to back. Storing empty blocks in a tree
 *      may cut down on computation. However, given the size of n and
 *      the extra implementation, it was deemed not necessary.
 * ***************************************************************** */
        public static MemoryBlock GetWorstMemoryBlock (
            Memory memory,
            Job job) {
            
            MemoryBlock walker = memory._head;
            MemoryBlock worst = null;
            while (walker != null) {
                if (!(walker is Job) &&
                    job.Size <= walker.Size &&
                    walker.Size >
                    (worst ?? new MemoryBlock (1)).Size) {
                    worst = walker;
                }
                walker = walker.Next;
            }
            return worst;
        }

/* **** LoadWaitingJobs ********************************************* *
 *  Description:
 *      Attempts to allocate as many jobs from backstore as possible to
 *      memory. Continues to attempt until all have been tried
 *
 *  Critque:
 *      There is no sanity check here and is simply first come first
 *      serve. Ideally, it would look for multiple allotable jobs to
 *      allocate together.
 * ***************************************************************** */
        public void LoadWaitingJobs () {
            // We'll break manunally
            while (true) {
                // Break flag
                bool allocatedJob = false;
                // Go through every job and attempt allocation
                for (int i = 0; i < Backstore.Processes.Count; i++) {
                    if (Allocate (Backstore.Processes[i])) {
                        // Add to scheduler and remove from backstore
                        ProcessScheduler.Add (Backstore.Processes[i]);
                        Backstore.Processes.RemoveAt (i);
                        allocatedJob = true;
                        break;
                    }
                }
                
                if (allocatedJob) {
                    // Got one in, try for two
                    continue;
                }
                // Couldn't fit anthing in
                break;
            }
        }

/* **** ToString **************************************************** *
 *  Description:
 *      Returns a string representing memory in the form:
 *          |---|@@@@|---|--|
 *      where - represents an empty block and @ represents an allocated
 *      Job
 * ***************************************************************** */
        public override string ToString () {
            
            string memoryString = "";
            
            MemoryBlock walker = _head;
            while (walker != null) {
                
                // Test if walker is a Job or MemoryBlock
                string printText = (walker is Job) ? "@" : "-";
                string blockString = "|";
                
                blockString = blockString +
                    string.Join(
                        "", 
                        Enumerable.Repeat (printText, walker.Size));
                memoryString = memoryString + blockString;
                
                walker = walker.Next;
            }
            memoryString += "|";
            return memoryString.PadRight (Console.WindowWidth - 1);
        }

    }
}