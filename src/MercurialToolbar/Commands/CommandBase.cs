using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

namespace CIDesigns.MercurialToolbar.Commands
{
    public abstract class CommandBase
    {
        public virtual void Run(string solutionPath, Action<string> onProcMessage = null) { }

        protected virtual void Execute(string cmd, string args, string solutionPath, Action<string> onProcMessage = null)
        {
            string tortoisePath = GetRegistryValue(Registry.CurrentUser, @"Software\TortoiseHg", string.Empty);

            if (string.IsNullOrEmpty(tortoisePath))
                tortoisePath = GetRegistryValue(Registry.Users, @"Software\TortoiseHg", string.Empty);

            cmd = tortoisePath + cmd;

            try
            {
                var process = new Process
                {
                    EnableRaisingEvents = true,
                    StartInfo =
                    {
                        UseShellExecute = false,
                        ErrorDialog = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        FileName = cmd,
                        Arguments = args,
                        WindowStyle = ProcessWindowStyle.Normal,
                        LoadUserProfile = true,
                        WorkingDirectory = solutionPath
                    }
                };

                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (sender, e) =>
                    {
                        if (onProcMessage == null) return;
                        if (!string.IsNullOrEmpty(e.Data))
                            onProcMessage.Invoke(e.Data + Environment.NewLine);
                    };

                process.OutputDataReceived += (sender, e) =>
                    {
                        if (onProcMessage == null) return;
                        if (!string.IsNullOrEmpty(e.Data))
                            onProcMessage.Invoke(e.Data + Environment.NewLine);
                    };

                process.Exited += (sender, ea) =>
                    {
                        if (onProcMessage != null) return;

                        var proc = (Process)sender;
                        string processMessage = proc.StandardOutput.ReadToEnd() + proc.StandardError.ReadToEnd();

                        if (string.IsNullOrEmpty(processMessage)) return;

                        Messenger.PopMessage(processMessage, MessageBoxIcon.Warning);
                    };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected static string GetRegistryValue(RegistryKey root, string subkey, string key)
        {
            try
            {
                RegistryKey registryKey = root.OpenSubKey(subkey, false);

                string value = "";

                if (registryKey != null && registryKey.GetValue(key) is string)
                {
                    value = registryKey.GetValue(key).ToString();
                    registryKey.Flush();
                    registryKey.Close();
                }

                return value;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You do not have sufficient permisions to check the registry.");
            }

            return "";
        }
    }
}