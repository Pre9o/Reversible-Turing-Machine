using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversibleTuringMachine;

public static class Utils
{
    public static void OpenBrowser(string url) {
        var sInfo = new System.Diagnostics.ProcessStartInfo(url) {
            UseShellExecute = true,
        };
        System.Diagnostics.Process.Start(sInfo);
    }
}
