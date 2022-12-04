//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll

// Keyboard PTT experiment by Bjorn Ekelund SM7IUN sm7iun@ssa.se 
// Keyboard PTT mapped to Oem5 key. 
// Updated 2022-12-04

using System.Windows.Forms;
using IOComm;

namespace DXLog.net
{
    public class KeyPTT : ScriptClass
    {
        FrmMain main;
        bool Sending = false;

        // PTT key
        readonly Keys PTTkey = Keys.Oem5;
        // Controls whether the script displays something in the status bar
        readonly bool verbose = true;

        public void Initialize(FrmMain mainForm)
        {
            main = mainForm;
            main.KeyDown += new KeyEventHandler(HandleKeyPress);
            main.KeyUp += new KeyEventHandler(HandleKeyRelease);
        }

        public void Deinitialize() { }

        public void Main(FrmMain m, ContestData c, COMMain a) { }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == PTTkey && !Sending)
            {
                main.EscStopKeying();
                main.ContestDataProvider.TXOnRadio = main.ContestDataProvider.FocusedRadio;
                main.COMMainProvider.SetPTTOn(main.ContestDataProvider.TXOnRadio, false);
                Sending = true;
                if (verbose)
                {
                    main.SetMainStatusText(string.Format("Transmitting on radio {0}.", main.ContestDataProvider.TXOnRadio));
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