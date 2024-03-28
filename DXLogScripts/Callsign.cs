// Experimental

using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class Callsign : IScriptClass
    {
        // Executes at DXLog.net start
        public void Initialize(FrmMain main) { }

        // Executes as DXLog.net close down
        public void Deinitialize() { }

        // Display entered callsign in status bar
        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            var callsign = mainForm.CurrentNewEntryLine.ActualQSO.Callsign;
            mainForm.SetMainStatusText($"Current callsign is {callsign}");
        }
    }
}
