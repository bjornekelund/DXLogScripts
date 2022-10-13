//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll

// Elecraft K3/K4 RX antenna toggling.
// By Bob Wilson N6TV n6tv@arrl.net 2 October 2022 20:53 UTC

using IOComm;

namespace DXLog.net
{
    public class K4RxAnt : ScriptClass
    {
        readonly bool StatusText = true;

        // Executes at DXLog.net start 
        public void Initialize(FrmMain main){}

        // Executes as DXLog.net close down
        public void Deinitialize() {}

        // Executes at macro invocation
        public void Main(FrmMain main, ContestData cdata, COMMain comMain)
        {
            // If SO1R or SO2V, only deal with radio 1
            int radio = cdata.OPTechnique == ContestData.Technique.SO1R || cdata.OPTechnique == ContestData.Technique.SO2V ? 1 : cdata.FocusedRadio;
            CATCommon radioobject = comMain.RadioObject(radio);

            if (radioobject != null)
            {
                string radiomodel = (string)radioobject.GetType().GetField("RadioID").GetValue(null);
                string catcommand = string.Empty;

                // Use Contains() to accept name variants with suffix e.g., old or test
                if (radiomodel.Contains("Elecraft K3/K3S"))
                {
                    catcommand = "SWT25;";
                }
                else if (radiomodel.Contains("Elecraft K4"))
                {
                    catcommand = "AR/;";
                }

				if (catcommand != string.Empty) 
				{
				    radioobject.SendCustomCommand(catcommand);
					if (StatusText)
					{
						main.SetMainStatusText(string.Format("K34RxAnt: Toggling Rx antenna on radio {0} ({1}) with command \"{2}\".", radio, radiomodel, catcommand));
					}
				}
            }
            else
            {
                if (StatusText)
                {
                    main.SetMainStatusText(string.Format("K34RxAnt: Radio {0} is not available.", radio));
                }
            }
        }
    }
}
