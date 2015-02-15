using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace MathEasy.src.FileManagement
{
    public static class FileManager
    {
        public static void WriteDataToFile(DataStream data, String fullPath)
        {
            FileStream fs=null;
            StreamWriter sw = null;
            try
            {
                fs = File.Create(fullPath);
                sw = new StreamWriter(fs);
                sw.Write(data.Content);
                sw.Close();
                fs.Close();
            }
            catch (System.IO.IOException)
            {
                if (sw!=null)
                    sw.Close();
                if (fs != null)
                    fs.Close();
                throw new Exception("An error occured while performing operation of writing data to file at: " + fullPath);
            }
            catch (Exception)
            {
                if (sw != null)
                    sw.Close();
                if (fs != null)
                    fs.Close();
                throw new Exception("An unexpected error occured, or you do not have rights to create file at path specified: " + fullPath);
            }
        }

        public static void WriteDataToFile(DataStream data, String fileName, String path)
        {
            String fullPath = (path == "") ? fileName : path + @"/" + fileName;
            if (data.TypeProperty == DataStream.Type.TXT)
                fullPath += ".txt";
            else if (data.TypeProperty == DataStream.Type.HTML)
                fullPath += ".html";

            WriteDataToFile(data, fullPath);
        }

        public static DataStream ReadDataFromFile(String fullPath)
        {
            if (!File.Exists(fullPath))
            {
                throw new Exception("File: " + fullPath + " does not exist.");
            }
            else if (!fullPath.Contains(".txt"))
            {
                throw new Exception("Only plaintext (txt) files can be loaded.");
            }
            else
            {
                try
                {
                    using (StreamReader sr = new StreamReader(fullPath))
                    {
                        String line = sr.ReadToEnd();
                        sr.Close();
                        return new DataStream(line, DataStream.Type.TXT);
                    }
                }
                catch (System.IO.IOException)
                {
                    throw new Exception("An error occured while performing operation of reading data from file at: " + fullPath);
                }
                catch (Exception)
                {
                    throw new Exception("An unexpected error occured, or you do not have rights to access file at fullPath specified:" + fullPath);
                }
            }
        }

        public static Boolean deleteFile(String fullPath)
        {
            if (!File.Exists(fullPath))
            {
                return false;
            }
            try
            {
                File.Delete(fullPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
