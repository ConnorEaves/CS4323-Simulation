using System; // Random

namespace OSPA2 {
    
/* **** SystemSimulation ******************************************** *
 *  Description:
 *     Runs and records a simulation
 * ***************************************************************** */
    public class SystemSimulation {

        // RNGesus
        private readonly Random _random;

        private readonly Memory _memory;
        private readonly CompactionScenario _compactionScenario;
        private readonly ProcessScheduler _processScheduler;
        private readonly Backstore _backstore;
        private readonly SimulationReporter _simulationReporter;

        private readonly bool _showBackstore;
        
/* **** SystemSimulation Constructor ******************************** *
 *  Description:
 *     Builds the simulation object and initializes required
 *     components
 * ***************************************************************** */
        public SystemSimulation (
            Func<Memory, Job, MemoryBlock> placementStrategy,
            CompactionScenario compactionScenario,
            bool showBackstore = false) {

            // Auto-seeded to something related to current system time.
            _random = new Random ();

            // Initialized to 175 "spaces":
            // 2000k bytes - 250k bytes for OS = 1750k bytes
            // 750k bytes / 10k job size "resolution" = 175 slots
            _memory = new Memory (175, placementStrategy);
            _processScheduler = new ProcessScheduler (5, _memory);
            _backstore = new Backstore (100);

            _compactionScenario = compactionScenario;

            // Circular references! Yay!
            _memory.Backstore = _backstore;
            _memory.ProcessScheduler = _processScheduler;
            _processScheduler.Memory = _memory;

            // The limits of our reporting for this simulation
            _simulationReporter = new SimulationReporter (1000, 5000);

            _showBackstore = showBackstore;
        }
        
/* **** Simulate **************************************************** *
 *  Description:
 *      Runs the simulation. (Main loop)
 * ***************************************************************** */
        public void Simulate (int duration) {
            int timeToNextJob = 0;
            for (int time = 0; time <= duration; time++) {
                
                // Call job generator and reset generation timer
                if (timeToNextJob-- == 0) {
                    GenerateNewJob (time);
                    timeToNextJob = _random.Next (1, 11);
                }
                
                // Compaction tests
                if (time % 250 == 0 &&
                    _compactionScenario == 
                    CompactionScenario.Every250VTU) {
                    _memory.Compact ();
                }
                
                if (time % 360 == 0 &&
                    _compactionScenario == 
                    CompactionScenario.Every360VTU) {
                    _memory.Compact ();
                }
                
                // Process jobs
                Job completedJob = _processScheduler.Process (time);
                if (completedJob != null) {
                    _simulationReporter.CompletedJobs.Add (completedJob);
                }
                
                // Output when necessary
                _simulationReporter.Report (
                    time,
                    _memory,
                    _backstore,
                    _showBackstore);
            }
        }

/* **** GenerateNewJob ********************************************** *
 *  Description:
 *      Generates a new job and requests it be allocated to memory
 * ***************************************************************** */
        public void GenerateNewJob (int time) {
            Job job = new Job (
                time,
                _random.Next (5, 31),
                _random.Next (5, 61),
                time);
            
            // Try to allocate to memory
            if (_memory.Allocate (job)) {
                _processScheduler.Add (job);
            } else {
                // add to backstore
                _backstore.StoreJob (job);
                if (_compactionScenario ==
                    CompactionScenario.OnMemRequestDenied) {
                    _memory.Compact ();
                }
            }
        }
    }
}