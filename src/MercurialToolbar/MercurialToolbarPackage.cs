using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using CIDesigns.MercurialToolbar.Commands;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace CIDesigns.MercurialToolbar
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidMercurialToolbarPkgString)]
    public sealed class MercurialToolbarPackage : Package
    {
        private Messenger _messenger;
        private Guid _mercurialPaneGuid = new Guid("D0145B58-ECA1-44B1-B2A6-F91B5A7B0D36");
        private IVsOutputWindowPane _mercurialPane;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public MercurialToolbarPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            var shell = (IVsUIShell) GetService(typeof (SVsUIShell));
            var output = (IVsOutputWindow) GetService(typeof (SVsOutputWindow));

            try
            {
                output.CreatePane(ref _mercurialPaneGuid, "Mercurial Toolbar", 1, 1);
                output.GetPane(ref _mercurialPaneGuid, out _mercurialPane);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _messenger = new Messenger(shell, _mercurialPane);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var commandService = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;

            if (commandService != null)
            {
                var commitCommandId = new CommandID(GuidList.guidMercurialToolbarCmdSet, (int) PkgCmdIDList.cmdidCommit);
                var commitMenuItem = new MenuCommand((sender, e) => ExecuteCommand(new CommitCommand()), commitCommandId);

                var logCommandId = new CommandID(GuidList.guidMercurialToolbarCmdSet, (int) PkgCmdIDList.cmdidLog);
                var logMenuItem = new MenuCommand((sender, e) => ExecuteCommand(new LogCommand()), logCommandId);

                var revertCommandId = new CommandID(GuidList.guidMercurialToolbarCmdSet, (int) PkgCmdIDList.cmdidRevert);
                var revertMenuItem = new MenuCommand((sender, e) => ExecuteCommand(new RevertCommand()), revertCommandId);

                var syncCommandId = new CommandID(GuidList.guidMercurialToolbarCmdSet, (int) PkgCmdIDList.cmdidSync);
                var syncMenuItem = new MenuCommand((sender, e) => ExecuteCommand(new SyncCommand()), syncCommandId);

                var updateCommandId = new CommandID(GuidList.guidMercurialToolbarCmdSet, (int) PkgCmdIDList.cmdidUpdate);
                var updateMenuItem = new MenuCommand((sender, e) => ExecuteCommand(new UpdateCommand()), updateCommandId);

                var settingsCommand = new CommandID(GuidList.guidMercurialToolbarCmdSet, (int) PkgCmdIDList.cmdidSettings);
                var settingsMenuItem = new MenuCommand((sender, e) => ExecuteCommand(new SettingsCommand()), settingsCommand);

                var commandComboId = new CommandID(GuidList.guidMercurialToolbarCmdSet, (int) PkgCmdIDList.cmdidCommandCombo);
                var commandComboMenuItem = new OleMenuCommand(OnArbitraryHgCommandCombo, commandComboId);

                commandService.AddCommand(commitMenuItem);
                commandService.AddCommand(logMenuItem);
                commandService.AddCommand(revertMenuItem);
                commandService.AddCommand(syncMenuItem);
                commandService.AddCommand(updateMenuItem);
                commandService.AddCommand(settingsMenuItem);
                commandService.AddCommand(commandComboMenuItem);
            }
        }
        #endregion

        private void ExecuteCommand<T>(T command) where T : CommandBase
        {
            var solution = (IVsSolution) GetService(typeof (SVsSolution));

            string solutionPath, solutionFile, userOptions;
            solution.GetSolutionInfo(out solutionPath, out solutionFile, out userOptions);

            if (string.IsNullOrEmpty(solutionPath))
            {
                _messenger.WriteOutput("No solution loaded.");
                return;
            }

            try
            {
                command.Run(solutionPath, (message => _messenger.WriteOutput(message)));
            }
            catch (InvalidOperationException ie)
            { _messenger.ShowMessage(ie.Message, OLEMSGICON.OLEMSGICON_WARNING); }
            catch (Exception e)
            { _messenger.ShowMessage(e.Message, OLEMSGICON.OLEMSGICON_CRITICAL); }
        }

        private string _currentMruComboChoice;
        private void OnArbitraryHgCommandCombo(object sender, EventArgs e)
        {
            if (e == EventArgs.Empty)
            {
                return;
            }

            var eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                object input = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (vOut != IntPtr.Zero && input != null)
                {
                    return;
                }
                if (vOut != IntPtr.Zero)
                {
                    // when vOut is non-NULL, the IDE is requesting the current value for the combo
                    Marshal.GetNativeVariantForObject(this._currentMruComboChoice, vOut);
                }
                else if (input != null)
                {
                    string newChoice = input.ToString();

                    // new value was selected or typed in
                    if (!string.IsNullOrEmpty(newChoice))
                    {
                        string commandType = newChoice.Trim().Split(' ')[0];

                        string args;
                        switch (commandType)
                        {
                            case "thg":
                                args = newChoice.Split(' ')
                                                .Where(str => str != "thg")
                                                .Aggregate(string.Empty, (current, str) => current + (str + " "))
                                                .Trim();

                                ExecuteCommand(new ArbitraryCommand(commandType, args));

                                break;
                            case "hg":
                                args = newChoice.Split(' ')
                                                .Where(str => str != "hg")
                                                .Aggregate(string.Empty, (current, str) => current + (str + " "))
                                                .Trim();

                                ExecuteCommand(new ArbitraryCommand(commandType, args));
                                break;
                            default:
                                _messenger.ShowMessage("Invalid hg or thg command.", OLEMSGICON.OLEMSGICON_WARNING);
                                break;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}
