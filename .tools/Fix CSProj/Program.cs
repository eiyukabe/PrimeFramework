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
            var currentDir = Directory.GetCurrentDirectory();

            /* Get filepaths to all scripts in the game */
            Console.WriteLine($"Searching for script files at {currentDir}");
            string rootPathPattern = currentDir.Replace(@"\", @"\\") + @"\\";   // Regular expression for the path to the root folder
            var success = SetScriptFilepaths(currentDir, rootPathPattern);

            if (!success)
            {
                Console.WriteLine("Edit failed");
                Console.ReadKey();
                return;
            }

            if (ScriptFilepaths.Count == 0)
            {
                Console.WriteLine("No scripts found.");
                Console.WriteLine("Edit failed");
                Console.ReadKey();
                return;
            }

            /* Create lines to write to csproj file */
            for (int i = 0; i < ScriptFilepaths.Count; i++)
            {
                ScriptIncludes.Add($"\t<Compile Include=\"{ScriptFilepaths[i]}\" />");
            }

            /* Edit the csproj file */
            success = EditProjFile();
            if (!success)
            {
                Console.WriteLine("Edit failed");
                Console.ReadKey();
            }

            // Console.ReadKey();
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
        private static bool EditProjFile()
        {
            var gameFullPath = Directory.GetCurrentDirectory();
            var gameName = new DirectoryInfo(gameFullPath).Name;
            var csprojPath = $"{gameFullPath}\\{gameName}.csproj";

            /* Make sure csproj file exists */
            Console.WriteLine($"Looking for csproj file at {csprojPath}.");
            if (!File.Exists(csprojPath))
            {
                Console.WriteLine("csproj file not found.");
                return false;
            }

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
