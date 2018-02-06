using System.Collections.Generic;

namespace OSPA2 {
/* **** Backstore *************************************************** *
 *  Description:
 *      Container for backing store. Simple list with fixed max size
 * ***************************************************************** */
    public class Backstore {
        public List<Job> Processes;
        
        private readonly int _capacity;
/* **** Backstore Constructor *************************************** *
 *  Description:
 *      Just initialization
 * ***************************************************************** */
        public Backstore (int capacity) {
            Processes = new List<Job> ();
            _capacity = capacity;
        }

/* **** StoreJob **************************************************** *
 *  Description:
 *      Wrapper for List<T>.Add that inforces size constraint
 * ***************************************************************** */
        public void StoreJob (Job job) {
            if (Processes.Count < _capacity) {
                Processes.Add (job);
            }
        }
    }
}