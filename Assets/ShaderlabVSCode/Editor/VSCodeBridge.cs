//  Copyright (c) 2017 amlovey
//  

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ShaderlabVSCode
{
    public class VSCodeBridge
    {
        public static string GetVSCodePath()
        {
#if UNITY_EDITOR_OSX
            string[] paths =
            {
                "/Applications/Visual Studio Code.app",
                "/Applications/Visual Studio Code - Insiders.app"
            };
#elif UNITY_EDITOR_WIN
            var programes = GetProgramFilesPath();
            string[] paths =
            {
                programes + Path.AltDirectorySeparatorChar + @"\Microsoft VS Code\bin\code.cmd",
                programes + Path.AltDirectorySeparatorChar + @"\Microsoft VS Code\bin\code-insiders.cmd"
            };
#endif

            for (int i = 0; i < paths.Length; i++)
            {
                if (CheckVSCodeExists(paths[i]))
                {
                    return paths[i];
                }
            }

            return paths[0];
        }

        private static string GetProgramFilesPath()
        {
            if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        public static bool CheckVSCodeExists(string path)
        {
#if UNITY_EDITOR_OSX
            return Directory.Exists(path);
#else
            return File.Exists(path);
#endif
        }

        public static bool IsVSCodeExists()
        {
            var code = GetVSCodePath();
            return CheckVSCodeExists(code);
        }

        public static void CallVSCodeWithArgs(string args)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            string codePath = GetVSCodePath();
            if (!CheckVSCodeExists(codePath))
            {
                return;
            }

#if UNITY_EDITOR_OSX
            process.StartInfo.FileName = "open";

            if (codePath.Contains("Insiders"))
            {
                process.StartInfo.Arguments = " -n -b \"com.microsoft.VSCodeInsiders\" --args " + args.Replace(@"\", @"\\");
            }
            else
            {
                process.StartInfo.Arguments = " -n -b \"com.microsoft.VSCode\" --args " + args.Replace(@"\", @"\\");
            }

            process.StartInfo.UseShellExecute = false;
#elif UNITY_EDITOR_WIN
            process.StartInfo.FileName = codePath;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
#endif
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
        }
    }
}