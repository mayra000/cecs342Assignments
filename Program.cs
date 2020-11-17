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
            
            foreach (string file in allFiles) { //iterate through the files
                yield return file; //process one file at a time
            }
        }

        static string FormatByteSize(long byteSize) {

            string[] unit = {"B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB"}; //size unit list 
            double formatSize = (double) byteSize; //cast to double for decimal places
            int temp = 0; //index denoting current unit

            while (formatSize >= 1024.0 && temp < unit.Length) { //loop while size can be converted >= 1.00 of the next unit
                formatSize /= 1024.0; //divide float value for next unit value
                temp++; //increment unit index
            }

            return formatSize.ToString("0.00") + " " + unit[temp]; //fix with 2 digits and return as string
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
            var allFiles = EnumerateFilesRecursively(args[0]); //iterate through files within folder, location stated in first argument
            XDocument report = CreateReport(allFiles);
            report.Save(args[1]); //save output as html file, location stated in second argument
            Console.WriteLine("Report Created!");
            //Console.WriteLine(String.Join(", ", allFiles.Select(n => n.ToString()).ToArray()));
        }
    }
}
