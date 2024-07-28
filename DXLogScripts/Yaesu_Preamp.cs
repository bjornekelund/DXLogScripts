// Experimental antenna cycling script for Yaesu FTDX101D.
// Steps through antennas for currently focused receiver
// By Bjorn Ekelund SM7IUN sm7iun@ssa.se 2024-07-28

using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class YaesuPreamp: IScriptClass
    {
        private int _currentPreamp;
        public string[] _currentState = new string[] {"IPO", "AMP 1", "AMP 2"};

        // Executes at DXLog.net start
        public void Initialize(FrmMain main)
        {
            // Choose and set IPO at start up
            _currentPreamp = 1;
            SetYaesuPreamp(_currentPreamp, main);
        }

        // Executes as DXLog.net close down
        public void Deinitialize() { }

        // Step through preamp settings, Main is mapped to a key, typically not a shifted
        // key to allow rapid multiple presses
        // The value of _currentPreamp steps through 0, 1, 2, 0, 1, 2
        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            _currentPreamp = (_currentPreamp + 1 ) % 3;
            SetYaesuPreamp(_currentPreamp, mainForm);
        }

        private void SetYaesuPreamp(int preamp, FrmMain main)
        {
            var modeIsSO2V = main.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;
            var focusedRadio = main.ContestDataProvider.FocusedRadio;

            // Physical radio is #1 in SO2V, otherwise the focused radio
            var physicalRadio = modeIsSO2V ? 1 : focusedRadio;

            // Act on currently selected VFO unless SO2V where the selected "radio" defines which VFO
            var activeVFO = ((focusedRadio == 2) && modeIsSO2V) ? "B" : main.ContestDataProvider.FocusedRadioActiveVFO;
            var vfoDigit = activeVFO == "A" ? "0" : "1";

            if (main.COMMainProvider.RadioObject(physicalRadio) != null)
            {
                main.COMMainProvider.RadioObject(physicalRadio).SendCustomCommand("PA" + vfoDigit + preamp + ";");
                main.SetMainStatusText($"{(vfoDigit == "0" ? "Main" : "Sub")} receiver set to {_currentState[_currentPreamp]}.");
            }
        }
    }
}
