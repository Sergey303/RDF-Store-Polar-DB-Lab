using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFTripleStore
{
  public static class Config
    {
      public static string Source_data_folder_path;

      static Config()
      {
          using (StreamReader file=new StreamReader("../../../config.ini"))
          {
              while (!file.EndOfStream)
              {
                   var readLine = file.ReadLine();
                  if (readLine.StartsWith("#source_data_folder_path"))
                      Source_data_folder_path = readLine.Replace("#source_data_folder_path","").Trim();
              }
              if (!Source_data_folder_path.EndsWith("\\") &&
                  !Source_data_folder_path.EndsWith("/")) 
                  Source_data_folder_path += "/";
          }
      }
    }
}
