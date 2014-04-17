using System;
using System.IO;

namespace Mail_Agent_Service
{
    class FileMan
    {
        // Public file path
        public string FilePath { get; set; }
        
        // Read-only public file contents
        public string FileContents
        {
            get
            {
                return this.fileContents;
            }
        }

        // Internal file contents
        private string fileContents;

        public FileMan() : this(string.Empty)
        {
        }

        public FileMan(string filePath)
        {
            this.FilePath = filePath;
            this.fileContents = string.Empty;
        }

        public void Read()
        {
            try
            {
                string directory = this.FilePath.Substring(0, this.FilePath.LastIndexOf('\\'));

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                this.fileContents = File.ReadAllText(this.FilePath);
            }
            catch (FileNotFoundException)
            {
                var newFile = File.Create(this.FilePath);
                newFile.Close();
            }
        }

        public void Save()
        {
            // Throw exception if there is no path to file
            if (this.FilePath.Length == 0)
                throw new FileNotFoundException();

            if (!File.Exists(this.FilePath))
            {
                File.Create(this.FilePath);
            }

            StreamWriter writer = new StreamWriter(this.FilePath);

            // Write file contents
            writer.Write(this.fileContents);

            // Close file writer
            writer.Flush();
            writer.Close();
        }

        public void Append(string appendString)
        {
            File.AppendAllText(this.FilePath, appendString);
        }

        public static FileMan LocalFile(string filePath)
        {
            return new FileMan(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + filePath);
        }
    }
}
