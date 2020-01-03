using System;
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
            Directory.CreateDirectory(newGameDir);

            /* Set filepaths to the project files */
            string gitignore        = $"{newGameSetupDir}\\.gitignore";
            string defaultEnv       = $"{primeDir}\\default_env.tres";
            string icon             = $"{primeDir}\\icon.png";
            string iconImport       = $"{primeDir}\\icon.png.import";
            string godotProjectFile = $"{primeDir}\\project.godot";
            string fixCSProj        = $"{primeDir}\\Fix csproj.exe";
            
            string newGitignore         = $"{newGameDir}\\.gitignore";
            string newDefaultEnv        = $"{newGameDir}\\default_env.tres";
            string newIcon              = $"{newGameDir}\\icon.png";
            string newIconImport        = $"{newGameDir}\\icon.png.import";
            string newGodotProjectFile  = $"{newGameDir}\\project.godot";
            string newFixCSProj         = $"{newGameDir}\\Fix csproj.exe";

            /* Copy project files to new game directory */
            if (File.Exists(gitignore) && !File.Exists(newGitignore))               { File.Copy(gitignore, newGitignore); }
            if (File.Exists(defaultEnv) && !File.Exists(newDefaultEnv))             { File.Copy(defaultEnv, newDefaultEnv); }
            if (File.Exists(icon) && !File.Exists(newIcon))                         { File.Copy(icon, newIcon); }
            if (File.Exists(iconImport) && !File.Exists(newIconImport))             { File.Copy(iconImport, newIconImport); }
            if (File.Exists(godotProjectFile) && !File.Exists(newGodotProjectFile)) { File.Copy(godotProjectFile, newGodotProjectFile); }
            if (File.Exists(fixCSProj) && !File.Exists(newFixCSProj))               { File.Copy(fixCSProj, newFixCSProj); }

            /* Copy "Game" folder*/
            CopyAll(new DirectoryInfo($"{primeDir}\\Game"), new DirectoryInfo($"{newGameDir}\\Game"));

            /* Set the name of the game in project.godot */
            string text = File.ReadAllText(newGodotProjectFile);
            text = text.Replace($"config/name=\"{PrimeFrameworkProjectName}\"", $"config/name=\"{gameName}\"");
            File.WriteAllText(newGodotProjectFile, text);

            /* Open Command Prompt and junction "Framework" folder to new game folder */
            string cmd = $"/C mklink /J \"{newGameDir}\\Framework\" \"{primeDir}\\Framework\"";
            System.Diagnostics.Process.Start("CMD.exe", cmd);

            Console.WriteLine($"\nCreated new folder: {newGameDir}\n");
            Console.WriteLine("To complete setup:");
            Console.WriteLine("  1: Open your new project in Godot");
            Console.WriteLine("  2: Run Fix CSProj.exe");
            Console.WriteLine("\nPress any key to quit.");
            Console.ReadKey();
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
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
    }
}
