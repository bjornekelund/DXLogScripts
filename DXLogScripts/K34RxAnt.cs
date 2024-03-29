// Elecraft K3/K4 RX antenna toggling.
// By SM7IUN & K1XM 2022-10-13
// Updated by James M1DST 2024-03-28

using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class K34RxAnt : IScriptClass
    {
        readonly bool StatusText = true;

        // Executes at DXLog.net start
        public void Initialize(FrmMain main){}

        // Executes as DXLog.net close down
        public void Deinitialize() {}

        // Executes at macro invocation
        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            // If SO1R or SO2V, only deal with radio 1
            var radio = cdata.OPTechnique == ContestData.Technique.SO1R || cdata.OPTechnique == ContestData.Technique.SO2V ? 1 : cdata.FocusedRadio;
            var radioObject = comMain.RadioObject(radio);

            if (radioObject != null)
            {
                var radioModel = (string)radioObject.GetType().GetField("RadioID").GetValue(null);
                var catCommand = string.Empty;

                // Use Contains() to accept name variants with suffix e.g., old or test
                if (radioModel.Contains("Elecraft K3/K3S"))
                {
                    catCommand = "SWT25;";
                }
                else if (radioModel.Contains("Elecraft K4"))
                {
                    catCommand = "AR/;";
                }

				if (catCommand != string.Empty)
				{
				    radioObject.SendCustomCommand(catCommand);
					if (StatusText)
					{
                        mainForm.SetMainStatusText($"K34RxAnt: Toggling Rx antenna on radio {radio} ({radioModel}) with command \"{catCommand}\".");
					}
				}
            }
            else
            {
                if (StatusText)
                {
                    mainForm.SetMainStatusText($"K34RxAnt: Radio {radio} is not available.");
                }
            }
        }
    }
}
