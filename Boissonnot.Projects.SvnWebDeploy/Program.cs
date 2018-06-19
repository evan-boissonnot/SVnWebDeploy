using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Boissonnot.Projects.SvnWebDeploy
{
    class Program
    {
        static void Main(string[] args)
        {
            string svnRepository = args[0];
            string svnRevision = args[1];
            string hudsonProject = args[2];

            Console.WriteLine("Process starting");

            //%LOOK% log %REPOS% -r %REV%
            string svnMessage = StartProcessToRead(Properties.Settings.Default.SVN_HOOK_PATH, string.Format("log \"{0}\" -r {1}", svnRepository, svnRevision));

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(Properties.Settings.Default.DEBUG_PATH, true))
            {
                writer.WriteLine("Starting process at : " + DateTime.Now.ToString());
                writer.WriteLine("repository : " + svnRepository);
                writer.WriteLine("svnRevision : " + svnRevision);
                writer.WriteLine("hudsonProject : " + hudsonProject);
                writer.WriteLine("svnMessage : " + svnMessage);
                writer.WriteLine(Environment.NewLine);
                writer.WriteLine(Environment.NewLine);
            }

            if (svnMessage.Contains(Properties.Settings.Default.PUBLISH_RECETTE))
            {
                hudsonProject = string.Format(Properties.Settings.Default.HUDSON_PROJECT_PUBLISH_URL, hudsonProject);
            }
            else
            {
                hudsonProject = string.Format(Properties.Settings.Default.HUDSON_PROJECT_URL, hudsonProject);
            }

            StartProcess(Properties.Settings.Default.WGET_PATH,
                             string.Format(" --auth-no-challenge --http-user={1} --http-password={2} {0}", hudsonProject, Properties.Settings.Default.JENKINS_USER, 
                                                                                                                          Properties.Settings.Default.JENKINS_PASSWORD));

            Console.WriteLine("Process ending");
        }

        private static void StartProcess(string fileName, string arguments = "")
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();

            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.Verb = "runas";
            process.Start();
        }

        private static string StartProcessToRead(string fileName, string arguments = "")
        {
            string output = "FAILED";
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = fileName;
            info.Arguments = " " + arguments;
            info.CreateNoWindow = false;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;

            using (Process process = Process.Start(info))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    output = reader.ReadToEnd();
                }
            }

            return output;
        }
    }
}
