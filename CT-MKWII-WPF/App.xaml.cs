using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CT_MKWII_WPF.Pages;
using CT_MKWII_WPF.Utils;
using CT_MKWII_WPF.Utils.Auto_updator;

namespace CT_MKWII_WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            VersionChecker.CheckForUpdates();
        }

    }

}