//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll

// CW transmission script

using System;
using IOComm;

namespace DXLog.net
{
    public class CW_SERIAL : ScriptClass
    {
        const bool Debug = true;

        // Executes at DXLog.net start 
        public void Initialize(FrmMain main) { }

        // Executes as DXLog.net close down
        public void Deinitialize() { } 

        public void Main(FrmMain main, ContestData cdata, COMMain comMain)
        {
            int qsonumber = 23;
            string message = main.CWAbbrev(qsonumber.ToString("000"));

            SendString(message, main);
        }

        private void SendString(string message, FrmMain main)
        {
            bool modeIsSO2V = main.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;
            int focusedRadio = main.ContestDataProvider.FocusedRadio;

            // Physical radio is #1 in SO2V, otherwised the focused radio
            int physicalRadio = modeIsSO2V ? 1 : focusedRadio;
            CATCommon radio = main.COMMainProvider.RadioObject(physicalRadio);

            // If there is no radio or if it is not ICOM, do nothing
            if (radio != null)
            {
                if (radio.IsICOM())
                {
                    byte[] bytes = new byte[message.Length + 1];
                    bytes[0] = 0x17;

                    for (int i = 1; i < message.Length + 1; i++)
                    {
                        bytes[i] = (byte)(message[i]);
                    }

                    radio.SendCustomCommand(bytes);

                    if (Debug)
                    {
                        main.SetMainStatusText(string.Format("ICOM_SERIAL: CI-V command: [{1}]. ", BitConverter.ToString(bytes)));
                    }
                }
                else
                {
                    string command = "KY" + message + ";";
                    radio.SendCustomCommand(command);

                    if (Debug)
                    {
                        main.SetMainStatusText(string.Format("ICOM_SERIAL: CAT command: [{1}]. ", command));
                    }
                }

            }
        }
    }
}
