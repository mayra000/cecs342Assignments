using System;
using System.Collections.Generic; //IEnumerable
using System.IO; //Directory
using System.Xml.Linq; //XDocument & XElement
using System.Linq;

namespace CECS342_Assignment3
{
    class Program
    {
        // recursively enumerate all files in the given folder (including the enrire sub-folder hierarchy)
        static IEnumerable<string> EnumerateFilesRecursively(string path) {
            // return IEnumerable<string> 
            IEnumerable<string> allFiles = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories); 
            
            // iterate through the files
            foreach (string file in allFiles) { 
                // process one file at a time
                yield return file; 
            }
        }

        // format a byte size in human readble form
        static string FormatByteSize(long byteSize) {
            // a string array of size units
            string[] unit = {"B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB"};
            // cast to double for decimal places
            double formatSize = (double) byteSize; 
            // index denoting current unit
            int temp = 0; 

            // loop while size can be converted >= 1.00 of the next unit
            while (formatSize >= 1024.0 && temp < unit.Length) {
                // divide float value for next unit value
                formatSize /= 1024.0; 
                // increment unit index
                temp++; 
            }
            
            // return the size as a string with 2 digits along with its unit
            return formatSize.ToString("0.00") + " " + unit[temp]; 
        }

        // create a HTML report with type, count and size for each file type
        static XDocument CreateReport(IEnumerable<string> files) {
            // GroupBy file extenstion out IEnumerable object with type, count, and size field sort by size descending
            var fileType = files.GroupBy(f => Path.GetExtension(f)).Select(
                g => new { type = g.Key, count = g.Count(), 
                    size = FormatByteSize(g.Sum(x => new FileInfo(x).Length)) })
                    .OrderByDescending(o => o.size);

            // functional way of generating xDocument
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
            
            // iterate through files within folder, location stated in first argument
            var allFiles = EnumerateFilesRecursively(args[0]); 
            
            // create a report for all files within the folder
            XDocument report = CreateReport(allFiles);
            
            // save output as html report file, location stated in second argument
            report.Save(args[1]); 
            
            Console.WriteLine("Report Created!");
        }
    }
}
