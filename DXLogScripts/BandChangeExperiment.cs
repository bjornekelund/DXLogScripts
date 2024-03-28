using IOComm;
using NAudio.Midi;

namespace DXLog.net
{
    public class AcomBandChange : IScriptClass
    {
        ContestData _cdata;
        private FrmMain _mainForm;

        public AcomBandChange(ContestData cdata)
        {
            _cdata = cdata;
        }

        public void Initialize(FrmMain main)
        {
            _cdata = main.ContestDataProvider;
            _mainForm = main;
            _cdata.ActiveRadioBandChanged += new ContestData.ActiveRadioBandChange(HandleBandChange);
            _mainForm.SetMainStatusText("script started " + main.ContestDataProvider);
        }

        public void Deinitialize() { }

        public void Main(FrmMain mainForm, ContestData cdata, COMMain comMain, MidiEvent midiEvent)
        {
            HandleBandChange(mainForm.ContestDataProvider.FocusedRadio);
        }

        private void HandleBandChange(int radioNumber)
        {
            _mainForm.SetMainStatusText("HandleBandChange Radio" + radioNumber);
        }

    }
}