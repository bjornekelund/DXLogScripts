//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll

// Experimental 

using IOComm;

namespace DXLog.net
{
    public class Callsign : ScriptClass
    {
        int currentAntenna;

        // Executes at DXLog.net start 
        public void Initialize(FrmMain main) { }

        // Executes as DXLog.net close down
        public void Deinitialize() { }

        // Display entered callsign in status bar
        public void Main(FrmMain main, ContestData cdata, COMMain comMain)
        {
            string callsign = main.CurrentNewEntryLine.ActualQSO.Callsign;
            main.SetMainStatusText(string.Format("Current callsign is {0}", callsign));
        }
    }
}
