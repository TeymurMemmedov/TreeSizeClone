using System.Runtime.CompilerServices;
using System.Text;

namespace TreeSizeClone
{
    class Program
    {
        static void Main(string[] args)

        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding= Encoding.Unicode;

            Console.WriteLine("Treesize is a program to monitor and analyze how your computer's memory is managed.");
            Console.Write("Please enter the path for analyse:");
            string targetPath = Console.ReadLine();
            MyDirectory directory = new MyDirectory(targetPath);


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
            string[] sizeProps = Program.SizeConverter(this.SizeAbsolute);
            this.SizeRelative = double.Parse(sizeProps[0]);
            this.SizeUnit = sizeProps[1];
            PrintDirectory(this);

        }

        public static void PrintDirectory(MyDirectory directory)
        {
            string defis = "-";
            string under = "|";
            Console.WriteLine($"{directory.Name}:{directory.SizeRelative} {directory.SizeUnit}");

            Thread.Sleep(500);

            foreach (var file in directory.Files)
            {
                Console.Write(under);
                Console.WriteLine($"{string.Concat(Enumerable.Repeat(defis, 4))}{file.Name} : {file.SizeRelative} {file.SizeUnit}");
                Thread.Sleep(50);
            }
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


}