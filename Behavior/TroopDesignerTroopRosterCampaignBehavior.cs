using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.InputSystem;
using TroopDesignerRewritten.TroopDesignerRoster;
using System.Reflection;

namespace TroopDesignerRewritten.Behavior
{
    internal class TroopDesignerTroopRosterCampaignBehavior : CampaignBehaviorBase
    {
      
        public void TickCampaignBehavior()
        {
            if (Mission.Current != null || !Input.IsKeyDown(InputKey.LeftControl) || !Input.IsKeyReleased(InputKey.P))
                return;
            this.OpenCustomTroopRoster();
        }

        private void OpenCustomTroopRoster()
        {
            TroopDesignerRosterSettings settings = this.GetSettings();
            TroopRoster troopRoster = new TroopRoster(PartyBase.MainParty);
            foreach (TroopDesignerRosterEntry character in settings.Characters)
                this.TryAddCharacterToRoster(troopRoster, character.Id, 500);
            // ISSUE: method pointer
            // ISSUE: method pointer
            // ISSUE: method pointer
            PartyScreenManager.OpenScreenAsQuest(troopRoster, new TextObject("{=partyScreen_PartyHeader}Custom Troops"), 0, 0, new PartyPresentationDoneButtonConditionDelegate(PartyScreenDoneCondition), new PartyScreenClosedDelegate(PartyScreenClosed), new IsTroopTransferableDelegate(TroopTransferableDelegate), null);
        }

        private void TryAddCharacterToRoster(TroopRoster troopRoster, string characterId, int count)
        {
            CharacterObject characterObject = CharacterObject.Find(characterId);
            if (characterObject != null)
                troopRoster.AddToCounts(characterObject, count, false, 0, 0, true, -1);
            else
                InformationManager.DisplayMessage(new InformationMessage("Troop Designer - " + characterId + " id not found."));
        }

        private TroopDesignerRosterSettings GetSettings()
        {
            try
            {
                string path = SubModule.GetPath("custom_troop_roster.xml");
                using (StreamReader streamReader = new StreamReader(path))
                    return (TroopDesignerRosterSettings)new XmlSerializer(typeof(TroopDesignerRosterSettings)).Deserialize((TextReader)streamReader);
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage("Troop Designer - Error occured while trying to retrieve custom troop roster file"));
                return new TroopDesignerRosterSettings();
            }
        }

        private bool TroopTransferableDelegate(
          CharacterObject character,
          PartyScreenLogic.TroopType type,
          PartyScreenLogic.PartyRosterSide side,
          PartyBase LeftOwnerParty)
        {
            return true;
        }

        private Tuple<bool, TextObject> PartyScreenDoneCondition(
          TroopRoster leftMemberRoster,
          TroopRoster leftPrisonRoster,
          TroopRoster rightMemberRoster,
          TroopRoster rightPrisonRoster,
          int leftLimitNum,
          int rightLimitNum)
        {
            return new Tuple<bool, TextObject>(true, new TextObject());
        }

        private void PartyScreenClosed(
          PartyBase leftOwnerParty,
          TroopRoster leftMemberRoster,
          TroopRoster leftPrisonRoster,
          PartyBase rightOwnerParty,
          TroopRoster rightMemberRoster,
          TroopRoster rightPrisonRoster,
          bool fromCancel)
        {
        }

        public override void RegisterEvents()
        {
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}
