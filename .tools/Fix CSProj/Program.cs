using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Fix_CSProj
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var pathToCurrentDir = Directory.GetCurrentDirectory();

            /* Get game name */
            string gameName;
            if (args.Length == 0) { gameName = GetGameName(pathToCurrentDir); }     // Get game name from project.godot file
            else                  { gameName = args[0]; }

            if (gameName == string.Empty)
            {
                EditFailed();
                return;
            }
            else
            {
                Console.WriteLine($"Game name: {gameName}");
            }

            /* These folders will be ignored when looking for scripts that should go in the csproj file */
            var blacklist = new List<string>();
            blacklist.Add($"{pathToCurrentDir}\\.git");
            blacklist.Add($"{pathToCurrentDir}\\.docs");
            blacklist.Add($"{pathToCurrentDir}\\.import");
            blacklist.Add($"{pathToCurrentDir}\\.mono");
            blacklist.Add($"{pathToCurrentDir}\\.setup new game");
            blacklist.Add($"{pathToCurrentDir}\\.tools");
            blacklist.Add($"{pathToCurrentDir}\\mono");

            /* Get filepaths to all scripts in the game */
            Console.WriteLine($"Searching for .cs files here: {pathToCurrentDir}");
            var pathToCurrentDirRegexPattern = pathToCurrentDir.Replace(@"\", @"\\") + @"\\";       // Result example: D:\\Projects\\My Game\\
            var scriptFilepaths = GetScriptFilepaths(pathToCurrentDir, pathToCurrentDirRegexPattern, blacklist);

            if(scriptFilepaths.Count == 0)
            {
                Console.WriteLine("No .cs files found.");
                EditFailed();
                return;
            }
            else
            {
                Console.WriteLine($"{scriptFilepaths.Count} .cs files found.");
            }

            /* Create CSProj text */
            StringBuilder csprojText = new StringBuilder();
            csprojText.Append(GetCSProjPart1(gameName));
            foreach(var script in scriptFilepaths)
            {
                csprojText.AppendLine($"\t<Compile Include=\"{script}\" />");
            }
            csprojText.Append(GetCSProjPart2());

            /* Create CSProj file */
            var pathToCSProjFile = Path.Combine(pathToCurrentDir, $"{gameName}.csproj");
            Console.WriteLine($"Creating a new csproj file here: {pathToCSProjFile}");

            try
            {
                File.WriteAllText(pathToCSProjFile, csprojText.ToString());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                EditFailed();
                return;
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

        /// <summary> Returns a list of filepaths to all the .cs files in the game's folders. </summary>
        private static List<string> GetScriptFilepaths(string path, string rootPathPattern, List<string> blacklist)
        {
            List<string> results = new List<string>();
            GetScriptFilepaths(path, rootPathPattern, blacklist, results);
            return results;
        }

        /// <summary> Returns a list of filepaths to all the .cs files in the game's folders. </summary>
        private static List<string> GetScriptFilepaths(string path, string rootPathPattern, List<string> blacklist, List<string> returnList)
        {
            try
            {
                foreach (string dir in Directory.GetDirectories(path))
                {
                    if (!IsFolderBlacklisted(dir, blacklist))
                    {
                        foreach (string file in Directory.GetFiles(dir))
                        {
                            if (file.EndsWith(".cs"))
                            {
                                string fileNameWithoutRootPath = Regex.Replace(file, rootPathPattern, string.Empty);
                                returnList.Add(fileNameWithoutRootPath);
                            }
                        }
                        GetScriptFilepaths(dir, rootPathPattern, blacklist, returnList);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return returnList;
        }

        private static bool IsFolderBlacklisted(string folderName, List<string> blacklist)
        {
            foreach (var blacklistedFolder in blacklist)
            {
                if (folderName.Equals(blacklistedFolder))
                {
                    return true;
                }
            }
            return false;
        }

        private static string GetCSProjPart1(string gameName)
        {
            var gameNameNoSpace = gameName.Replace(" ", "");

            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid></ProjectGuid>
    <OutputType>Library</OutputType>
    <OutputPath>.mono\temp\bin\$(Configuration)</OutputPath>
    <RootNamespace>{gameNameNoSpace}</RootNamespace>
    <AssemblyName>{gameName}</AssemblyName>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
    <BaseIntermediateOutputPath>.mono\temp\obj</BaseIntermediateOutputPath>
    <IntermediateOutputPath>$(BaseIntermediateOutputPath)\$(Configuration)</IntermediateOutputPath>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>portable</DebugType>
    <Optimize>false</Optimize>
    <DefineConstants>DEBUG;</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <ConsolePause>false</ConsolePause>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <DebugType>portable</DebugType>
    <Optimize>true</Optimize>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <ConsolePause>false</ConsolePause>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Tools|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>portable</DebugType>
    <Optimize>false</Optimize>
    <DefineConstants>DEBUG;TOOLS;</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <ConsolePause>false</ConsolePause>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""GodotSharp"">
      <HintPath>$(ProjectDir)\.mono\assemblies\GodotSharp.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include=""GodotSharpEditor"" Condition="" '$(Configuration)' == 'Tools' "">
      <HintPath>$(ProjectDir)\.mono\assemblies\GodotSharpEditor.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include=""System"" />
  </ItemGroup>
  <ItemGroup>
";
        }

        private static string GetCSProjPart2()
        {
            return @"  </ItemGroup>
  <Import Project=""$(MSBuildBinPath)\Microsoft.CSharp.targets"" />
</Project>";
        }
    }
}
