//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll
//INCLUDE_ASSEMBLY IOComm.dll

// Script to force other VFO of radio to same frequency as currently selected VFO 
// Works for all supported operating techniques
// By Björn Ekelund SM7IUN sm7iun@ssa.se 2019-01-30

using IOComm;

namespace DXLog.net
{
    public class EqualizeRadio : ScriptClass
    {
        public void Initialize(FrmMain main) {}

        public void Deinitialize() {}

        public void Main(FrmMain main, ContestData cdata, COMMain comMain)
        {
            double frequency;
            int focusedradio, vfo;
            string othervfo;

            focusedradio = cdata.FocusedRadio; // 1 or 2
            vfo = cdata.Radios[focusedradio].ActiveVFO; // 0 = VFO A, 1 = VFO B
            frequency = cdata.Radios[focusedradio].Freq[vfo];
            othervfo = vfo == 0 ? "B" : "A";
            main.SetCATFrequency(focusedradio, othervfo, frequency);
        }
    }
}

