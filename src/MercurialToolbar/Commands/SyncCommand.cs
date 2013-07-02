using System;

namespace CIDesigns.MercurialToolbar.Commands
{
    public class SyncCommand : CommandBase
    {
        public override void Run(string solutionPath, Action<string> onProcMessage = null)
        {
            base.Execute("thg", "sync", solutionPath, onProcMessage);
        }
    }
}