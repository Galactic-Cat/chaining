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
            string[] facts = ParseKnowledgeBase(knowledgeBase, rawKnowledgeBase); // Parses knowledgebase and returns all facts for forward chaining.

            if (forward)
                for (int i = 0; i < queryCount; i++)
                {
                    string query = Regex.Match(Console.ReadLine(), @"\w+").ToString();
                    Queue<string> agenda = new Queue<string>(facts);
                    while (agenda.Count > 0)
                    {
                        string p = agenda.Dequeue();
                        if (p == query)
                        {
                            Console.WriteLine(query + ". true.");
                            break;
                        }

                        if (knowledgeBase[p].Evaluate())
                    }
                }
        }

        static string[] ParseKnowledgeBase(Dictionary<string, Clause> knowledgeBase, string[] rawKnowledgeBase)
        {
            List<string> facts = new List<string>();
            foreach (string line in rawKnowledgeBase)
            {
                if (Regex.IsMatch(line, @"^\w+\.$")) // This line contains a fact
                {
                    string name = Regex.Match(line, @"\w+").ToString();
                    if (knowledgeBase[name] == null)
                        knowledgeBase.Add(name, new Clause(new IProlog[1] { new Fact() }));
                    else
                        knowledgeBase[name].Add(new Fact());
                    facts.Add(name);
                    continue;
                }
                else // This line contains a rule
                {
                    string[] splitLine = Regex.Split(line, ":-");
                    string name = splitLine[0];
                    string[] parents = Regex.Split(splitLine[1], @"[\.,]");
                    if (knowledgeBase[name] == null)
                        knowledgeBase.Add(name, new Clause(new IProlog[1] { new Rule(parents) }));
                    else
                        knowledgeBase[name].Add(new Rule(parents));
                }
            }
            return facts.ToArray();
        }

        static string[] FindChildren(string[] names, Dictionary<string, IProlog[]> knowledgeBase)
        {
            List<string> children = new List<string>();
            foreach (string key in knowledgeBase.Keys)
            {
                if (!names.Contains(key))
                    foreach (IProlog prolog in knowledgeBase[key])
                    {
                        foreach (string parent in prolog.Parents)
                            if (names.Contains(parent))
                            {
                                children.Add(key);
                                break;
                            }
                        if (children.Contains(key))
                            break;
                    }
            }
            return children.ToArray();
        }
    }
}
