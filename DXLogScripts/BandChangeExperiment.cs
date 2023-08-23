//INCLUDE_ASSEMBLY System.dll
//INCLUDE_ASSEMBLY System.Windows.Forms.dll
using System;
using IOComm;

namespace DXLog.net
{
    public class AcomBandChange : ScriptClass
    {
        ContestData cdata;
        FrmMain mainForm;

        public void Initialize(FrmMain main)
        {
            cdata = main.ContestDataProvider;
            mainForm = main;
            cdata.ActiveRadioBandChanged += new ContestData.ActiveRadioBandChange(HandleBandChange);
            mainForm.SetMainStatusText("script started " + cdata);
        }

        public void Deinitialize() { }

        public void Main(FrmMain main, ContestData cdata, COMMain comMain)
        {
            HandleBandChange(mainForm.ContestDataProvider.FocusedRadio);
        }

        private void HandleBandChange(int RadioNumber)
        {
            mainForm.SetMainStatusText("HandleBandChange Radio" + RadioNumber.ToString());
        }

    }
}