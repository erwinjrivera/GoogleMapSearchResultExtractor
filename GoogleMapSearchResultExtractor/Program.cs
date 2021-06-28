using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleMapSearchResultExtractor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Updater.GitHubRepo = Properties.Settings.Default.Repository;

            if (Updater.AutoUpdate(args))
                return;
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

           var mainForm = new MainForm();
            
            AppDomain.CurrentDomain.AssemblyResolve += (sender, a) =>
            {
                string resourceName = new AssemblyName(a.Name).Name + ".dll";
                string resource = Array.Find(mainForm.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
            

            Application.Run(mainForm);
        }

   
    }
}
