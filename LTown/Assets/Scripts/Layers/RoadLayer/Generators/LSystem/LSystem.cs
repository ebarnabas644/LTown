using System.Collections.Generic;
using System.Text;

namespace RoadLayer.Generators
{
    public class LSystem
    {
        private string axiom;
        private Dictionary<char, string> rules;

        public LSystem(string axiom)
        {
            this.axiom = axiom;
            this.rules = new Dictionary<char, string>();
        }

        public string GetAxiom => axiom;

        public void Iterate()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in this.axiom)
            {
                if (this.rules.ContainsKey(item))
                {
                    builder.Append(this.rules[item]);
                }
                else
                {
                    builder.Append(item);
                }
            }

            this.axiom = builder.ToString();
        }

        public void AddRule(char condition, string result)
        {
            rules.Add(condition, result);
        }

        public void RemoveRule(char condition)
        {
            rules.Remove(condition);
        }
    }

}