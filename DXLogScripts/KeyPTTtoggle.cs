// Keyboard PTT experiment by Bjorn Ekelund SM7IUN sm7iun@ssa.se
// Updated 2024-03-25

using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class KeyPTTtoggle : IScriptClass
    {
        bool Sending = false;

        public void Initialize(FrmMain main) {}

        public void Deinitialize() { }

        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            cdata.TXOnRadio = cdata.FocusedRadio;
            if (!Sending)
            {
                mainForm.EscStopKeying();
                Sending = true;
                mainForm.HandleTXRequestChange(Sending, false, 0, false);
                mainForm.SetMainStatusText($"Transmitting on radio {cdata.TXOnRadio}.");
                mainForm.COMMainProvider.SetPTTOn(cdata.TXOnRadio, false);
            }
            else
            {
                Sending = false;
                mainForm.HandleTXRequestChange(Sending, false, 0, false);
                mainForm.SetMainStatusText($"PTT off on radio {cdata.TXOnRadio}");
                mainForm.COMMainProvider.SetPTTOff(cdata.FocusedRadio);
            }
        }
    }
}