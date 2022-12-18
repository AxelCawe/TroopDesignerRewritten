using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Localization;

namespace TroopDesignerRewritten.ViewModel
{
    public class OccupationItemVM : SelectorItemVM
    {
        public Occupation Occupation { get; private set; }

        public OccupationItemVM(Occupation occupation)
          : base("")
        {
            this.Occupation = occupation;
            this.StringItem = this.Occupation.ToString();
            this.Hint = new HintViewModel(new TextObject(), (string)null);
            this.CanBeSelected = true;
        }
    }
}
