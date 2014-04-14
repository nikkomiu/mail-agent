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

        public FileMan()
        {
            this.FilePath = string.Empty;
            this.fileContents = string.Empty;
        }

        public FileMan(string filePath)
        {
            this.FilePath = filePath;
            this.fileContents = string.Empty;
        }

        public void Read()
        {
            StreamReader reader = new StreamReader(this.FilePath);

            this.fileContents = reader.ReadToEnd();

            reader.Close();
        }

        public void Save()
        {
            // Throw exception if there is no path to file
            if (this.FilePath.Length == 0)
                throw new FileNotFoundException();

            // Create the file if it doesnt exist
            if (!File.Exists(this.FilePath))
                File.Create(this.FilePath);

            StreamWriter writer = new StreamWriter(this.FilePath);

            // Write file contents
            writer.Write(this.fileContents);

            // Close file writer
            writer.Flush();
            writer.Close();
        }

        public void Append(string appendString)
        {
            if (this.fileContents.Length == 0)
                this.Read();
            this.fileContents += appendString;
        }

        public void Prepend(string prependString)
        {
            if (this.fileContents.Length == 0)
                this.Read();
            this.fileContents = prependString + this.fileContents;
        }

        public static FileMan LocalFile(string filePath)
        {
            return new FileMan(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + filePath);
        }
    }
}
