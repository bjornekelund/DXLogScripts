//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll

// Experimental antenna cycling script for Yaesu FTDX101D. 
// By Bjorn Ekelund SM7IUN sm7iun@ssa.se 2022-11-09

using IOComm;

namespace DXLog.net
{
    public class YaesuAntenna2 : ScriptClass
    {
        bool RxAntenna;

        // Executes at DXLog.net start 
        public void Initialize(FrmMain main)
        {
            // Choose and set the first antenna at start up
            RxAntenna = false;
            SetAntenna3(RxAntenna, main);
        }

        // Executes as DXLog.net close down
        public void Deinitialize() { }

        // Toggle between main and receive antenna.
        // Main is mapped to a key, typically not a shifted key to allow rapid multiple presses
        // The value of currentAntenna steps through 1,3,1,3,1...
        public void Main(FrmMain main, ContestData cdata, COMMain comMain)
        {
            RxAntenna = !RxAntenna;
            SetAntenna3(RxAntenna, main);
        }

        private void SetAntenna3(bool rxant, FrmMain main)
        {
            bool modeIsSO2V = main.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;
            int focusedRadio = main.ContestDataProvider.FocusedRadio;

            // Physical radio is #1 in SO2V, otherwised the focused radio
            int physicalRadio = modeIsSO2V ? 1 : focusedRadio;

            // Act on currently selected VFO unless SO2V where the selected "radio" defines which VFO
            string avfo = ((focusedRadio == 2) && modeIsSO2V) ? "B" : main.ContestDataProvider.FocusedRadioActiveVFO;
            string vfo = avfo == "A" ? "0" : "1";
            string catcommand = "AN" + vfo + (rxant ? "3" : "1") + ";";

            if (main.COMMainProvider.RadioObject(physicalRadio) != null)
            {
                main.COMMainProvider.RadioObject(physicalRadio).SendCustomCommand(catcommand);
                main.SetMainStatusText(string.Format("{0} antenna switched to #{1}.", vfo == "0" ? "Main" : "Sub", RxAntenna ? 3 : 1));
            }
        }
    }
}
