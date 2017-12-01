using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc{
    public class count_linebreak{
        public static void Main(string[] args){
            
            if (args.GetLength(0)==1){
                Console.WriteLine(args[0]);
                var myFiles = Directory.GetFiles(args[0], "*.cs", SearchOption.AllDirectories);
                int line_count = 0;
                foreach (var thing in myFiles){
                    int c = File.ReadLines(thing.ToString()).Count();
                    Console.WriteLine(Path.GetFileNameWithoutExtension(thing.ToString()) + " : " + c);
                    line_count += c;
                }
                
                Console.WriteLine("Total line count:" + line_count);
            }
        }
    }
}