using System;
using System.Collections.Generic;

namespace chaining
{
    class Clause
    {
        private List<List<string>> Evaluents = new List<List<string>>();
        public bool? State { get; set; } = null;
        public bool Evaluating { get; private set; } = false;

        public Clause(bool? state = null)
        {
            State = state;
        }

        public void Add(string[] evaluents)
        {
            Evaluents.Add(new List<string>(evaluents));
        }

        public bool Update(Dictionary<string, Clause> knowledgeBase, string updatedParent = null)
        {
            // If state is already set, there won't be any change.
            if (State != null)
                return (bool)State;

            foreach (List<string> parents in Evaluents)
            {
                // Continue if this set of parents doesn't contain the updated parent.
                if (updatedParent != null && !parents.Contains(updatedParent))
                    continue; 

                bool isTrue = true;
                foreach (string parent in parents)
                {
                    if (!knowledgeBase.ContainsKey(parent))
                    {
                        // Clause is missing from the knowledgebase, it has to be false.
                        isTrue = false;
                        continue;
                    }

                    bool? state = knowledgeBase[parent].State;
                    if (state == null)
                        // State can't be true anymore because not all parents evaluate.
                        isTrue = false;

                    if (state == false)
                    {
                        isTrue = false;
                        break;
                    }
                }
                if (isTrue) // All parents are true, so state should be true as well.
                {
                    State = true;
                    return true;
                }
            }

            // Appearently no change occured.
            return false;
        }

        public bool? EvaluateDown(Dictionary<string, Clause> knowledgeBase)
        {
            if (State != null)
                return (bool)State;

            Evaluating = true;

            foreach (List<string> children in Evaluents)
            {
                bool allTrue = true;
                foreach (string child in children)
                {
                    if (!knowledgeBase.ContainsKey(child))
                    {
                        // This value doesn't exist, so it's necessarily false.
                        State = false;
                        Evaluating = false;
                        return false;
                    }

                    if (knowledgeBase[child].Evaluating)
                        // This evaluation is looping, gtfo!
                        break;

                    bool? evaluation = knowledgeBase[child].EvaluateDown(knowledgeBase);

                    if (evaluation == false)
                    {
                        // This child is false, so this clause is also false.
                        allTrue = false;
                        break;
                    }
                    else if (evaluation == null)
                    {
                        // This child is unsure of it's state, so this evaluation is useless.
                        allTrue = false;
                        break;
                    }
                }
                if (allTrue)
                {
                    // All children are true, so this clause must be true as well.
                    Evaluating = false;
                    State = true;
                    return true;
                }
            }

            // Failed to assertain valuation, so return null
            Evaluating = false;
            return null;
        }
    }
}
