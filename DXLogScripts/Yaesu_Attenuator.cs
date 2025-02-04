// Experimental antenna cycling script for Yaesu FTDX101D.
// Steps through attenuator levels for currently selected receiver
// By Bjorn Ekelund SM7IUN sm7iun@ssa.se 2025-02-04

using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class YaesuAttenuator : IScriptClass
    {
        private int currentAttenuator;

        // Executes at DXLog.net start
        public void Initialize(FrmMain main)
        {
            // Choose and set the first antenna at start up
            currentAttenuator = 0;
            SetYaesuAttenuator(currentAttenuator, main);
        }

        // Executes as DXLog.net close down
        public void Deinitialize() { }

        // Step through Attenuator levels, Main is mapped to a key, typically not a shifted
        // key to allow rapid multiple presses
        // The value of currentAttenuator steps through 0,1,2,3,0,1,2,3,1...
        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            currentAttenuator = (currentAttenuator + 1) % 4;
            SetYaesuAttenuator(currentAttenuator, mainForm);
        }

        private void SetYaesuAttenuator(int attenuator, FrmMain main)
        {
            bool modeIsSO2V = main.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;
            int focusedRadio = main.ContestDataProvider.FocusedRadio;

            // Physical radio is #1 in SO2V, otherwise the focused radio
            int physicalRadio = modeIsSO2V ? 1 : focusedRadio;

            // Act on currently selected VFO unless SO2V where the selected "radio" defines which VFO
            string activeVFO = ((focusedRadio == 2) && modeIsSO2V) ? "B" : main.ContestDataProvider.FocusedRadioActiveVFO;
            string vfoDigit = activeVFO == "A" ? "0" : "1";
            string vfoName = vfoDigit == "0" ? "Main" : "Sub";

            string attName;
            switch (attenuator)
            {
                case 1:
                    attName = "6dB";
                    break;
                case 2:
                    attName = "12dB";
                    break;
                case 3:
                    attName = "18dB";
                    break;
                default:
                    attName = "off";
                    break;
            }

            if (main.COMMainProvider.RadioObject(physicalRadio) != null)
            {
                string command = $"RA{vfoDigit}{attenuator};";
                main.COMMainProvider.RadioObject(physicalRadio).SendCustomCommand(command);
                main.SetMainStatusText($"{vfoName} attenuator set to {attName}");
            }
        }
    }
}
