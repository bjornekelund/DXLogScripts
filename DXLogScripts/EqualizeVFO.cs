// Script to force other VFO of radio to same mode and frequency as currently selected VFO
// Works for all supported operating techniques
// Does not try to enforce mode if not Phone or CW.
// By Bjorn Ekelund SM7IUN sm7iun@ssa.se 2020-06-15
// Updated by James M1DST 2024-03-28
// Implemented in DXLog as Ctrl-F4

using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class EqualizeRadio : IScriptClass
    {
        public void Initialize(FrmMain mainForm)
        {
            Main(mainForm, mainForm.ContestDataProvider, mainForm.COMMainProvider, null);
        }

        public void Deinitialize() {}

        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            var focusedRadio = cdata.FocusedRadio; // 1 or 2
            var vfo = cdata.Radios[focusedRadio].ActiveVFO; // 0 = VFO A, 1 = VFO B
            var mode = cdata.FocusedRadioActiveMode;
            var frequency = cdata.Radios[focusedRadio].Freq[vfo];
            var otherVfo = vfo == 0 ? "B" : "A";
            mainForm.SetCATFrequency(focusedRadio, otherVfo, frequency);

            switch (mode)
            {
                case "SSB":
                case "LSB":
                case "USB":
                case "AM":
                case "FM":
                case "CW":
                    mainForm.SetCATMode(focusedRadio, otherVfo, mode, false);
                    break;
                default:
                    break;
            }
        }
    }
}

