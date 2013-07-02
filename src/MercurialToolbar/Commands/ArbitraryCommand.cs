using System;

namespace CIDesigns.MercurialToolbar.Commands
{
    public class ArbitraryCommand : CommandBase
    {
        private readonly string _cmd;
        private readonly string _args;

        public ArbitraryCommand(string cmd, string args)
        {
            _cmd = cmd;
            _args = args;
        }

        public override void Run(string solutionPath, Action<string> onProcMessage = null)
        {
            if (!string.IsNullOrEmpty(_cmd) && !string.IsNullOrEmpty(_args))
                base.Execute(_cmd, _args, solutionPath, onProcMessage);
        }
    }
}