using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace TreeSizeClone
{
    class Program
    {
        static void Main(string[] args)

        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            Console.ForegroundColor= ConsoleColor.DarkRed;

            

            Console.WriteLine("Treesize is a program to monitor and analyze how your computer's memory is managed.");
            Console.Write("Please enter the path for analyse:");
            string targetPath = Console.ReadLine();
            try
            {
                MyDirectory directory = new MyDirectory(targetPath);
                Console.WriteLine($"{directory.Name}:{directory.SizeRelative} {directory.SizeUnit} ({directory.Subdirectories.Length} folders, {directory.Files.Length} files)");
                PrintSubDirectories(directory);
                PrintFiles(directory.Files, "");
            }
            catch (UnauthorizedAccessException) {
                Console.WriteLine("\nSorry, I have not access to this path :(");
            }
            

        }

        public static void PrintSubDirectories(MyDirectory myDirectory, string indent = "")
        {
            foreach (var subdirectory in myDirectory.Subdirectories)
            {
                Console.WriteLine($"|{indent}---+{subdirectory.Name} [{subdirectory.SizeRelative}{subdirectory.SizeUnit}]");
                PrintFiles(subdirectory.Files, $"{indent}   ");
                Thread.Sleep(75);
                PrintSubDirectories(subdirectory, $"{indent}    |");
                
            }
        }

        public static void PrintFiles(MyFile[] filesOfDirectory, string indent)
        {
            foreach (var file in filesOfDirectory)
            {
                Console.WriteLine($"|{indent} |--{file.Name} [{file.SizeRelative}{file.SizeUnit}]");
                Thread.Sleep(200);
            }
        }


        public static string[] SizeConverter(long size)
        {
            if (size < 0)
            {
                throw new ArgumentException("Size cannot be negative.");
            }

            string[] units = { "byte", "KB", "MB", "GB", "TB" };
            int unitIndex = 0;
            double sizeDouble = Convert.ToDouble(size);
            while (sizeDouble >= 1024 && unitIndex < units.Length - 1)
            {

                sizeDouble /= 1024;
                unitIndex++;
            }

            return new string[] { sizeDouble.ToString("F2"), units[unitIndex] };
        }




    }



    public class MyDirectory
    {
        public string Path;
        public string Name;
        public MyFile[] Files;
        public long SizeAbsolute;
        public double SizeRelative;
        public string SizeUnit;

        public MyDirectory[] Subdirectories;

        public MyDirectory(string _Path)
        {
            this.Path = _Path;
            this.Name = new DirectoryInfo(Path).Name;
            this.Files = CreateFileArray(this.Path);
            Array.Sort(this.Files, new SizeAbsoluteComparer());
            this.SizeAbsolute = GetFileArraySize(this.Files);
            this.Subdirectories = CreateSubDirectoryArray(Path);
            this.SizeAbsolute += GetSubdirectoriesArraySize(this.Subdirectories);
            Array.Sort(this.Subdirectories, new SizeAbsoluteComparerForDirectories());
            string[] sizeProps = Program.SizeConverter(this.SizeAbsolute);
            this.SizeRelative = double.Parse(sizeProps[0]);
            this.SizeUnit = sizeProps[1];
        }



        public static MyFile[] CreateFileArray(string _directory)
        {

            var files = Directory.GetFiles(_directory);
            var fileArray = new MyFile[files.Length];
            int i = 0;
            foreach (var filePath in files)
            {

                var file = new MyFile(filePath);
                fileArray[i] = file;
                i++;

            }

            return fileArray;
        }

        public static long GetFileArraySize(MyFile[] fileArray)
        {
            long sizeOfFiles = 0;
            foreach (var files in fileArray)
            {
                sizeOfFiles += files.SizeAbsolute;
            }
            return sizeOfFiles;
        }

        public static MyDirectory[] CreateSubDirectoryArray(string _dictionary)
        {
            var subDirectories = Directory.GetDirectories(_dictionary);
            var subDirectoriesInMyDictionary = new MyDirectory[subDirectories.Length];
            int i = 0;
            foreach (var subDirectory in subDirectories)
            {
                subDirectoriesInMyDictionary[i] = new MyDirectory(subDirectory);
                i++;
            }
            return subDirectoriesInMyDictionary;

        }

        public static long GetSubdirectoriesArraySize(MyDirectory[] SubdirectoryArray)
        {
            long sizeOfSubDirectories = 0;

            foreach (var subdirectory in SubdirectoryArray)
            {

                sizeOfSubDirectories += subdirectory.SizeAbsolute;

            }
            return sizeOfSubDirectories;
        }


    }

    public class MyFile
    {
        public string Name;
        public double SizeRelative;
        public long SizeAbsolute;
        public string SizeUnit;
        public MyFile(string filePath)
        {
            this.Name = new FileInfo(filePath).Name;
            this.SizeAbsolute = new FileInfo(filePath).Length;
            string[] sizeProps = Program.SizeConverter(this.SizeAbsolute);
            this.SizeRelative = double.Parse(sizeProps[0]);
            this.SizeUnit = sizeProps[1];
        }


    }

    public class SizeAbsoluteComparer : IComparer<MyFile>
    {
        public int Compare(MyFile x, MyFile y)
        {
            return y.SizeAbsolute.CompareTo(x.SizeAbsolute);
        }
    }

    public class SizeAbsoluteComparerForDirectories : IComparer<MyDirectory>
    {
        public int Compare(MyDirectory x, MyDirectory y)
        {
            return y.SizeAbsolute.CompareTo(x.SizeAbsolute);
        }
    }


}