using System;
using System.Web;
using System.Net;

namespace DonkeyInput
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            new App().Run(args);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new InputForm(args));
            //DonkeyInput(args[0]);
        }
    }
}
