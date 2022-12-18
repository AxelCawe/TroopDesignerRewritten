using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TroopDesignerRewritten.TroopDesigner
{
    internal class TroopDesignerEquipmentSet
    {
        public bool IsCivillian = false;

        public List<TroopDesignerItem> Slots = new List<TroopDesignerItem>();

        public void TryAddEquipment(TroopDesignerItem item)
        {
            if (!(item.Id != ""))
                return;
            this.Slots.Add(item);
        }
    }
}
