﻿#region License
/*
* Copyright (C) 2002-2009 the original author or authors.
* 
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* 
*      http://www.apache.org/licenses/LICENSE-2.0
* 
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
#endregion

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.ApplicationServices;
using System.Diagnostics;

namespace DonkeyInput
{
    /// <summary>
    /// To handle single instance and parameter passing.
    /// </summary>
    /// <author>Kenneth Xu</author>
    class App : WindowsFormsApplicationBase
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private InputForm _inputForm;

        public App()
        {
            IsSingleInstance = true;
            EnableVisualStyles = true;
            ShutdownStyle = ShutdownMode.AfterMainFormCloses;
            StartupNextInstance += HandlerAppStartupNextInstance;
            Startup += HandlerAppStartup;
        }

        protected override void OnCreateMainForm()
        {
            _inputForm = new InputForm(CommandLineArgs);
            MainForm = _inputForm;
        }

        private static void HandlerAppStartup(object sender, StartupEventArgs e)
        {
            Process[] myProcesses =
                Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (myProcesses.Length > 1)
            {
                foreach (Process process in myProcesses)
                {
                    if (process != Process.GetCurrentProcess())
                    {
                        SetForegroundWindow(process.MainWindowHandle);
                    }
                }
            }
        }

        private void HandlerAppStartupNextInstance(object sender, StartupNextInstanceEventArgs eventArgs)
        {
            _inputForm.AddLink(eventArgs.CommandLine);
        }
    }
}
