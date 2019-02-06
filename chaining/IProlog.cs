using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chaining
{
    interface IProlog
    {
        string[] Parents { get; }

        bool? Evaluate(Dictionary<string, Clause> knowledgeBase, bool shallow);
    }

    class Fact : IProlog
    {
        public string[] Parents { get; private set; } = new string[0];
        private bool State { get; set; } = true;

        public Fact() { }
        public Fact(bool state)
        {
            State = state;
        }
    
        public bool? Evaluate(Dictionary<string, Clause> knowledgeBase, bool shallow = true)
        {
            return State;
        }
    }

    class Rule : IProlog
    {
        private bool? State { get; set; } = null;
        public string[] Parents { get; private set; }

        public Rule(string[] parents)
        {
            Parents = parents;
        }

        public bool? Evaluate(Dictionary<string, Clause> knowledgeBase, bool shallow = false)
        {
            // Return the state if already found
            if (State != null || shallow)
                return State;

            // Evaluate the parents
            foreach (string parent in Parents)
            {
                if (knowledgeBase[parent] == null)
                {
                    knowledgeBase.Add(parent, new Clause(new IProlog[1] { new Fact(false) }));
                    State = false;
                    return false;
                }

                bool? evaluation = knowledgeBase[parent].Evaluate(knowledgeBase, true);
                if (evaluation == false)
                {
                    State = false;
                    return false;
                }
                else if (evaluation == null)
                    return null;
            }

            // All parents were true, so this rule is also true
            State = true;
            return true;
        }
    }

    class Clause : IProlog
    {
        private bool? State { get; set; } = null;
        public string[] Parents { get; private set; }
        private List<IProlog> SubClauses { get; set; }

        public Clause(IProlog[] subClauses)
        {
            List<string> parents = new List<string>();
            foreach (IProlog subClause in SubClauses)
                parents.Concat(subClause.Parents);
            Parents = parents.ToArray();
            SubClauses = new List<IProlog>(subClauses);
        }

        public void Add(IProlog subClause)
        {
            SubClauses.Add(subClause);
        }

        public bool? Evaluate(Dictionary<string, Clause> knowledgeBase, bool shallow = false)
        {
            if (State != null || shallow)
                return State;
            
            foreach (IProlog subClause in SubClauses)
            {
                bool? evaluation = subClause.Evaluate(knowledgeBase, false);
                if (evaluation == true)
                {
                    State = true;
                    return true;
                }
                else if (evaluation == false)
                {
                    State = false;
                    return false;
                }
            }

            return null;
        }
    }
}
