// CW transmission script

using System;
using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class CW_MYCALL : IScriptClass
    {
        private const bool Debug = true;

        // Executes at DXLog.net start
        public void Initialize(FrmMain main) { }

        // Executes as DXLog.net close down
        public void Deinitialize() { }

        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            var message = mainForm.ContestDataProvider.dalHeader.Callsign;
            SendString(message, mainForm);
        }

        private void SendString(string message, FrmMain main)
        {
            var modeIsSO2V = main.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;
            var focusedRadio = main.ContestDataProvider.FocusedRadio;

            // Physical radio is #1 in SO2V, otherwise the focused radio
            var physicalRadio = modeIsSO2V ? 1 : focusedRadio;
            var radio = main.COMMainProvider.RadioObject(physicalRadio);

            // If there is no radio or if it is not ICOM, do nothing
            if (radio != null)
            {
                if (radio.IsICOM())
                {
                    var bytes = new byte[message.Length + 1];
                    bytes[0] = 0x17;

                    for (var i = 1; i < message.Length + 1; i++)
                    {
                        bytes[i] = (byte)(message[i]);
                    }

                    radio.SendCustomCommand(bytes);

                    if (Debug)
                    {
                        main.SetMainStatusText($"ICOM_SERIAL: CI-V command: [{BitConverter.ToString(bytes)}].");
                    }
                }
                else
                {
                    var command = "KY" + message + ";";
                    radio.SendCustomCommand(command);

                    if (Debug)
                    {
                        main.SetMainStatusText($"ICOM_SERIAL: CAT command: [{command}].");
                    }
                }

            }
        }
    }
}
