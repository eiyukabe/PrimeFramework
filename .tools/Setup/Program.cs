using System;
using System.Diagnostics;
using System.IO;

namespace Setup
{
    public class Program
    {
        private const string PrimeFrameworkProjectName = "Prime Framework";

        static void Main(string[] args)
        {
            /* Get the name of the new game */
            Console.WriteLine("Enter the name of your new game:");
            var gameName = Console.ReadLine();

            /* Set filepaths */
            string newGameSetupDir = Directory.GetCurrentDirectory();                           // <path>\Prime Framework\.New Game Setup\
            string primeDir = Directory.GetParent(newGameSetupDir).FullName;                    // <path>\Prime Framework\
            string newGameDir = $"{Directory.GetParent(primeDir).FullName}\\{gameName}\\";      // <path>\New Game Name\

            /* Create new game directory */
            try
            {
                Directory.CreateDirectory(newGameDir);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                SetupFailed();
                return;
            }

            /* Set filepaths to the project files */
            string gitignore = $"{newGameSetupDir}\\.gitignore";
            string defaultEnv = $"{primeDir}\\default_env.tres";
            string icon = $"{primeDir}\\icon.png";
            string iconImport = $"{primeDir}\\icon.png.import";
            string godotProjectFile = $"{primeDir}\\project.godot";
            string fixCSProj = $"{primeDir}\\Fix csproj.exe";

            string newGitignore = $"{newGameDir}\\.gitignore";
            string newDefaultEnv = $"{newGameDir}\\default_env.tres";
            string newIcon = $"{newGameDir}\\icon.png";
            string newIconImport = $"{newGameDir}\\icon.png.import";
            string newGodotProjectFile = $"{newGameDir}\\project.godot";
            string newFixCSProj = $"{newGameDir}\\Fix csproj.exe";

            /* Copy project files to new game directory */
            CopyFile(gitignore, newGitignore);
            CopyFile(defaultEnv, newDefaultEnv);
            CopyFile(icon, newIcon);
            CopyFile(iconImport, newIconImport);
            CopyFile(godotProjectFile, newGodotProjectFile);
            CopyFile(fixCSProj, newFixCSProj);

            /* Copy "Game" folder*/
            CopyAll(new DirectoryInfo($"{primeDir}\\Game"), new DirectoryInfo($"{newGameDir}\\Game"));

            /* Set the name of the game in project.godot */
            string text = File.ReadAllText(newGodotProjectFile);
            text = text.Replace($"config/name=\"{PrimeFrameworkProjectName}\"", $"config/name=\"{gameName}\"");
            WriteFile(newGodotProjectFile, text);

            /* Junction "Framework" folder to new game folder */
            string cmd = $"/C mklink /J \"{newGameDir}\\Framework\" \"{primeDir}\\Framework\"";
            Process.Start("CMD.exe", cmd);

            /* Create .sln file */
            WriteFile($"{newGameDir}\\{gameName}.sln", GetSlnText(gameName));

            /* Create launch.json file for debugging in Visual Studio Code */
            var pathToVSCode = $"{newGameDir}\\.vscode";
            Directory.CreateDirectory(pathToVSCode);
            WriteFile($"{pathToVSCode}\\launch.json", GetLaunchJsonText());

            /* Create .csproj file by running Fix csproj.exe */
            Process fix = new Process();
            fix.StartInfo.UseShellExecute = true;
            fix.StartInfo.WorkingDirectory = newGameDir;
            fix.StartInfo.FileName = "Fix csproj.exe";
            fix.StartInfo.Arguments = "\"" + gameName + "\"";
            fix.Start();

            /* Open new project in Godot */
            Process project = new Process();
            project.StartInfo.UseShellExecute = true;
            project.StartInfo.WorkingDirectory = newGameDir;
            project.StartInfo.FileName = "project.godot";

            try
            {
                project.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.ReadKey();
            }
        }

        private static void SetupFailed()
        {
            Console.WriteLine("Setup failed.");
            Console.ReadKey();
        }

        private static void WriteFile(string path, string content)
        {
            try
            {
                File.WriteAllText(path, content);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private static void CopyFile(string source, string target)
        {
            if (File.Exists(source) && !File.Exists(target))
            {
                try
                {
                    File.Copy(source, target);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (!Directory.Exists(target.FullName))
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        private static string GetSlnText(string gameName)
        {
            return $@"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 2012
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""{gameName}"", ""{gameName}.csproj"", ""{{EF7750CA-ADAC-429F-B254-C911A85B5F35}}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
	Debug|Any CPU = Debug|Any CPU
	Release|Any CPU = Release|Any CPU
	Tools|Any CPU = Tools|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{{EF7750CA-ADAC-429F-B254-C911A85B5F35}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{EF7750CA-ADAC-429F-B254-C911A85B5F35}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{EF7750CA-ADAC-429F-B254-C911A85B5F35}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{EF7750CA-ADAC-429F-B254-C911A85B5F35}}.Release|Any CPU.Build.0 = Release|Any CPU
		{{EF7750CA-ADAC-429F-B254-C911A85B5F35}}.Tools|Any CPU.ActiveCfg = Tools|Any CPU
		{{EF7750CA-ADAC-429F-B254-C911A85B5F35}}.Tools|Any CPU.Build.0 = Tools|Any CPU
	EndGlobalSection
EndGlobal
";
        }

        private static string GetLaunchJsonText()
        {
            return $@"{{
    ""version"": ""0.2.0"",
    ""configurations"": [
        {{
            ""name"": ""Attach"",
            ""type"": ""mono"",
            ""request"": ""attach"",
            ""address"": ""localhost"",
            ""port"": 23685
        }}
    ]
}}";
        }
    }
}
