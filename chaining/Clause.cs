using System;
using System.Collections.Generic;

namespace chaining
{
    class Clause
    {
        private List<List<string>> Children { get; set; } = new List<List<string>>();
        private string Name { get; set; }
        public bool? State { get; private set; } = null;

        public Clause(string name)
        {
            Name = name;
            State = true; // No children, thus this is a fact.
        }

        public Clause(string name, List<string> children)
        {
            Name = name;
            Children.Add(children);
        }

        public void AddChildren(List<string> children)
        {
            Children.Add(children);
            if (children.Count == 0)
                // No children == fact, so this clause is automatically true.
                State = true;

            if (State == false)
                // If state was already false, it can't still be, because this new group of children might be true.
                State = null;
        }

        public bool? Evaluate(KnowledgeBase knowledgeBase, string filter = null)
        {
            if (State != null)
            // If the state is already known, just return it. No need to overcomplicate things.
                return State;

            bool allFalse = true; // Checks whether all children are false, to update the state if possible.

            foreach (List<string> children in Children)
            {
                if (filter != null && !children.Contains(filter))
                {
                    allFalse = false; // Can't draw any conclusions on the over-all state of this clause, because not all children are evaluated.
                    continue; // This list of children doesn't meet the filter, so we skip it.
                }

                bool allTrue = true; // Checks whether all children in this list are true.

                foreach (string child in children)
                {
                    bool? childState = knowledgeBase.Entails(child);
                    if (childState == null)
                    {
                        allTrue = false;
                        allFalse = false;
                        break; // We can't infer anything without knowing every child in this set for sure.
                    }
                    else if (childState == false)
                    {
                        allTrue = false;
                        break; // This set of children is false for sure.
                    }
                }

                if (allTrue)
                // Found a group of children that are all true, so this is also true.
                {
                    State = true; // For next time.
                    return true;
                }
            }

            if (allFalse)
            // All children are false, so this clause is also necessarily false.
            {
                State = false; // For next time.
                return false;
            }

            return null; // Unfortunatly nothing could be inferred for sure.
        }

        public override int GetHashCode() => (new Tuple<string, List<List<string>>>(Name, Children)).GetHashCode();
    }
}
