// Synchronization of the speed of the radio's built-in keyer with DXLog.net.
// Since it is event driven it runs automatically and is not mapped to any key.
// Works for all operating scenarios including SO2V and for Kenwood, Yaesu,
// ICOM, Flex, etc. in any combination.
// By Bjorn Ekelund SM7IUN sm7iun@ssa.se 2022-11-17
// Updated by James M1DST 2024-03-28

using CWKeyer;
using IOComm;
using System;
using NAudio.Midi;

namespace DXLog.net
{
    public class RadioSpeedSync : IScriptClass
    {
        readonly bool Debug = true;
        FrmMain mainForm;
        ContestData cdata;
        int lastFocus;
        int[] lastSpeed;

        public void Initialize(FrmMain main)
        {
            var radio1 = main.COMMainProvider.RadioObject(1);
            cdata = main.ContestDataProvider;
            mainForm = main;
            lastFocus = 1;
            lastSpeed[0] = 0;
            lastSpeed[1] = 0;

            if (mainForm._cwKeyer != null)
            {
                // Subscribe to CW speed change event
                mainForm._cwKeyer.CWSpeedChange += new CWKey.CWSpeedChangeDelegate(HandleCWSpeedChange);
                // Subscribe to radio focus change event
                cdata.FocusedRadioChanged += new ContestData.FocusedRadioChange(HandleFocusChange);
            }
        }

        public void Deinitialize() { }

        // This script is event driven so there is no need to map a key to this script
        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent) { }

        // Executes every time DXLog.net CW keying speed is changed
        private void HandleCWSpeedChange(int radioNumber, int newSpeed)
        {
            var icomSetSpeed = new byte[4] { 0x14, 0x0C, 0x00, 0x00 };

            // If SO2V, physical radio is always #1
            var modeIsSo2V = mainForm.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;
            var physicalRadio = modeIsSo2V ? 1 : radioNumber;

            // Radio object to control
            var radioObject = mainForm.COMMainProvider.RadioObject(physicalRadio);

            if (radioObject != null)
            {
                if (newSpeed != lastSpeed[radioNumber - 1])
                {
                    // Update radio's keyer speed
                    if (radioObject.IsICOM())
                    {
                        // ICOM scales 6-48 WPM onto 0-255 in a slightly weird way
                        var ICOMspeed = (int)((255.0 * (newSpeed - 5.5)) / (48.0 - 6.0));
                        icomSetSpeed[2] = (byte)((ICOMspeed / 100) % 10);
                        icomSetSpeed[3] = (byte)((((ICOMspeed / 10) % 10) << 4) + (ICOMspeed % 10));
                        radioObject.SendCustomCommand(icomSetSpeed);

                        if (Debug)
                        {
                            mainForm.SetMainStatusText($"RadioSpeedSync: {ICOMspeed}. Command: [{BitConverter.ToString(icomSetSpeed)}]. Radio {radioNumber} CW speed changed to {mainForm._cwKeyer.CWSpeed(radioNumber)} wpm! R1speed = {mainForm._cwKeyer.CWSpeed(1)} R2speed = {mainForm._cwKeyer.CWSpeed(2)}."
                            );
                        }
                    }
                    else // Not ICOM
                    {
                        // KS###; CAT command is common for Yaesu, Kenwood, Elecraft, etc.
                        var speedCommand = "KS" + newSpeed.ToString("000") + ";";
                        radioObject.SendCustomCommand(speedCommand);
                        if (Debug)
                        {
                            mainForm.SetMainStatusText($"RadioSpeedSync: Command: [{speedCommand}]. Radio {radioNumber} CW speed changed to {mainForm._cwKeyer.CWSpeed(radioNumber)} wpm! R1speed = {mainForm._cwKeyer.CWSpeed(1)} R2speed = {mainForm._cwKeyer.CWSpeed(2)}.");
                        }
                    }
                }
                lastSpeed[radioNumber - 1] = newSpeed;
            }
            else
            {
                // Radio object is missing
                mainForm.SetMainStatusText($"RadioSpeedSync: Radio {physicalRadio} is not available!");
            }
        }

        // Event handler invoked when switching between radios (in SO2R) or VFO (in SO1R and SO2V) in DXLog.net
        private void HandleFocusChange()
        {
            var focusedRadio = mainForm.ContestDataProvider.FocusedRadio;
            var modeIsSo2V = mainForm.ContestDataProvider.OPTechnique == ContestData.Technique.SO2V;

            // Do nothing unless its SO2V and there is an actual change in focus
            if (modeIsSo2V && focusedRadio != lastFocus)
            {
                HandleCWSpeedChange(1, mainForm._cwKeyer.CWSpeed(focusedRadio));
            }
            lastFocus = focusedRadio;
        }
    }
}