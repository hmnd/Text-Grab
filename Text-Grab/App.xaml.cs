﻿using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Text_Grab.Properties;
using Text_Grab.Utilities;
using Text_Grab.Views;

namespace Text_Grab
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        void appStartup(object sender, StartupEventArgs e)
        {
            // Register COM server and activator type
            bool handledArgument = false;

            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                string argsInvoked = toastArgs.Argument;
                // Need to dispatch to UI thread if performing UI operations
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (String.IsNullOrWhiteSpace(argsInvoked) == false)
                    {
                        EditTextWindow mtw = new EditTextWindow(argsInvoked);
                        mtw.Show();
                        handledArgument = true;
                    }
                }));
            };

            Current.DispatcherUnhandledException += CurrentDispatcherUnhandledException;


            for (int i = 0; i != e.Args.Length && !handledArgument; ++i)
            {
                Debug.WriteLine($"ARG {i}:{e.Args[i]}");
                if (e.Args[i].Contains("ToastActivated"))
                {
                    Debug.WriteLine("Launched from toast");
                    handledArgument = true;
                }
                else if (e.Args[i] == "Settings")
                {
                    SettingsWindow sw = new SettingsWindow();
                    sw.Show();
                    handledArgument = true;
                }
                else if (e.Args[i] == "GrabFrame")
                {
                    GrabFrame gf = new GrabFrame();
                    gf.Show();
                    handledArgument = true;
                }
                else if (e.Args[i] == "Fullscreen")
                {
                    WindowUtilities.LaunchFullScreenGrab();
                    handledArgument = true;
                }
                else if (e.Args[i] == "EditText")
                {
                    EditTextWindow manipulateTextWindow = new EditTextWindow();
                    manipulateTextWindow.Show();
                    handledArgument = true;
                }
                else if (File.Exists(e.Args[i]))
                {
                    EditTextWindow manipulateTextWindow = new EditTextWindow();
                    manipulateTextWindow.OpenThisPath(e.Args[i]);
                    manipulateTextWindow.Show();
                    handledArgument = true;
                }
            }

            if (!handledArgument)
            {
                if (Settings.Default.FirstRun)
                {
                    FirstRunWindow frw = new FirstRunWindow();
                    frw.Show();

                    Settings.Default.FirstRun = false;
                    Settings.Default.Save();
                }
                else
                {
                    switch (Settings.Default.DefaultLaunch)
                    {
                        case "Fullscreen":
                            WindowUtilities.LaunchFullScreenGrab();
                            break;
                        case "GrabFrame":
                            GrabFrame gf = new GrabFrame();
                            gf.Show();
                            break;
                        case "EditText":
                            EditTextWindow manipulateTextWindow = new EditTextWindow();
                            manipulateTextWindow.Show();
                            break;
                        default:
                            EditTextWindow editTextWindow = new EditTextWindow();
                            editTextWindow.Show();
                            break;
                    }
                }
            }
        }

        private void CurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // unhandled exceptions thrown from UI thread
            Debug.WriteLine($"Unhandled exception: {e.Exception}");
            e.Handled = true;
            Current.Shutdown();
        }
    }
}
