using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace DutchServisMCV.Logic
{
    public class SaveFaildException : IOException
    {
        public SaveFaildException(string msg) : base(msg) { }
    }

    public class FileManager
    {
        public static void Save(HttpPostedFileBase file, string path)
        {
            if (File.Exists(path))
            {
                throw new SaveFaildException("Plik o nazwie "+ Path.GetFileName(path) + " już znajduje się na serwerze");
            }
            else
            {
                try
                {
                    file.SaveAs(path);
                }
                catch (DirectoryNotFoundException)
                {
                    throw new SaveFaildException("Wystąpił niespodziewany błąd (Nieprawidłowa ścieżka pliku)");
                }
            }
        }
        public static void Remove(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else throw new FileNotFoundException("Plik o nazwie " + Path.GetFileName(path) + " nie istnieje");
        }

        public static SResponse FileExtIsValid(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string ext = Path.GetExtension(file.FileName);
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                {
                    return new SResponse(false, "Przesłany plik nie posiada akceptowanego rozszerzenia");
                }
            }
            return new SResponse(true, "");
        }
    }
}