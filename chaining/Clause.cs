using System.Collections.Generic;

namespace chaining
{
    class Clause
    {
        private List<string[]> Evaluents = new List<string[]>();
        public bool? State { get; private set; } = false;

        public Clause(bool? state = null)
        {
            State = state;
        }

        public void Add(string[] evaluents)
        {
            Evaluents.Add(evaluents);
        }

        public bool? Evaluate(Dictionary<string, Clause> knowledgeBase)
        {
            if (State != null)
                return State;

            foreach (string[] evaluent in Evaluents)
            {
                foreach (string clause in evaluent)
                {
                    if (knowledgeBase[clause] == null)
                    {
                        State = false;
                        return false;
                    }
                    bool? state = knowledgeBase[clause].State;
                    if (state != null)
                    {
                        State = state;
                        return State;
                    }
                }
            }

            return null;
        }
    }
}
