using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace DutchServisMCV.Logic
{
    public class OverrideException : IOException
    {
        public OverrideException(string msg) : base(msg) { }
    }

    public class FileManager
    {
        public static void Save(HttpPostedFileBase file, string path)
        {
            if (File.Exists(path))
            {
                throw new OverrideException("Plik o nazwie "+ Path.GetFileName(path) + " już znajduje się na serwerze");
            }
            else
            {
                file.SaveAs(path);
            }
        }
    }
}