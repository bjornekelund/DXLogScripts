// Experimental antenna cycling script for Yaesu FTDX101D.
// By Bjorn Ekelund SM7IUN sm7iun@ssa.se 2020-04-18

using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class YaesuAntenna : IScriptClass
    {
        private int _currentAntenna;

        // Executes at DXLog.net start
        public void Initialize(FrmMain main)
        {
            // Choose and set the first antenna at start up
            _currentAntenna = 1;
            SetYaesuAntenna(_currentAntenna, main);
        }

        // Executes as DXLog.net close down
        public void Deinitialize() { }

        // Step through Antennas, Main is mapped to a key, typically not a shifted
        // key to allow rapid multiple presses
        // The value of currentAntenna steps through 1,2,3,1,2,3,1...
        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            _currentAntenna = (_currentAntenna % 3) + 1;
            SetYaesuAntenna(_currentAntenna, mainForm);
        }

        private void SetYaesuAntenna(int antenna, FrmMain main)
        {
            var modeIsSO2V = main.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;
            var focusedRadio = main.ContestDataProvider.FocusedRadio;

            // Physical radio is #1 in SO2V, otherwise the focused radio
            var physicalRadio = modeIsSO2V ? 1 : focusedRadio;

            // Act on currently selected VFO unless SO2V where the selected "radio" defines which VFO
            var avfo = ((focusedRadio == 2) && modeIsSO2V) ? "B" : main.ContestDataProvider.FocusedRadioActiveVFO;
            var vfo = avfo == "A" ? "0" : "1";

            if (main.COMMainProvider.RadioObject(physicalRadio) != null)
            {
                main.COMMainProvider.RadioObject(physicalRadio).SendCustomCommand("AN" + vfo + antenna + ";");
                main.SetMainStatusText($"{(vfo == "0" ? "Main" : "Sub")} antenna switched to #{_currentAntenna}.");
            }
        }
    }
}
