using System;

namespace CIDesigns.MercurialToolbar.Commands
{
    public class UpdateCommand : CommandBase
    {
        public override void Run(string solutionPath, Action<string> onProcMessage = null)
        {
            base.Execute("thg", "update", solutionPath, onProcMessage);
        }
    }
}