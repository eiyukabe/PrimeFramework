using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Fix_CSProj
{
    public class Program
    {
        private static List<string> ScriptFilepaths = new List<string>();   // A list of filepaths to each script in the game.
        private static List<string> ScriptIncludes = new List<string>();    // Script includes that will be written to the csproj file based on the ScriptFilepaths.

        private static void Main(string[] args)
        {
            var pathToCurrentDir = Directory.GetCurrentDirectory();

            /* Get game name from project.godot */
            var gameName = GetGameName(pathToCurrentDir);
            if (gameName == string.Empty)
            {
                EditFailed();
            }
            else
            {
                Console.WriteLine($"Fix csproj thinks the name of your game is: {gameName}");
            }

            /* Get filepaths to all scripts in the game */
            Console.WriteLine($"Searching for script files at {pathToCurrentDir}");
            var pathToCurrentDirRegexPattern = pathToCurrentDir.Replace(@"\", @"\\") + @"\\";       // Result example: D:\\Projects\\My Game\\
            var success = SetScriptFilepaths(pathToCurrentDir, pathToCurrentDirRegexPattern);

            if (!success)
            {
                EditFailed();
                return;
            }

            if (ScriptFilepaths.Count == 0)
            {
                EditFailed();
                return;
            }

            /* Create lines to write to csproj file */
            for (int i = 0; i < ScriptFilepaths.Count; i++)
            {
                ScriptIncludes.Add($"\t<Compile Include=\"{ScriptFilepaths[i]}\" />");
            }

            /* Edit the csproj file */
            Console.WriteLine(pathToCurrentDir);

            var pathToCSProjFile = Path.Combine(pathToCurrentDir, $"{gameName}.csproj");
            success = EditProjFile(pathToCSProjFile);
            if (!success)
            {
                EditFailed();
            }
        }

        private static void EditFailed()
        {
            Console.WriteLine("Edit failed.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary> Returns the name of the game that is set in project.godot. </summary>
        private static string GetGameName(string pathToCurrentDir)
        {
            var pathToGodotProjectFile = Path.Combine(pathToCurrentDir, "project.godot");
            if (File.Exists(pathToGodotProjectFile))
            {
                var text = File.ReadAllText(pathToGodotProjectFile);
                var pattern = @"config/name=""(.*)""";
                var match = Regex.Match(text, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                else
                {
                    Console.WriteLine("The name of your game could not be determined from your project.godot file.");
                    Console.WriteLine(@"Open project.godot in a text editor and make sure it has a line that says this (including quotes): config/name=""Your Game Name""");
                }
            }
            else
            {
                Console.WriteLine("project.godot file not found. This file is required to generate a new .csproj file.");
            }
            return string.Empty;
        }

        /// <summary> Iterate through the game's folders looking for .cs files and record their paths in ScriptFilepaths. Returns true if successful. </summary>
        private static bool SetScriptFilepaths(string path, string rootPathPattern)
        {
            try
            {
                foreach (string dir in Directory.GetDirectories(path))
                {
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        if (file.EndsWith(".cs"))
                        {
                            string fileNameWithoutRootPath = Regex.Replace(file, rootPathPattern, string.Empty);
                            ScriptFilepaths.Add(fileNameWithoutRootPath);
                        }
                    }
                    SetScriptFilepaths(dir, rootPathPattern);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }

            return true;
        }

        /// <summary> Attempt to edit csproj file. Returns true if successful. </summary>
        private static bool EditProjFile(string csprojPath)
        {
            /* Read csproj line by line. */
            string line;
            var lines = new List<string>();
            bool insertDone = false;
            StreamReader file = new StreamReader(csprojPath);

            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("<Compile Include="))
                {
                    if (!insertDone)
                    {
                        /* Insert script includes here */
                        for (int j = 0; j < ScriptIncludes.Count; j++)
                        {
                            lines.Add(ScriptIncludes[j]);
                        }
                        insertDone = true;
                    }
                }
                else
                {
                    lines.Add(line);    // Add csproj file lines to 'lines' list if it doesn't start with "<Compile Include="
                }
            }
            file.Close();

            /* Convert list to string builder */
            var sb = new StringBuilder();
            foreach (var l in lines)
            {
                sb.AppendLine(l);
            }

            File.WriteAllText(csprojPath, sb.ToString());
            return true;
        }

    }
}
