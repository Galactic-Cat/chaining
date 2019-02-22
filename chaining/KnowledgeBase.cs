using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace chaining
{
    class KnowledgeBase
    {
        private Dictionary<string, Clause> Clauses { get; set; } = new Dictionary<string, Clause>();
        public bool ForwardChaining { get; set; } = true;

        public KnowledgeBase(string[] raw, bool forwardChaining = true)
        {
            ForwardChaining = forwardChaining; // Store whether or not to use forward chaining, rather than backward chaining.
            Parse(raw); // Parse the raw knowledgebase into the clauses hashset.
        }

        public bool? Entails(string name) => Entails(name, ForwardChaining);

        public bool? Entails(string name, bool forward)
        {
            if (!Clauses.ContainsKey(name))
                // If the clause is missing from the knowledgebase, it's nessecarily false.
                return false;

            if (!forward)
            {

            }
        }

        private void Parse(string[] raw)
        {
            foreach (string line in raw)
            {
                if (Regex.IsMatch(line, @"^\w+\.$"))
                // This line is a fact (and has no 'child'-clauses)
                {
                    string name = Regex.Match(line, @"^\w+").ToString();
                    if (Clauses.ContainsKey(name))
                        Clauses[name].AddChildren(new List<string>(0)); // Add an empty list (no children == fact)
                    else
                        Clauses[name] = new Clause(name);
                }
            }
        }
    }
}
