using System;

namespace CIDesigns.MercurialToolbar.Commands
{
    public class CommitCommand : CommandBase
    {
        public override void Run(string solutionPath, Action<string> onProcMessage = null)
        {
            base.Execute("thg", "commit", solutionPath, onProcMessage);
        }
    }
}