using System;

namespace CIDesigns.MercurialToolbar.Commands
{
    public class SettingsCommand : CommandBase
    {
        public override void Run(string solutionPath, Action<string> onProcMessage = null)
        {
            base.Execute("thg", "repoconfig", solutionPath, onProcMessage);
        }
    }
}