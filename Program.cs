using System;
using System.Windows.Forms;

namespace FireworksOverlay;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FireworksForm());
    }
}