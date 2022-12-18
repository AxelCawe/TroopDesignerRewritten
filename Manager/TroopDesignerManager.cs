using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.GameState;
using TroopDesignerRewritten.Data;
using TroopDesignerRewritten.ViewModel;

namespace TroopDesignerRewritten.Manager
{
    internal class TroopDesignerManager : DesignerManager
    {
        private InventoryLogic _inventoryLogic;

        public override void OnLoadFinished()
        {
            base.OnLoadFinished();
            Hero.MainHero.SetCharacterObject(CharacterObject.Find("troop_designer_character"));
            MobileParty.MainParty.ItemRoster.Clear();
            this.OpenTroopDesignerInventory();
        }

        private void OpenTroopDesignerInventory()
        {
            ItemRoster itemRoster = new ItemRoster();
            MBReadOnlyList<ItemObject> objectTypeList = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>();
            for (int index = 0; index != objectTypeList.Count; ++index)
            {
                ItemObject itemObject = objectTypeList[index];
                if (!itemObject.IsTradeGood && !itemObject.IsAnimal)
                    itemRoster.AddToCounts(itemObject, 4);
            }
            this._inventoryLogic = new InventoryLogic(MobileParty.MainParty, CharacterObject.PlayerCharacter, (PartyBase)null);
            this._inventoryLogic.Initialize(itemRoster, MobileParty.MainParty, false, true, CharacterObject.PlayerCharacter, InventoryManager.InventoryCategoryType.All, (IMarketData)new TroopDesignerMarketData(), false, null);
            TroopDesignerState state = Game.Current.GameStateManager.CreateState<TroopDesignerState>();
            state.InitializeLogic(this._inventoryLogic);
            Game.Current.GameStateManager.PushState((GameState)state);
        }
    }
}

