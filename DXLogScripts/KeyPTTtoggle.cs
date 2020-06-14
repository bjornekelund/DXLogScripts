//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll

// Keyboard PTT experiment by Björn Ekelund SM7IUN sm7iun@ssa.se 

using IOComm;

namespace DXLog.net
{
    public class KeyPTTltoggle : ScriptClass
    {
        bool Sending = false;

        public void Initialize(FrmMain main) {}

        public void Deinitialize() { }

        public void Main(FrmMain main, ContestData cdata, COMMain comMain) 
        {
            cdata.TXOnRadio = cdata.FocusedRadio;
            if (!Sending)
            {
                Sending = true;
                main.HandleTXRequestChange(Sending);
                main.SetMainStatusText(string.Format("Transmitting on radio {0}.", cdata.TXOnRadio));
                main.COMMainProvider.SetPTTOn(cdata.TXOnRadio, false);
            }
            else
            {
                Sending = false;
                main.HandleTXRequestChange(Sending);
                main.SetMainStatusText(string.Format("PTT off on radio {0}", cdata.TXOnRadio));
                main.COMMainProvider.SetPTTOff(cdata.FocusedRadio);
            }
        }
    }
}