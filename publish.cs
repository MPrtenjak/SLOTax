//css_import publish_worker;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;

class Script
{
    [STAThread]
    static public int Main(string[] args)
    {
        Worker worker = new Worker();
        worker.Execute();
        return 0;
    }

    internal class Worker : PublishWorker
    {
        public Worker()
        : base()
        {
					finalOutputName += @"\SloTaxOutput\SloTax";
        }

        public int Execute()
        {
            showStep("Clean");

            int errorID = 1;

            helper.ExecuteExternalProgram("clean.bat", string.Empty);

            showStep("Update version");
            string newNumber;

            if (!helper.UpdateVersionWithYear(out newNumber))
                return showError("Update version error", ++errorID);

            showStep("Build");
            if (helper.ExecuteExternalProgram("build.bat", string.Empty) != 0)
                return showError("Build error", ++errorID);

            showStep("Run NUnit tests");
            // .NET 4.0 can't RUN TLS1.2 ==> test fails
            if (!runNunitTests(@"SLOTaxService\bin\release", "SLOTaxService.dll", ++errorID))
                return ++errorID;

            showStep("Create folders version");
            //string newFolderName = helper.CreateFolders("FinalOutput", newNumber);
            string newFolderName = finalOutputName + @"\" + newNumber;

            showStep("Copy files");
            if (!copyFiles(newFolderName, "Net45", "SLOTaxGuiTest"))
                return showError("Copy files error", ++errorID);

            showStep("Delete NUnit help folders");
            string nunitFolder45 = System.IO.Path.Combine(newFolderName, @"Net45\UnitTests");
            System.IO.Directory.Delete(nunitFolder45, true);

            showStep("Done!!! New file version : " + newNumber);

            return 0;
        }

        public bool copyFiles(string destinationPath, string destinationSubPath, string sourceProject)
        {
            var script = helper.GetScriptName();
            var scriptPath = System.IO.Path.GetDirectoryName(script);

            string sourcePath = System.IO.Path.Combine(scriptPath, sourceProject);
            sourcePath = System.IO.Path.Combine(sourcePath, @"bin\Release");
            destinationPath = System.IO.Path.Combine(destinationPath, destinationSubPath);
            if (!helper.CopyFolderContents(sourcePath, destinationPath,
              new string[] { "*.exe", "*.dll", "*.config" })) return false;

            return true;
        }

        public bool runNunitTests(string destinationPath, string assembly, int errorID)
        {
            this.showCurrentElement(assembly);

            var script = helper.GetScriptName();
            var scriptPath = System.IO.Path.GetDirectoryName(script);

            string fullName = System.IO.Path.Combine(scriptPath, destinationPath);
            fullName = System.IO.Path.Combine(fullName, assembly);
            showStep("fullname : " + fullName);
            showStep("nunitConsoleFullPath : " + nunitConsoleFullPath);
            showStep(nunitConsoleFullPath + "  " + fullName);
            if (!helper.ExecuteNunit3Tests(nunitConsoleFullPath, fullName))
            {
                this.showError(string.Format("NUnit {0} error", assembly), errorID);
                return false;
            }

            return true;
        }
    }
}