﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chaining
{
    class Chaining
    {
        private HashSet<string> Agenda { get; set; } = new HashSet<string>();
        private Dictionary<string, Clause> KnowledgeBase { get; set; } = new Dictionary<string, Clause>();

        public Chaining(string[] rawKnowledgeBase)
        {
            Parse(rawKnowledgeBase);
        }

        public bool Forward(string query)
        {
            Queue<string> agenda = new Queue<string>(Agenda);
            Dictionary<string, bool> inferences = KnowledgeBase.Keys.ToDictionary(x => x, x => false);

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
                        if (KnowledgeBase[key].Update(KnowledgeBase, p))
                            if (KnowledgeBase[key].State == true)
                                agenda.Enqueue(key);
                    }
                }
            }

            // Update the main agenda for later use
            Agenda.Concat(agenda);
            foreach (KeyValuePair<string, bool> kvp in inferences)
                    if (kvp.Value)
                        Agenda.Add(kvp.Key);

            // Return the result
            return result;
        }

        public bool Backward(string query)
        {
            if (!KnowledgeBase.ContainsKey(query))
                return false;

            bool? evaluation = KnowledgeBase[query].EvaluateDown(KnowledgeBase);

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
                        KnowledgeBase.Add(name, new Clause(true));
                    else
                        KnowledgeBase[name].State = true;
                    Agenda.Add(name);
                }
                else // This line contains a rule
                {
                    string[] splitLine = Regex.Split(line, ":-");
                    string name = splitLine[0];
                    string[] parents = Regex.Split(splitLine[1].Substring(0, splitLine[1].Length - 1), @",");
                    if (!KnowledgeBase.ContainsKey(name))
                        KnowledgeBase.Add(name, new Clause());
                    KnowledgeBase[name].Add(parents);
                }
            }
        }
    }
}
