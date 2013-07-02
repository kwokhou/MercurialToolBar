using System;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;

namespace CIDesigns.MercurialToolbar
{
    public class Messenger
    {
        private readonly IVsUIShell _uiShell;
        private readonly IVsOutputWindowPane _outPane;

        public Messenger(IVsUIShell uiShell, IVsOutputWindowPane outPane)
        {
            _uiShell = uiShell;
            _outPane = outPane;
        }

        public void WriteOutput(string message)
        {
            _outPane.OutputString(message);
            _outPane.Activate();
        }

        public void ShowMessage(string message, OLEMSGICON icon)
        {
            Guid clsid = Guid.Empty;
            int result;
            
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(
                _uiShell.ShowMessageBox(
                    0,
                    ref clsid,
                    "Mercurial Toolbar",
                    string.Format(CultureInfo.CurrentCulture, message),
                    string.Empty,
                    0,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                    icon,
                    0, 
                    out result)
                );
        }

        public static void PopMessage(string message, MessageBoxIcon icon)
        {
            MessageBox.Show(message, "Mercurial Toolbar", MessageBoxButtons.OK, icon);
        }
    }
}