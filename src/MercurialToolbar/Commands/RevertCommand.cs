using System;

namespace CIDesigns.MercurialToolbar.Commands
{
    public class RevertCommand : CommandBase
    {
        public override void Run(string solutionPath, Action<string> onProcMessage = null)
        {
            base.Execute("thg", "revert", solutionPath, onProcMessage);
        }
    }
}