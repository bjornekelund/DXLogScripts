// CW transmission script

using System;
using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class CW_SERIAL : IScriptClass
    {
        const bool Debug = true;

        // Executes at DXLog.net start
        public void Initialize(FrmMain main) { }

        // Executes as DXLog.net close down
        public void Deinitialize() { }

        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            var qso = mainForm.panelQSO.GetLogEntryLine(mainForm.ContestDataProvider.FocusedRadio + 9);
            var qsonumber = qso.ActualQSO.Nr;
            //int qsonumber = 23;

            var message = mainForm.CWAbbrev(qsonumber.ToString("000"));

            SendString(message, mainForm);
        }

        private void SendString(string message, FrmMain main)
        {
            var modeIsSO2V = main.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;
            var focusedRadio = main.ContestDataProvider.FocusedRadio;

            // Physical radio is #1 in SO2V, otherwised the focused radio
            var physicalRadio = modeIsSO2V ? 1 : focusedRadio;
            var radio = main.COMMainProvider.RadioObject(physicalRadio);
            //CATCommon radio = null;
            // If there is no radio or if it is not ICOM, do nothing
            //if (radio != null && radio.IsICOM())
            if (false)
            {
                byte[] bytes = new byte[message.Length + 1];
                bytes[0] = 0x17;

                for (int i = 1; i < message.Length + 1; i++)
                {
                    bytes[i] = (byte)(message[i]);
                }

                if (radio != null)
                    radio.SendCustomCommand(bytes);

                if (Debug)
                {
                    main.SetMainStatusText(string.Format("ICOM_SERIAL: CI-V command: [{1}]. ", BitConverter.ToString(bytes)));
                }
            }
            else
            {
                var command = "KY" + message + ";";
                radio?.SendCustomCommand(command);

                if (Debug)
                {
                    main.SetMainStatusText("ICOM_SERIAL: CAT command: [{" + command + "}].");
                }
            }
        }
    }
}
