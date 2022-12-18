using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TroopDesignerRewritten.TroopDesigner
{
    internal class TroopDesignerCharacter
    {
        public string Id = "default_id";
        public FormationClass DefaultGroup = FormationClass.Infantry;
        public int Level = 6;
        public string Name = "{=}default_name";
        public bool IsBasicTroop = false;
        public bool IsFemale = false;
        public bool IsHero = false;
        public bool UseUpgradeRequirement = false;
        public string UpgradeRequirement = "";
        public Occupation Occupation = Occupation.Soldier;
        public string Culture = "empire";
        public bool UseSkillTemplate = false;
        public string SkillTemplate = "";
        public int DefaultSkillValue = 20;
        public string FaceKeyTemplate = "fighter_empire";
        public string FirstUpgradeTarget = "";
        public string SecondUpgradeTarget = "";
        private List<TroopDesignerEquipmentSet> _equipmentSets = new List<TroopDesignerEquipmentSet>();
        private TroopDesignerItem _horseEquipment = new TroopDesignerItem("", "");
        private TroopDesignerItem _horseHarnessEquipment = new TroopDesignerItem("", "");

        public TroopDesignerCharacter(CharacterObject characterTemplate)
        {
            if (characterTemplate == null)
                return;
            this.Id = characterTemplate.StringId;
            this.DefaultGroup = characterTemplate.DefaultFormationClass;
            this.Name = "{=!}" + characterTemplate.Name.ToString();
            this.Level = characterTemplate.Level;
            this.IsBasicTroop = characterTemplate.IsBasicTroop;
            this.IsFemale = characterTemplate.IsFemale;
            this.IsHero = characterTemplate.IsHero;
            this.UseUpgradeRequirement = characterTemplate.UpgradeRequiresItemFromCategory != null;
            if (this.UseUpgradeRequirement)
                this.UpgradeRequirement = characterTemplate.UpgradeRequiresItemFromCategory.GetName().ToString();
            this.Occupation = characterTemplate.Occupation;
            this.Culture = characterTemplate.Culture.StringId;
            this.FaceKeyTemplate = characterTemplate.BodyPropertyRange.StringId;
            if (characterTemplate.UpgradeTargets.Length != 0)
            {
                this.FirstUpgradeTarget = characterTemplate.UpgradeTargets[0].StringId;
                if (characterTemplate.UpgradeTargets.Length > 1)
                    this.SecondUpgradeTarget = characterTemplate.UpgradeTargets[1].StringId;
            }
        }

        public void AddEquipmentSet(bool isCivillian)
        {
            Equipment battleEquipment = Hero.MainHero.BattleEquipment;
            TroopDesignerEquipmentSet designerEquipmentSet = new TroopDesignerEquipmentSet();
            designerEquipmentSet.IsCivillian = isCivillian;
            designerEquipmentSet.TryAddEquipment(this.GetItemAtSlot(battleEquipment, EquipmentIndex.WeaponItemBeginSlot));
            designerEquipmentSet.TryAddEquipment(this.GetItemAtSlot(battleEquipment, EquipmentIndex.Weapon1));
            designerEquipmentSet.TryAddEquipment(this.GetItemAtSlot(battleEquipment, EquipmentIndex.Weapon2));
            designerEquipmentSet.TryAddEquipment(this.GetItemAtSlot(battleEquipment, EquipmentIndex.Weapon3));
            designerEquipmentSet.TryAddEquipment(this.GetItemAtSlot(battleEquipment, EquipmentIndex.NumAllWeaponSlots));
            designerEquipmentSet.TryAddEquipment(this.GetItemAtSlot(battleEquipment, EquipmentIndex.Cape));
            designerEquipmentSet.TryAddEquipment(this.GetItemAtSlot(battleEquipment, EquipmentIndex.Body));
            designerEquipmentSet.TryAddEquipment(this.GetItemAtSlot(battleEquipment, EquipmentIndex.Gloves));
            designerEquipmentSet.TryAddEquipment(this.GetItemAtSlot(battleEquipment, EquipmentIndex.Leg));
            designerEquipmentSet.TryAddEquipment(this.GetItemAtSlot(battleEquipment, EquipmentIndex.Horse));
            designerEquipmentSet.TryAddEquipment(this.GetItemAtSlot(battleEquipment, EquipmentIndex.HorseHarness));
            if (designerEquipmentSet.Slots.Count > 0)
            {
                this._equipmentSets.Add(designerEquipmentSet);
                InformationManager.DisplayMessage(new InformationMessage("Troop Designer - Equipment set added (" + this._equipmentSets.Count.ToString() + ")"));
            }
            else
                InformationManager.DisplayMessage(new InformationMessage("Troop Designer - Equipment set is empty"));
        }

        public void AddHorseEquipment()
        {
            Equipment battleEquipment = Hero.MainHero.BattleEquipment;
            this._horseEquipment = this.GetItemAtSlot(battleEquipment, EquipmentIndex.ArmorItemEndSlot);
            this._horseHarnessEquipment = this.GetItemAtSlot(battleEquipment, EquipmentIndex.HorseHarness);
            InformationManager.DisplayMessage(new InformationMessage("Troop Designer - Horse Equipement set (Horse: " + this._horseEquipment.Id + ", Harness: " + this._horseHarnessEquipment.Id + ")"));
        }

        private TroopDesignerItem GetItemAtSlot(
          Equipment equipment,
          EquipmentIndex equipmentIndex)
        {
            string slot = "";
            string id = "";
            EquipmentElement equipmentFromSlot = equipment.GetEquipmentFromSlot(equipmentIndex);
            if (equipmentFromSlot.Item != null)
            {
                string str;
                switch (equipmentIndex)
                {
                    case EquipmentIndex.WeaponItemBeginSlot:
                        str = "Item0";
                        break;
                    case EquipmentIndex.Weapon1:
                        str = "Item1";
                        break;
                    case EquipmentIndex.Weapon2:
                        str = "Item2";
                        break;
                    case EquipmentIndex.Weapon3:
                        str = "Item3";
                        break;
                    case EquipmentIndex.ExtraWeaponSlot:
                        str = "Item4";
                        break;
                    case EquipmentIndex.NumAllWeaponSlots:
                        str = "Head";
                        break;
                    case EquipmentIndex.ArmorItemEndSlot:
                        str = "Horse";
                        break;
                    default:
                        str = equipmentIndex.ToString();
                        break;
                }
                slot = str;
                id = equipmentFromSlot.Item.StringId;
            }
            return new TroopDesignerItem(slot, id);
        }

        public List<string> ToXml()
        {
            List<string> xml = new List<string>();
            xml.Add("\t<NPCCharacter");
            xml.Add("\t\tid=\"" + this.Id + "\"");
            xml.Add("\t\tdefault_group=\"" + (this.DefaultGroup == FormationClass.NumberOfDefaultFormations ? "Skirmisher" : this.DefaultGroup.ToString()) + "\"");
            xml.Add("\t\tlevel=\"" + this.Level.ToString() + "\"");
            xml.Add("\t\tname=\"" + this.Name + "\"");
            if (this.IsBasicTroop)
                xml.Add("\t\tis_basic_troop=\"true\"");
            if (this.IsFemale)
                xml.Add("\t\tis_female=\"true\"");
            if (this.IsHero)
                xml.Add("\t\tis_hero=\"true\"");
            if (this.UseUpgradeRequirement && this.UpgradeRequirement != "")
                xml.Add("\t\tupgrade_requires=\"ItemCategory." + this.UpgradeRequirement + "\"");
            xml.Add("\t\toccupation=\"" + this.Occupation.ToString() + "\"");
            xml.Add("\t\tculture=\"Culture." + this.Culture + "\"" + (this.SkillTemplate == "" ? " >" : ""));
            if (this.UseSkillTemplate)
                xml.Add("\t\tskill_template=\"SkillSet." + this.SkillTemplate + "\" >");
            xml.Add("\t\t<face>");
            xml.Add("\t\t\t<face_key_template value=\"BodyProperty." + this.FaceKeyTemplate + "\" />");
            xml.Add("\t\t</face>");
            if (!this.UseSkillTemplate)
            {
                xml.Add("\t\t<skills>");
                xml.Add("\t\t\t<skill id=\"Athletics\" value=\"" + this.DefaultSkillValue.ToString() + "\" />");
                xml.Add("\t\t\t<skill id=\"Riding\" value=\"" + this.DefaultSkillValue.ToString() + "\" />");
                xml.Add("\t\t\t<skill id=\"OneHanded\" value=\"" + this.DefaultSkillValue.ToString() + "\" />");
                xml.Add("\t\t\t<skill id=\"TwoHanded\" value=\"" + this.DefaultSkillValue.ToString() + "\" />");
                xml.Add("\t\t\t<skill id=\"Polearm\" value=\"" + this.DefaultSkillValue.ToString() + "\" />");
                xml.Add("\t\t\t<skill id=\"Bow\" value=\"" + this.DefaultSkillValue.ToString() + "\" />");
                xml.Add("\t\t\t<skill id=\"Crossbow\" value=\"" + this.DefaultSkillValue.ToString() + "\" />");
                xml.Add("\t\t\t<skill id=\"Throwing\" value=\"" + this.DefaultSkillValue.ToString() + "\" />");
                xml.Add("\t\t</skills>");
            }
            xml.Add("\t\t<upgrade_targets>");
            if (this.FirstUpgradeTarget != "")
                xml.Add("\t\t\t<upgrade_target id=\"NPCCharacter." + this.FirstUpgradeTarget + "\" />");
            if (this.SecondUpgradeTarget != "")
                xml.Add("\t\t\t<upgrade_target id=\"NPCCharacter." + this.SecondUpgradeTarget + "\" />");
            xml.Add("\t\t</upgrade_targets>");
            xml.Add("\t\t<Equipments>");
            foreach (TroopDesignerEquipmentSet equipmentSet in this._equipmentSets)
            {
                if (equipmentSet.IsCivillian)
                    xml.Add("\t\t\t<EquipmentRoster civilian=\"true\">");
                else
                    xml.Add("\t\t\t<EquipmentRoster>");
                foreach (TroopDesignerItem slot in equipmentSet.Slots)
                    xml.Add("\t\t\t\t<equipment slot=\"" + slot.Slot + "\" id=\"Item." + slot.Id + "\" />");
                xml.Add("\t\t\t</EquipmentRoster>");
            }
            //if (this._horseEquipment.Id != "")
            //{
            //    xml.Add("\t\t\t<equipment slot=\"" + this._horseEquipment.Slot + "\" id=\"Item." + this._horseEquipment.Id + "\" />");
            //    if (this._horseHarnessEquipment.Id != "")
            //        xml.Add("\t\t\t<equipment slot=\"" + this._horseHarnessEquipment.Slot + "\" id=\"Item." + this._horseHarnessEquipment.Id + "\" />");
            //}
            xml.Add("\t\t</Equipments>");
            xml.Add("\t</NPCCharacter>");
            return xml;
        }
    }
}
