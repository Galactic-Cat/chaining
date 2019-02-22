using System;
using System.Collections.Generic;

namespace chaining
{
    class Clause
    {
        private List<List<string>> Antecedents = new List<List<string>>();
        public bool? State { get; private set; } = null;
        public bool Evaluating { get; private set; } = false;

        public Clause(string[] firstAntecedents)
        {
            Antecedents.Add(new List<string>(firstAntecedents));
        }

        public void Add(string[] newAntecedents)
        {
            Antecedents.Add(new List<string>(newAntecedents));
        }

        public bool EvaluateShallow(Dictionary<string, Clause> knowledgeBase, string updatedAntecedent = null)
        {
            // If state is already set, we don't need to go through all the hassle.
            if (State != null)
                return (bool)State;

            foreach (List<string> antecedents in Antecedents)
            {
                // Continue if this set of antecedents doesn't contain the updated antecedent (and make sure the updated antecedent isn't the empty string (functioning as verum)).
                if (updatedAntecedent != null && updatedAntecedent != "" && !antecedents.Contains(updatedAntecedent))
                    continue; 

                bool isTrue = true;
                foreach (string antecedent in antecedents)
                {
                    if (!knowledgeBase.ContainsKey(antecedent))
                    {
                        // Clause is missing from the knowledgebase, it has to be false.
                        isTrue = false;
                        continue;
                    }

                    bool? state = knowledgeBase[antecedent].State;
                    if (state == null)
                        // State can't be true anymore because not all antecedents evaluate.
                        isTrue = false;

                    if (state == false)
                    {
                        isTrue = false;
                        break;
                    }
                }
                if (isTrue) // All antecedents are true, so state should be true as well.
                {
                    State = true;
                    return true;
                }
            }

            // We can't evaluate this to true, so it's false.
            return false;
        }

        public bool? EvaluateDeep(Dictionary<string, Clause> knowledgeBase)
        {
            if (State != null)
                return (bool)State;

            Evaluating = true;

            foreach (List<string> antencedents in Antecedents)
            {
                bool allTrue = true;
                foreach (string antencedent in antencedents)
                {
                    if (!knowledgeBase.ContainsKey(antencedent))
                    {
                        // This value doesn't exist, so it's necessarily false.
                        State = false;
                        Evaluating = false;
                        return false;
                    }

                    if (knowledgeBase[antencedent].Evaluating)
                        // This evaluation is looping, gtfo!
                        break;

                    bool? evaluation = knowledgeBase[antencedent].EvaluateDeep(knowledgeBase);

                    if (evaluation == false)
                    {
                        // This antencedent is false, so this clause is also false.
                        allTrue = false;
                        break;
                    }
                    else if (evaluation == null)
                    {
                        // This antencedent is unsure of it's state, so this evaluation is useless.
                        allTrue = false;
                        break;
                    }
                }
                if (allTrue)
                {
                    // All antencedents are true, so this clause must be true as well.
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
