using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TroopDesignerRewritten.TroopDesigner;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.ObjectSystem;

namespace TroopDesignerRewritten.ViewModel
{
    internal class TroopDesignerVM : SPInventoryVM
    {
        private TroopDesignerCharacter _character;
        private TroopDesignerFinalizePopupVM _finalizePopup;
        private TroopDesignerLoadFromCharacterId _loadFromCharacterIdPopup;

        private InventoryLogic _inventoryLogic;

        private string defaultId = "troop_designer_character";



        public TroopDesignerVM(
          InventoryLogic inventoryLogic,
          bool isInCivilianModeByDefault,
          Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> getItemUsageSetFlags,
          string fiveStackShortcutkeyText,
          string entireStackShortcutkeyText)
          : base(inventoryLogic, isInCivilianModeByDefault, getItemUsageSetFlags, fiveStackShortcutkeyText, entireStackShortcutkeyText)
        {
            this._inventoryLogic = inventoryLogic;
            this._character = new TroopDesignerCharacter(CharacterObject.Find("troop_designer_character"));
            this.FinalizePopup = new TroopDesignerFinalizePopupVM(new Action(this.OnFinalizeCharacter), this._character);
            this._loadFromCharacterIdPopup = new TroopDesignerLoadFromCharacterId(new Action<string,string>(this.LoadFromExistingCharacter));
            this.FinalizePopup.OnGenderChange = new Action<bool>(ChangeModelGender);
        }

        #region UPDATE_CHARACTER_MODEL

        private void LoadCharacterModel(CharacterObject character)
        {
            Hero.MainHero.SetCharacterObject(character);
            List<SkillObject> allSkillObjects = MBObjectManager.Instance.GetObjectTypeList<SkillObject>().ToList();
            foreach (var skill in allSkillObjects)
            {
                Hero.MainHero.SetSkillValue(skill, 300);
            }
            MainCharacter.FillFrom(character);
            //HeroViewModel model = new HeroViewModel();
            //model.FillFrom(character);
            //MainCharacter = model;
            //MainCharacter.SetEquipment(character.FirstBattleEquipment);
        }
        private SPItemVM InitializeCharacterEquipmentSlot(ItemRosterElement itemRosterElement, EquipmentIndex equipmentIndex)
        {
            SPItemVM spitemVM;
            if (!itemRosterElement.IsEmpty)
            {
                spitemVM = new SPItemVM(this._inventoryLogic, this.MainCharacter.IsFemale, true, InventoryMode.Default, itemRosterElement, InventoryLogic.InventorySide.Equipment, "", "", this._inventoryLogic.GetCostOfItemRosterElement(itemRosterElement, InventoryLogic.InventorySide.Equipment), new EquipmentIndex?(equipmentIndex));
            }
            else
            {
                spitemVM = new SPItemVM();
                spitemVM.RefreshWith(null, InventoryLogic.InventorySide.Equipment);
            }
            return spitemVM;
        }

