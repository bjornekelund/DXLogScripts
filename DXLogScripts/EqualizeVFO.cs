//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll

// Script to force other VFO of radio to same mode and frequency as currently selected VFO 
// Works for all supported operating techniques
// Does not try to enforce mode if not Phone or CW.
// By Björn Ekelund SM7IUN sm7iun@ssa.se 2020-06-15
// Updated 2022-10-13
// Implemented in DXLog as Ctrl-F4

using IOComm;

namespace DXLog.net
{
    public class EqualizeRadio : ScriptClass
    {
        public void Initialize(FrmMain main) 
        {
            Main(main, main.ContestDataProvider, main.COMMainProvider);
        }

        public void Deinitialize() {}

        public void Main(FrmMain main, ContestData cdata, COMMain comMain)
        {
            double frequency;
            int focusedradio, vfo;
            string mode, othervfo;

            focusedradio = cdata.FocusedRadio; // 1 or 2
            vfo = cdata.Radios[focusedradio].ActiveVFO; // 0 = VFO A, 1 = VFO B
            mode = cdata.FocusedRadioActiveMode; 
            frequency = cdata.Radios[focusedradio].Freq[vfo];
            othervfo = vfo == 0 ? "B" : "A";
            main.SetCATFrequency(focusedradio, othervfo, frequency);

            switch (mode)
            {
                case "SSB":
                case "LSB":
                case "USB":
                case "AM":
                case "FM":
                case "CW":
                    main.SetCATMode(focusedradio, othervfo, mode, false);
                    break;
                default:
                    break;
            }
        }
    }
}

