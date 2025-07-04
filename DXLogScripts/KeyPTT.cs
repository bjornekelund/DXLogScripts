// Keyboard PTT experiment by Bjorn Ekelund SM7IUN sm7iun@ssa.se
// Keyboard PTT mapped to Oem5 key.
// Updated by James M1DST 2024-03-28

using System.Windows.Forms;
using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class KeyPTT : IScriptClass
    {
        FrmMain main;
        bool Sending = false;

        // PTT key
        readonly Keys PTTkey = Keys.ControlKey;

        // Controls whether the script displays something in the status bar
        readonly bool verbose = true;

        public void Initialize(FrmMain mainForm)
        {
            main = mainForm;
            main.KeyDown += HandleKeyPress;
            main.KeyUp += HandleKeyRelease;
        }

        public void Deinitialize() 
        {
            main.KeyDown -= HandleKeyPress;
            main.KeyUp -= HandleKeyRelease;
        }

        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent) { }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == PTTkey && !Sending)
            {
                main.EscStopKeying();
                main.ContestDataProvider.TXOnRadio = main.ContestDataProvider.FocusedRadio;
                main.HandleTXRequestChange(true, false, 0, false);
                main.COMMainProvider.SetPTTOn(main.ContestDataProvider.TXOnRadio, false);

                Sending = true;
                if (verbose)
                {
                    main.SetMainStatusText($"Transmitting on radio {main.ContestDataProvider.TXOnRadio}.");
                }
            }
        }

        private void HandleKeyRelease(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == PTTkey)
            {
                main.COMMainProvider.SetPTTOff(main.ContestDataProvider.FocusedRadio);
                Sending = false;
                if (verbose)
                {
                    main.SetMainStatusText("");
                }
            }
        }
    }
}