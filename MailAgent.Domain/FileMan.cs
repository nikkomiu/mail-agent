﻿using System;
using System.IO;
using System.Xml.Serialization;

namespace MailAgent.Domain
{
    public class FileMan
    {
        // Public file path
        public string FilePath { get; set; }
        
        // Read-only public file contents
        public string FileContents
        {
            get
            {
                return this._fileContents;
            }
        }

        // Internal file contents
        private string _fileContents;

        public FileMan() : this(string.Empty)
        {
        }

        public FileMan(string filePath)
        {
            this.FilePath = filePath;
            this._fileContents = string.Empty;
        }

        public void Read()
        {
            try
            {
                string directory = this.FilePath.Substring(0, this.FilePath.LastIndexOf('\\'));

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                this._fileContents = File.ReadAllText(this.FilePath);
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
            writer.Write(this._fileContents);

            // Close file writer
            writer.Flush();
            writer.Close();
        }

        public void Append(string appendString)
        {
            string directory = this.FilePath.Substring(0, this.FilePath.LastIndexOf('\\'));

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.AppendAllText(this.FilePath, appendString);
        }

        public void Delete()
        {
            File.Delete(this.FilePath);
        }

        public void Serialize<T>(T objectToSerialize)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StreamWriter writer = new StreamWriter(this.FilePath))
            {
                serializer.Serialize(writer, objectToSerialize);
            }
        }

        public T Deserialize<T>()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            T output;

            using (StreamReader stream = new StreamReader(this.FilePath))
            {
                output = (T)serializer.Deserialize(stream);
            }

            return output;
        }

        public static FileMan LocalFile(string filePath)
        {
            return new FileMan(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + filePath);
        }
    }
}
