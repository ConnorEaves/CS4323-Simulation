namespace OSPA2 {
/* **** CompactionScenario ****************************************** *
 *  Description:
 *      Options that dictate when comaption should be done
 *
 *  Critiques:
 *      Using flags could allow for multiple selections
 * ***************************************************************** */
    public enum CompactionScenario {
        OnMemRequestDenied,
        Every250VTU,
        Every360VTU
    }
}