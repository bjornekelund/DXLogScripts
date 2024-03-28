// Experimental antenna cycling script for Yaesu FTDX101D.
// By Bjorn Ekelund SM7IUN sm7iun@ssa.se 2022-11-09

using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class YaesuAntenna2 : IScriptClass
    {
        private bool _rxAntenna;

        // Executes at DXLog.net start
        public void Initialize(FrmMain main)
        {
            // Choose and set the first antenna at start up
            _rxAntenna = false;
            SetAntenna3(_rxAntenna, main);
        }

        // Executes as DXLog.net close down
        public void Deinitialize() { }

        // Toggle between main and receive antenna.
        // Main is mapped to a key, typically not a shifted key to allow rapid multiple presses
        // The value of currentAntenna steps through 1,3,1,3,1...
        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            _rxAntenna = !_rxAntenna;
            SetAntenna3(_rxAntenna, mainForm);
        }

        private void SetAntenna3(bool rxAnt, FrmMain main)
        {
            var modeIsSO2V = main.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;
            var focusedRadio = main.ContestDataProvider.FocusedRadio;

            // Physical radio is #1 in SO2V, otherwise the focused radio
            var physicalRadio = modeIsSO2V ? 1 : focusedRadio;

            // Act on currently selected VFO unless SO2V where the selected "radio" defines which VFO
            var avfo = ((focusedRadio == 2) && modeIsSO2V) ? "B" : main.ContestDataProvider.FocusedRadioActiveVFO;
            var vfo = avfo == "A" ? "0" : "1";
            var catCommand = "AN" + vfo + (rxAnt ? "3" : "1") + ";";

            if (main.COMMainProvider.RadioObject(physicalRadio) != null)
            {
                main.COMMainProvider.RadioObject(physicalRadio).SendCustomCommand(catCommand);
                main.SetMainStatusText($"{(vfo == "0" ? "Main" : "Sub")} antenna switched to #{(_rxAntenna ? 3 : 1)}.");
            }
        }
    }
}
