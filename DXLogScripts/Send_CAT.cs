// Experimental script to send text CAT command to focused radio
// By Bjorn Ekelund SM7IUN sm7iun@ssa.se 2024-06-03

//using IOComm;
//using NAudio.Midi;

namespace DXLog.net
{
    public class SendCAT : IScriptClass
    {
        FrmMain main;

        // Executes at DXLog.net start
        public void Initialize(FrmMain m) 
        { 
            main = m;
        }

        // Executes as DXLog.net close down
        public void Deinitialize() { }

        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            var modeIsSO2V = main.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;
            var focusedRadio = main.ContestDataProvider.FocusedRadio;
            var physicalRadio = modeIsSO2V ? 1 : focusedRadio;

            var catCommand = "KY CQ;";

            if (main.COMMainProvider.RadioObject(physicalRadio) != null)
            {
                main.COMMainProvider.RadioObject(physicalRadio).SendCustomCommand(catCommand);
                main.SetMainStatusText($"Sent command {catCommand} to radio #{physicalRadio}.");
            }
        }
    }
}
