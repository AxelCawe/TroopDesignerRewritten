using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TroopDesignerRewritten.TroopDesigner
{
    internal struct TroopDesignerItem
    {
        public string Slot;
        public string Id;

        public TroopDesignerItem(string slot, string id)
        {
            this.Slot = slot;
            this.Id = id;
        }
    }
}
