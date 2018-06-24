namespace CsFileContentGrabbes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary> Сервис для сбора исходного кода дипломной работы </summary>
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var rootDirectory = @"E:\Stash\diplom\EC_Console";
            var csFileNames = Directory.GetFiles(rootDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(fileName => !fileName.Contains(@"\obj\") && !fileName.Contains(@"\bin\") &&
                                   !fileName.Contains(@"AssemblyInfo"))
                .Select(fileName => new
                {
                    FileName = fileName,
                    MainPart = fileName.Substring(27)
                })
                .Select(dto =>
                {
                    var words = dto.MainPart.Split('\\');

                    return new
                    {
                        ProjectName = words[0],
                        RelativeFileName = string.Join("\\", words.Skip(1)),
                        dto.FileName,
                        Link = @"https://github.com/Kamil-Zakiev/diplom/blob/new_magister_diplom/EC_Console/" + dto.MainPart.Replace('\\', '/')
                    };
                });
            
            var groups = csFileNames.GroupBy(x => x.ProjectName);

            var sourceCodeFile = @"E:\Stash\diplom\CsFileContentGrabbes\CsFileContentGrabbes\SourceCode.txt";
            
            var lines = new LinkedList<string>();
            lines.AddFirst("Код актуален на дату: " + DateTime.Now.ToShortDateString());
            lines.AddLast(string.Empty);
            void WriteLine(string str)
            {
                lines.AddLast(str);
            }
            
            foreach (var group in groups)
            {
                WriteLine("Проект \"" + group.Key + "\"");
                var projFiles = group.ToList();
                
                foreach (var projFile in projFiles)
                {
                    WriteLine(projFile.RelativeFileName);
                    WriteLine(projFile.Link);
                    
                    foreach (var line in File.ReadAllLines(projFile.FileName))
                    {
                        WriteLine(line);
                    }
                    
                    WriteLine(string.Empty);
                }

                WriteLine(string.Empty);
                WriteLine(string.Empty);
            }
            
            File.WriteAllLines(sourceCodeFile, lines);
        }
    }
}