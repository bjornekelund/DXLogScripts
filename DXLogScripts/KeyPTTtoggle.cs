// Keyboard PTT experiment by Bjorn Ekelund SM7IUN sm7iun@ssa.se
// Updated by James M1DST 2024-03-28

using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class KeyPTTtoggle : IScriptClass
    {
        public void Initialize(FrmMain main) {}

        public void Deinitialize() { }

        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            cdata.TXOnRadio = cdata.FocusedRadio;

            if (cdata.Radios[cdata.TXOnRadio].TXStatus)
            {
                mainForm.EscStopKeying();
                mainForm.HandleTXRequestChange(false, false, 0, false);
                mainForm.SetMainStatusText($"PTT off on radio {cdata.TXOnRadio}");
                mainForm.COMMainProvider.SetPTTOff(cdata.FocusedRadio);
            }
            else
            {
                mainForm.EscStopKeying();
                mainForm.HandleTXRequestChange(true, false, 0, false);
                mainForm.SetMainStatusText($"Transmitting on radio {cdata.TXOnRadio}.");
                mainForm.COMMainProvider.SetPTTOn(cdata.TXOnRadio, false);
            }
        }
    }
}