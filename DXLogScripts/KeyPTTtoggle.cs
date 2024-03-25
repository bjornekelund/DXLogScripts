//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll

// Keyboard PTT experiment by Bjorn Ekelund SM7IUN sm7iun@ssa.se 
// Updated 2024-03-25

using IOComm;

namespace DXLog.net
{
    public class KeyPTTtoggle : ScriptClass
    {
        bool Sending = false;

        public void Initialize(FrmMain main) {}

        public void Deinitialize() { }

        public void Main(FrmMain main, ContestData cdata, COMMain comMain) 
        {
            cdata.TXOnRadio = cdata.FocusedRadio;
            if (!Sending)
            {
                main.EscStopKeying();
                Sending = true;
                main.HandleTXRequestChange(Sending, false, 0, false);
                main.SetMainStatusText(string.Format("Transmitting on radio {0}.", cdata.TXOnRadio));
                main.COMMainProvider.SetPTTOn(cdata.TXOnRadio, false);
            }
            else
            {
                Sending = false;
                main.HandleTXRequestChange(Sending, false, 0, false);
                main.SetMainStatusText(string.Format("PTT off on radio {0}", cdata.TXOnRadio));
                main.COMMainProvider.SetPTTOff(cdata.FocusedRadio);
            }
        }
    }
}