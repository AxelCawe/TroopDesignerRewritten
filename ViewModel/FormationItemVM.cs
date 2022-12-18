using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Localization;

namespace TroopDesignerRewritten.ViewModel
{
    public class FormationItemVM : SelectorItemVM
    {
        public FormationClass FormationClass { get; private set; }

        public FormationItemVM(FormationClass formationClass)
          : base("")
        {
            this.FormationClass = formationClass;
            this.StringItem = this.FormationClass == FormationClass.NumberOfDefaultFormations ? "Skirmisher" : this.FormationClass.ToString();
            this.Hint = new HintViewModel(new TextObject(), (string)null);
            this.CanBeSelected = true;
        }
    }
}
