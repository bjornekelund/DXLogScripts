//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll

// Synchronization of the speed of the radio's built-in keyer with DXLog.net.
// Since it is event driven it runs automatically and is not mapped to any key.
// Works for all operating scenarios including SO2V and for Kenwood, Yaesu,
// ICOM, Flex, etc. in any combination.
// By Bjorn Ekelund SM7IUN sm7iun@ssa.se 2022-11-17

using CWKeyer;
using IOComm;
using System;

namespace DXLog.net
{
    public class RadioSpeedSync : ScriptClass
    {
        readonly bool Debug = true;
        FrmMain mainForm;
        ContestData cdata;
        int lastFocus;
        int[] lastSpeed = { 0, 0 };

	    public void Initialize(FrmMain main)
	    {
            CATCommon radio1 = main.COMMainProvider.RadioObject(1);
            cdata = main.ContestDataProvider;
            mainForm = main;
            lastFocus = 1;

            if (mainForm._cwKeyer != null)
            {
                // Subscribe to CW speed change event
                mainForm._cwKeyer.CWSpeedChange += new CWKey.CWSpeedChangeDelegate(HandleCWSpeedChange);
                // Subscribe to radio focus change event
                cdata.FocusedRadioChanged += new ContestData.FocusedRadioChange(HandleFocusChange);
            }
        }

        public void Deinitialize() { }

        // No key is mapped to this script
        public void Main(FrmMain main, ContestData cdata, COMMain comMain) { }

        // Executes every time DXLog.net keyer speed is changed 
        private void HandleCWSpeedChange(int radioNumber, int newSpeed)
        {
            byte[] IcomSetSpeed = new byte[4] { 0x14, 0x0C, 0x00, 0x00 };

            // If SO2V, physical radio is always #1
            bool modeIsSo2V = mainForm.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;
            int physicalRadio = modeIsSo2V ? 1 : radioNumber;
            CATCommon radioObject = mainForm.COMMainProvider.RadioObject(physicalRadio);

            if (radioObject != null)
            {
                if (newSpeed != lastSpeed[radioNumber - 1])
                {
                    // Update radio's keyer speed
                    if (radioObject.IsICOM())
                    {
                        // ICOM scales 6-48 WPM onto 0-255 in a slightly weird way
                        int ICOMspeed = (int)((255.0 * (newSpeed - 5.5)) / (48.0 - 6.0)); 
                        IcomSetSpeed[2] = (byte)((ICOMspeed / 100) % 10);
                        IcomSetSpeed[3] = (byte)((((ICOMspeed / 10) % 10) << 4) + (ICOMspeed % 10));
                        radioObject.SendCustomCommand(IcomSetSpeed);

                        if (Debug)
                        {
                            mainForm.SetMainStatusText(string.Format(
                            "RadioSpeedSynch: {0}. Command: [{1}]. Radio {2} CW speed changed to {3} wpm! R1speed = {4} R2speed = {5}.",
                            ICOMspeed, BitConverter.ToString(IcomSetSpeed), radioNumber, mainForm._cwKeyer.CWSpeed(radioNumber), mainForm._cwKeyer.CWSpeed(1), mainForm._cwKeyer.CWSpeed(2))
                            );
                        }
                    }
                    else // Not ICOM
                    {
                        string speedCommand = "KS" + newSpeed.ToString("000") + ";";
                        radioObject.SendCustomCommand(speedCommand);
                        if (Debug)
                        {
                            mainForm.SetMainStatusText(string.Format(
                            "RadioSpeedSynch: Command: [{0}]. Radio {1} CW speed changed to {2} wpm! R1speed = {3} R2speed = {4}.",
                            speedCommand, radioNumber, mainForm._cwKeyer.CWSpeed(radioNumber), mainForm._cwKeyer.CWSpeed(1), mainForm._cwKeyer.CWSpeed(2))
                            );
                        }
                    }
                }
                lastSpeed[radioNumber - 1] = newSpeed;
            }
            else
            {
                mainForm.SetMainStatusText(string.Format("RadioSpeedSynch: Radio {0} is not available!", physicalRadio));
            }
        }

        // Event handler invoked when switching between radios (in SO2R) or VFO (in SO1R and SO2V) in DXLog.net
        private void HandleFocusChange()
        {
            int focusedRadio = mainForm.ContestDataProvider.FocusedRadio;
            bool modeIsSo2V = mainForm.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;

            // Do nothing unless its SO2V and there is an actual change in focus
            if (modeIsSo2V && (focusedRadio != lastFocus)) 
            {
                HandleCWSpeedChange(1, mainForm._cwKeyer.CWSpeed(focusedRadio));
            }
            lastFocus = focusedRadio;
        }
    }
}