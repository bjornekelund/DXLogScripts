//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll

// Elecraft K3/K4 RX antenna toggling.
// By Bob Wilson N6TV n6tv@arrl.net 2 October 2022 20:53 UTC

using System;
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
            int radio;
            CATCommon radioobject;

            if (cdata.OPTechnique == ContestData.Technique.SO1R || cdata.OPTechnique == ContestData.Technique.SO2V)
            {
                radio = 1;
                radioobject = comMain._radio1Object;
            }
            else
            {
                radio = cdata.FocusedRadio;
                radioobject = radio == 1 ? comMain._radio1Object : comMain._radio2Object;
            }

            string HOSTcmd = string.Empty;
            string radiotype = radioobject.RadioType();

            //main.SetMainStatusText(string.Format("K34RxAnt: Radio type {0}.", radiotype));

            if (radioobject != null)
            {
				switch (radiotype)
				{
				    case "Elecraft K3/K3S":				
						HOSTcmd = "SWT25;";
					    break;
				    case "Elecraft K4":
						HOSTcmd = "AR/;";
					    break;
				}

				if (HOSTcmd != string.Empty) 
				{
				    radioobject.SendCustomCommand(HOSTcmd);
					if (StatusText)
					{
						main.SetMainStatusText(string.Format("K34RxAnt: Toggling Rx antenna on radio {0} ({1}) with command {2}.", radio, radiotype, HOSTcmd));
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
