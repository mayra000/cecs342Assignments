using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using System.IO;

namespace CECS342_Assignment3
{
  class Program
  {
    static IEnumerable<string> EnumerateFilesRecursively(string path)
    {
      IEnumerable<string> allFiles = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories); //return IEnumerable<string> 
      foreach (string file in allFiles)
      {
        yield return file;
      }
    }

    //this still need to be written
    // Format a byte size in human readable form. Use the following units: B, kB, MB, GB, TB, PB,
    // EB, and ZB where 1kB = 1000B. The numerical value should be greater or equal to 1, less
    // than 1000 and formatted with 2 fixed digits after the decimal point, e.g. 1.30kB
    static string FormatByteSize(long byteSize)
        {
            // Make sure to "TypeCast" because Math.Pow returns a double.
            //declaring variables
            long KB;
            long MB;
            long GB;
            long TB;
            long PB; 
            long EB;
            long ZB;
            KB = (long)Math.Pow(10, 3);
            MB = (long)Math.Pow(10, 6);
            GB = (long)Math.Pow(10, 9);
            TB = (long)Math.Pow(10, 12);
            PB = (long)Math.Pow(10, 15);
            EB = (long)Math.Pow(10, 18);
            ZB = (long)Math.Pow(10, 21);

            // Use numValue to hold the result 
            double numValue = 0;

            // Use temp to hold the modified byteSize
            long temp = 0;

            long sixtyFourBitInteger = 9223372036854775807;

            /*
            1st Test Case
            byteSize = 0
            */
            if (byteSize == 0)
            {
                return "0" + "B";
            }

            /*
            2nd Test Case
            0B < byteSize <= 1,000B 
            0B < byteSize <= KB
            */
            else if (byteSize > 0 && byteSize <= KB)
            {
                numValue = Convert.ToDouble(byteSize);
                return numValue.ToString("#.##") + "B";
            }

            /*
            3rd Test Case
            1000B <= byteSize < 1,000,000B
            KB <= byteSize < MB
            */
            else if (byteSize >= KB && byteSize < MB)
            {
                temp = byteSize / KB;
                numValue = Convert.ToDouble(temp);
                return numValue.ToString("#.##") + "KB";
            }

            /*
            4th Test Case:
            1,000,000B < byteSize <= 1,000,000,000B
            MB < byteSize <= GB
            */
            else if (byteSize >= MB && byteSize < GB)
            {
                temp = byteSize / MB;
                numValue = Convert.ToDouble(temp);
                return numValue.ToString("#.##") + "MB";
            }

            /*
            5th Test Case:
            1,000,000,000B < byteSize <= 1,000,000,000,000B
            GB < byteSize <= TB
            */
            else if (byteSize >= GB && byteSize < TB)
            {
                temp = byteSize / GB;
                numValue = Convert.ToDouble(temp);
                return numValue.ToString("#.##") + "GB";
            }

            /*
            6th Test Case:
            1,000,000,000,000B < byteSize <= 1,000,000,000,000,000B
            TB < byteSize <= PB
            */
            else if (byteSize >= TB && byteSize < PB)
            {
                temp = byteSize / TB;
                numValue = Convert.ToDouble(temp);
                return numValue.ToString("#.##") + "TB";
            }

            /*
            7th Test Case:
            1,000,000,000,000,000B < byteSize <= 1,000,000,000,000,000,000B
            PB < byteSize <= EB
            */
            else if (byteSize >= PB && byteSize < EB)
            {
                temp = byteSize / PB;
                numValue = Convert.ToDouble(temp);
                return numValue.ToString("#.##") + "PB";
            }

            /*
            8th Test Case:
            Long Maximum Value for 64-bit signed integer = 9,223,372,036,854,775,807
            1,000,000,000,000,000,000B < byteSize <= 9,223,372,036,854,775,807B
            EB < byteSize <= 9,223,372,036,854,775,807B
            */
            else if (byteSize >= EB && byteSize <= sixtyFourBitInteger)
            {
                temp = byteSize / EB;
                numValue = Convert.ToDouble(temp);
                return numValue.ToString("#.##") + "EB";
            }
            //default
            else
            {
                return null;
            }
        } // end of "FormatByteSize"

    static XDocument CreateReport(IEnumerable<string> files)
    {
      //GroupBy file extenstion out IEnumerable object with type, count, and size field sort by size descending
      var fileType = files.GroupBy(f => Path.GetExtension(f)).Select(
        g => new { type = g.Key, count = g.Count(), size = g.Sum(x => new FileInfo(x).Length) }).OrderByDescending(o => o.size);
      //functional way of generating xDocument
      return new XDocument(
        new XDocumentType("html", null, null, null),
        new XElement("html",
          new XAttribute("lang", "en"),
          new XElement("head",
            new XElement("meta",
              new XAttribute("charset", "utf-8")
            ),
            new XElement("style", "table, th, td {border: 1px solid black; border-collapse: collapse;} th, td {padding: 5px; text-align: left;}table{width:50%;}")
            ),
          new XElement("body",
            new XElement("table",
              new XElement("tr",
                new XElement("th", "Type"),
                new XElement("th", "Count"),
                new XElement("th", "Size")
              ),
              from file in fileType
              select new XElement("tr",
                new XElement("td", file.type),
                new XElement("td", file.count.ToString()),
                new XElement("td", file.size.ToString())
              )
            )
          )
        )
      );
    }
    static void Main(string[] args)
    {
      var allFiles = EnumerateFilesRecursively(args[0]);
      XDocument report = CreateReport(allFiles);
      report.Save(args[1]);
    }
  }
}
