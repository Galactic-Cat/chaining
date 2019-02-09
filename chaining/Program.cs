using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chaining
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Clause> knowledgeBase = new Dictionary<string, Clause>();
            string[] firstLine = Regex.Match(Console.ReadLine(), @"[0-9]+ [0-9]+ [A-Za-z]").ToString().Split(' ');
            int clauseCount = int.Parse(firstLine[0]);
            int queryCount = int.Parse(firstLine[1]);
            bool forward = (firstLine[2] == "f");

            string[] rawKnowledgeBase = new string[clauseCount];
            for (int i = 0; i < clauseCount; i++)
            {
                string line = Regex.Replace(Console.ReadLine().Split('%')[0], @"\s", ""); // Read and cleanup line
                if (!line.Contains(".")) do
                    {
                        line += Regex.Replace(Console.ReadLine().Split('%')[0], @"\s", ""); // Extend line until a '.' is found
                    } while (!line.Contains("."));
                rawKnowledgeBase[i] = line;
            }

            Chaining chaining = new Chaining(rawKnowledgeBase);

            for (int i = 0; i < queryCount; i++)
            {
                string query = Regex.Match(Console.ReadLine(), @"\w+").ToString();
                string result = "";
                if (forward)
                    result = chaining.Forward(query).ToString().ToLower();
                else
                    result = chaining.Backward(query).ToString().ToLower();
                Console.WriteLine(query + ". " + result + ".");
            }

            //Console.ReadLine();
        }
    }
}
