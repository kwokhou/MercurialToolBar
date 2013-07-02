using System;

namespace CIDesigns.MercurialToolbar.Commands
{
    public class LogCommand : CommandBase
    {
        public override void Run(string solutionPath, Action<string> onProcMessage = null)
        {
            base.Execute("thg", "log", solutionPath, onProcMessage);
        }
    }
}