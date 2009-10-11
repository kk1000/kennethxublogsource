using Microsoft.VisualBasic.ApplicationServices;

namespace DonkeyInput
{
    class App : WindowsFormsApplicationBase
    {
        private InputForm _inputForm;

        public App()
        {
            IsSingleInstance = true;
            EnableVisualStyles = true;
            ShutdownStyle = ShutdownMode.AfterMainFormCloses;
            StartupNextInstance += HandlerAppStartupNextInstance;
        }

        protected override void OnCreateMainForm()
        {
            _inputForm = new InputForm(CommandLineArgs);
            MainForm = _inputForm;
        }

        private void HandlerAppStartupNextInstance(object sender, StartupNextInstanceEventArgs eventArgs)
        {
            _inputForm.AddLink(eventArgs.CommandLine);
        }
    }
}
