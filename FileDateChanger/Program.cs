using System;
using System.ComponentModel;
using System.IO;

namespace FileDateChanger
{
    internal class Program
    {
        [DefaultValue(PathKind.Invalid)]
        enum PathKind
        {
            Invalid = -1,
            File = 0,
            Directory = 1,
        }

        [DefaultValue(DateChangeKind.Invalid)]
        enum DateChangeKind
        {
            Invalid = -1,
            Created = 0,
            Modified = 1,
            Accessed = 2,
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("File Date Changer");
                Console.WriteLine("Version 1.0");
                Console.WriteLine();

                try
                {
                    //get path type
                    PathKind pathKind;
                    do
                    {
                        Console.Write("File or Directory (F/D):");
                    } while (!TryParseFileFolderKind(Console.ReadLine(), out pathKind));
                    Console.WriteLine();

                    //get path
                    string path;
                    do
                    {
                        Console.Write("Path:");
                    } while (!IsPathValid(Console.ReadLine(), pathKind, out path));
                    Console.WriteLine();

                    string searchPattern = "*";
                    if (pathKind == PathKind.Directory)
                    {
                        Console.WriteLine("Filter string. Just press enter if you want to collect all files in the directory.");
                        Console.WriteLine("*<name>* = Only files containing the <name> string will be collected");
                        Console.WriteLine("<name>* = Only files starting with the <name> string will be collected");
                        Console.WriteLine("<name> = Only the file with the name of <name> will be collected");
                        Console.Write("Filter:");

                        searchPattern = Console.ReadLine() ?? "*";

                        Console.WriteLine();
                    }

                    //list files and count
                    string[] files = GetFiles(path, searchPattern, pathKind);
                    for (int i = 0; i < files.Length; i++)
                    {
                        Console.WriteLine(Path.GetFileName(files[i]));
                    }
                    Console.WriteLine();
                    Console.WriteLine($"{files.Length} file(s) found!");

                    //get date change type
                    DateChangeKind dateChangeKind;
                    Console.WriteLine("Which attribute to change (Created, Modified, or Accessed)");
                    do
                    {
                        Console.Write("Type: ");
                    } while (!TryParseDateChangeKind(Console.ReadLine(), out dateChangeKind));
                    Console.WriteLine();

                    //get date
                    DateTime date;
                    Console.WriteLine("New local date to set. Format: yyyy-MM-dd");
                    do
                    {
                        Console.Write("Date:");
                    } while (!DateTime.TryParse(Console.ReadLine(), out date));
                    Console.WriteLine();

                    //get time
                    TimeSpan time;
                    Console.WriteLine("New local time to set. Format: hh:mm:ss.fff Example: 07:45:13.000");
                    do
                    {
                        Console.Write("Time:");
                    } while (!TimeSpan.TryParse(Console.ReadLine(), out time));
                    Console.WriteLine();
                    date.Add(time);

                    //apply date change
                    for (int i = 0; i < files.Length; i++)
                    {
                        SetDate(files[i], date, dateChangeKind, true);
                    }
                    Console.WriteLine();
                    Console.WriteLine($"File dates changed for {files.Length} files");
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        static void SetDate(string path, DateTime date, DateChangeKind kind, bool writeLog)
        {
            Console.WriteLine(Path.GetFileName(path));
            string format = "yyyy-MM-dd hh:mm:ss.fff tt";
            switch (kind)
            {
                case DateChangeKind.Created:
                    Console.Write($" Old: [{File.GetCreationTime(path).ToString(format)}]");
                    File.SetCreationTime(path, date);
                    Console.Write($" New: [{File.GetCreationTime(path).ToString(format)}]");
                    break;
                case DateChangeKind.Modified:
                    Console.Write($" Old: [{File.GetLastWriteTime(path).ToString(format)}]");
                    File.SetLastWriteTime(path, date);
                    Console.Write($" New: [{File.GetLastWriteTime(path).ToString(format)}]");
                    break;
                case DateChangeKind.Accessed:
                    Console.Write($" Old: [{File.GetLastAccessTime(path).ToString(format)}]");
                    File.SetLastAccessTime(path, date);
                    Console.Write($" New: [{File.GetLastAccessTime(path).ToString(format)}]");
                    break;
            }
            Console.WriteLine();
        }

        static string[] GetFiles(string path, string filter, PathKind kind)
        {
            if (kind == PathKind.File)
                return new string[] { path };
            else if (kind == PathKind.Directory)
                return Directory.GetFiles(path, filter);
            else
                return Array.Empty<string>();
        }

        static bool IsPathValid(string? path, PathKind kind, out string? fullPath)
        {
            try
            {
                fullPath = Path.GetFullPath(path?.Trim(' ', '\"'));
            }
            catch
            {
                fullPath = "";
                return false;
            }

            if (kind == PathKind.File)
                return File.Exists(fullPath);
            else if (kind == PathKind.Directory)
                return Directory.Exists(fullPath);

            return false;
        }

        static bool TryParseFileFolderKind(string? str, out PathKind kind)
        {
            if (str == null)
            {
                kind = default(PathKind);
                return false;
            }
            else if (str.Equals("file", StringComparison.OrdinalIgnoreCase) || str.Equals("f", StringComparison.OrdinalIgnoreCase))
            {
                kind = PathKind.File;
                return true;
            }
            else if (str.Equals("directory", StringComparison.OrdinalIgnoreCase) || str.Equals("d", StringComparison.OrdinalIgnoreCase))
            {
                kind = PathKind.Directory;
                return true;
            }
            else
            {
                kind = default(PathKind);
                return false;
            }
        }

        static bool TryParseDateChangeKind(string? str, out DateChangeKind kind)
        {

            if (str == null)
            {
                kind = default(DateChangeKind);
                return false;
            }
            else if (str.Equals("created", StringComparison.OrdinalIgnoreCase))
            {
                kind = DateChangeKind.Created;
                return true;
            }
            else if (str.Equals("modified", StringComparison.OrdinalIgnoreCase))
            {
                kind = DateChangeKind.Modified;
                return true;
            }
            else if (str.Equals("accessed", StringComparison.OrdinalIgnoreCase))
            {
                kind = DateChangeKind.Accessed;
                return true;
            }
            else
            {
                kind = default(DateChangeKind);
                return false;
            }
        }
    }
}