        private void UpdateCharacterEquipment(Equipment equipment)
        {
            this.CharacterHelmSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.Head], 1), EquipmentIndex.NumAllWeaponSlots);
            this.CharacterCloakSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.Cape], 1), EquipmentIndex.Cape);
            this.CharacterTorsoSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.Body], 1), EquipmentIndex.Body);
            this.CharacterGloveSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.Gloves], 1), EquipmentIndex.Gloves);
            this.CharacterBootSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.Leg], 1), EquipmentIndex.Leg);
            this.CharacterMountSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.Horse], 1), EquipmentIndex.ArmorItemEndSlot);
            this.CharacterMountArmorSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.HorseHarness], 1), EquipmentIndex.HorseHarness);
            this.CharacterWeapon1Slot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.Weapon0], 1), EquipmentIndex.WeaponItemBeginSlot);
            this.CharacterWeapon2Slot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.Weapon1], 1), EquipmentIndex.Weapon1);
            this.CharacterWeapon3Slot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.Weapon2], 1), EquipmentIndex.Weapon2);
            this.CharacterWeapon4Slot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.Weapon3], 1), EquipmentIndex.Weapon3);
            this.CharacterBannerSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(equipment[EquipmentIndex.ExtraWeaponSlot], 1), EquipmentIndex.ExtraWeaponSlot);
            this.MainCharacter.SetEquipment(equipment);
        }
        #endregion
        public void LoadFromExistingCharacter(string characterId, string characterEquipmentSetId)
        {
            CharacterObject characterObject = CharacterObject.Find(characterId);
            if (characterObject != null)
            {
                int characterEquipmentSet = -1;

                if (characterEquipmentSetId == "")
                    characterEquipmentSetId = "0";

                
                if (int.TryParse(characterEquipmentSetId, out characterEquipmentSet))
                {
                    if (characterEquipmentSet < characterObject.BattleEquipments.Count())
                    {
                        this.IsInWarSet = true;
                        InformationManager.DisplayMessage(new InformationMessage("Loading from character: " + characterObject.Name));
                        defaultId = characterId;
                        LoadCharacterModel(characterObject);
                        UpdateCharacterEquipment(characterObject.BattleEquipments.ElementAt(characterEquipmentSet));
                        ResetCharacter(false);


                    }
                    else
                        InformationManager.DisplayMessage(new InformationMessage("Cannot find character equipment set #" + characterId + "!", new Color(1f, 0f, 0f)));

                }
                else
                    InformationManager.DisplayMessage(new InformationMessage("Invalid integer: \"" + characterEquipmentSetId + "\"!", new Color(1f, 0f, 0f)));

            }
            else
            {
                InformationManager.DisplayMessage(new InformationMessage("Cannot find character of id: \"" + characterId + "\"!", new Color(1f, 0f, 0f)));
            }
        }

        public void ChangeModelGender(bool isFemale)
        {
            Hero.MainHero.UpdatePlayerGender(isFemale);
            MainCharacter.IsFemale = true;
        }

        public void OpenLoadFromCharacterId()
        {
            this.LoadFromCharacterIdPopup.Open();
        }

        public void ExecuteResetAndCompleteTranstactions()
        {
            if (Input.IsKeyPressed(InputKey.I))
                return;
            Game.Current.GameStateManager.PopState();
        }

        public void ExecuteBuyAllItems()
        {
            if (this._finalizePopup != null && this._finalizePopup.IsOpen)
                return;
            base.ExecuteBuyAllItems();
        }

        public void ExecuteSellAllItems()
        {
            if (this._finalizePopup != null && this._finalizePopup.IsOpen)
                return;
            base.ExecuteSellAllItems();
        }

        public void ExecuteCompleteTranstactions() => this.FinalizePopup.Open();

        public void ExecuteResetTranstactions()
        {
            base.ExecuteResetTranstactions();
            this.ResetCharacter();
        }

        public void ExecuteAddEquipment() => this._character.AddEquipmentSet(!this.IsInWarSet);


        private void ResetCharacter(bool showMessage = true)
        {
            this._character = new TroopDesignerCharacter(CharacterObject.Find(defaultId));
            this.FinalizePopup.SetCharacterRef(this._character);
            if (showMessage)
                InformationManager.DisplayMessage(new InformationMessage("Troop Designer - Character has been reset."));
        }

        private void OnFinalizeCharacter()
        {
            SubModule.TryOutputLines(this._character.ToXml(), "output_characters", "Troop Designer - NPCCharacter xml added", "Troop Designer - Failed to add NPCCharacter xml");
            this.ResetCharacter();
        }

        

        [DataSourceProperty]
        public TroopDesignerFinalizePopupVM FinalizePopup
        {
            get => this._finalizePopup;
            set
            {
                if (value == this._finalizePopup)
                    return;
                this._finalizePopup = value;
                OnPropertyChangedWithValue((object)value, nameof(FinalizePopup));
            }
        }

        [DataSourceProperty]
        public TroopDesignerLoadFromCharacterId LoadFromCharacterIdPopup
        {
            get => this._loadFromCharacterIdPopup;
            set
            {
                if (value == this._loadFromCharacterIdPopup)
                    return;
                this._loadFromCharacterIdPopup = value;
                OnPropertyChangedWithValue((object)value, nameof(_loadFromCharacterIdPopup));
            }
        }
    }
}
