using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JHEvaluation.Rank.Model
{
    class ItemTypeMapping
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }


        public ItemTypeMapping(string name ,string displayName)
        {
            this.Name = name;
            this.DisplayName = displayName;
        }
    }
}
