//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll

// Script to force radio 2 to frequency of radio 1
// By Björn Ekelund SM7IUN sm7iun@ssa.se 2019-01-30

using IOComm;

namespace DXLog.net
{
    public class EqualizeRadio : ScriptClass
    {
        static readonly bool debug = false;
        ContestData cdata;
        FrmMain mainForm;

        public void Initialize(FrmMain main) {}

        public void Deinitialize() {}

        public void Main(FrmMain main, ContestData cdata, COMMain comMain)
        {
            double _newFreq = cdata.Radios[1].Freq[0];
            main.SetCATFrequency(1, "B", _newFreq);

        }
    }
}

