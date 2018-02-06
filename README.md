# CS4323-Simulation

Operating system simulation that mimics the general function of a system's memory manager, process scheduler, and backing disk.

The simulation performs the following functions:
* Generate randomized jobs within specific parameters at random intervals.
* Attempt to allocate those jobs into main memory using specified memory management schemes (Best-fit, First-fit).
* Save unallocated jobs to a backing disk.
* Process jobs using a round-robin scheduler.
* Coalesce and compact freed memory upon process completion.
* Retrieve jobs from the backing disk if possible.
