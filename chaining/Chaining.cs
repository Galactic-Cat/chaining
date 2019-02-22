using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chaining
{
    class Chaining
    {
        private Dictionary<string, Clause> KnowledgeBase { get; set; } = new Dictionary<string, Clause>();

        public Chaining(string[] rawKnowledgeBase)
        {
            Parse(rawKnowledgeBase);
        }

        public bool Forward(string query)
        {
            // Create an agenda with only an empty string (functioning as verum)
            Queue<string> agenda = new Queue<string>(new string[] { "" });
            // Create a inference dictionary including the empty string (functioning as verum)
            Dictionary<string, bool> inferences = KnowledgeBase.Keys.Concat(agenda).ToDictionary(x => x, x => false);

            bool result = false;
            while (agenda.Count > 0)
            {
                string p = agenda.Dequeue();
                if (p == query)
                {
                    result = true;
                    break;
                }
                if (inferences[p] == false)
                {
                    inferences[p] = true;
                    foreach (string key in KnowledgeBase.Keys)
                    {
                        if (KnowledgeBase[key].EvaluateShallow(KnowledgeBase, p))
                            if (KnowledgeBase[key].State == true)
                                agenda.Enqueue(key);
                    }
                }
            }

            // Return the result
            return result;
        }

        public bool Backward(string query)
        {
            if (!KnowledgeBase.ContainsKey(query))
                return false;

            bool? evaluation = KnowledgeBase[query].EvaluateDeep(KnowledgeBase);

            if (evaluation == null)
                return false;

            return (bool)evaluation;
        }

        private void Parse(string[] raw)
        {
            foreach (string line in raw)
            {
                if (Regex.IsMatch(line, @"^\w+\.$")) // This line contains a fact
                {
                    string name = Regex.Match(line, @"\w+").ToString();
                    if (!KnowledgeBase.ContainsKey(name))
                        KnowledgeBase.Add(name, new Clause(new string[0]));
                    else
                        KnowledgeBase[name].Add(new string[0]);
                }
                else // This line contains a rule
                {
                    string[] splitLine = Regex.Split(line, ":-"); // Split name and antecedents
                    string name = splitLine[0];
                    string[] antecedents = Regex.Split(splitLine[1].Substring(0, splitLine[1].Length - 1), @",");
                    if (!KnowledgeBase.ContainsKey(name))
                        KnowledgeBase.Add(name, new Clause(antecedents));
                    else
                        KnowledgeBase[name].Add(antecedents);
                }
            }
        }
    }
}
