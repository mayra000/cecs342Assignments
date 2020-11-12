using System;
using System.Collections.Generic; //IEnumerable
using System.IO; //Directory
using System.Xml.Linq; //XDocument & XElement
using System.Linq;

namespace CECS342_Assignment3
{
    class Program
    {

        static IEnumerable<string> EnumerateFilesRecursively(string path) {

            IEnumerable<string> allFiles = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories); //return IEnumerable<string> 
            
            foreach (string file in allFiles) yield return file;
        }

        static string FormatByteSize(long byteSize) {

            string[] unit = {"B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB"};
            double formatSize = (double) byteSize;
            int temp = 0;

            while (formatSize >= 1024.0 && temp < unit.Length) {
                formatSize /= 1024.0;
                temp++;
            }

            return Math.Round(formatSize, 2).ToString() + " " + unit[temp];
        }

        static XDocument CreateReport(IEnumerable<string> files) {

            //GroupBy file extenstion out IEnumerable object with type, count, and size field sort by size descending
            var fileType = files.GroupBy(f => Path.GetExtension(f)).Select(
                g => new { type = g.Key, count = g.Count(), 
                    size = FormatByteSize(g.Sum(x => new FileInfo(x).Length)) })
                    .OrderByDescending(o => o.size);

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
                        new XElement("td", file.size)
                    )
                    )
                )
                )
            );
        }

        static void Main(string[] args) {
            Console.WriteLine("Starting...");
            var allFiles = EnumerateFilesRecursively(args[0]);
            XDocument report = CreateReport(allFiles);
            report.Save(args[1]);
            Console.WriteLine("Report Created!");
        }
    }
}
