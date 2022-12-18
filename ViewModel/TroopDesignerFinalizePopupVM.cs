using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TroopDesignerRewritten.TroopDesigner;
using TaleWorlds.ObjectSystem;
using TaleWorlds.Localization;

namespace TroopDesignerRewritten.ViewModel
{
    internal class TroopDesignerFinalizePopupVM : TaleWorlds.Library.ViewModel
    {
        public Action<bool> OnGenderChange;

        private Action _onFinalizeCharacter;
        private TroopDesignerCharacter _characterRef;
        private bool _isOpen;
        private SelectorVM<FormationItemVM> _formationSelection;
        private SelectorVM<OccupationItemVM> _occupationSelection;

        public TroopDesignerFinalizePopupVM(
          Action onFinalizeCharacter,
          TroopDesignerCharacter characterRef)
        {
            this._onFinalizeCharacter = onFinalizeCharacter;
            this._characterRef = characterRef;
            this.FormationSelection = new SelectorVM<FormationItemVM>(0, new Action<SelectorVM<FormationItemVM>>(this.OnChangeFormation));
            this.FormationSelection.AddItem(new FormationItemVM(FormationClass.Infantry));
            this.FormationSelection.AddItem(new FormationItemVM(FormationClass.Ranged));
            this.FormationSelection.AddItem(new FormationItemVM(FormationClass.Cavalry));
            this.FormationSelection.AddItem(new FormationItemVM(FormationClass.HorseArcher));
            this.FormationSelection.AddItem(new FormationItemVM(FormationClass.NumberOfDefaultFormations));
            this.FormationSelection.AddItem(new FormationItemVM(FormationClass.HeavyInfantry));
            this.FormationSelection.AddItem(new FormationItemVM(FormationClass.LightCavalry));
            this.FormationSelection.AddItem(new FormationItemVM(FormationClass.HeavyCavalry));
            //this.FormationSelection.SelectedIndex = (int)this._characterRef.DefaultGroup;
            this.SetDefaultFormationSelection();
            this.OccupationSelection = new SelectorVM<OccupationItemVM>(0, new Action<SelectorVM<OccupationItemVM>>(this.OnChangeOccupation));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.Bandit));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.BannerBearer));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.CaravanGuard));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.GangLeader));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.Gangster));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.Guard));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.Lord));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.Mercenary));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.PrisonGuard));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.Soldier));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.Townsfolk));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.Villager));
            this.OccupationSelection.AddItem(new OccupationItemVM(Occupation.Wanderer));
            this.SetDefaultOccupationSelection();

    
            
        }

        private void SetDefaultFormationSelection()
        {
            int num = 0;
            foreach (FormationItemVM formationItemVm in (Collection<FormationItemVM>)this.FormationSelection.ItemList)
            {
                if (formationItemVm.FormationClass != this._characterRef.DefaultGroup)
                    ++num;
                else
                    break;
            }
            this.FormationSelection.SelectedIndex = num;
        }

        private void SetDefaultOccupationSelection()
        {
            int num = 0;
            foreach (OccupationItemVM occupationItemVm in (Collection<OccupationItemVM>)this.OccupationSelection.ItemList)
            {
                if (occupationItemVm.Occupation != this._characterRef.Occupation)
                    ++num;
                else
                    break;
            }
            this.OccupationSelection.SelectedIndex = num;
        }

    


        public void Open() => this.IsOpen = true;

        public void Close() => this.IsOpen = false;

        public void SetCharacterRef(TroopDesignerCharacter characterRef)
        {
            this._characterRef = characterRef;
            this.OnPropertyChangedWithValue((object)this._characterRef.Id, "CharacterId");
            this.OnPropertyChangedWithValue((object)this._characterRef.Name, "CharacterName");
            this.FormationSelection.SelectedIndex = (int)this._characterRef.DefaultGroup;
            this.OnPropertyChangedWithValue((object)this._characterRef.Level.ToString(), "CharacterLevel");
            this.OnPropertyChangedWithValue((object)this._characterRef.IsBasicTroop, "CharacterIsBasicTroop");
            this.OnPropertyChangedWithValue((object)this._characterRef.IsFemale, "CharacterIsFemale");
            this.OnPropertyChangedWithValue((object)this._characterRef.IsHero, "CharacterIsHero");
            this.OnPropertyChangedWithValue((object)this._characterRef.UseUpgradeRequirement, "CharacterUseUpgradeRequirement");
            this.OnPropertyChangedWithValue((object)this._characterRef.UpgradeRequirement, "CharacterUpgradeRequirement");
            this.SetDefaultFormationSelection();
            this.SetDefaultOccupationSelection();
            //this.SetDefaultCultureSelection();
            this.OnPropertyChangedWithValue((object)this._characterRef.Culture, "CharacterCulture");
            this.OnPropertyChangedWithValue((object)this._characterRef.UseSkillTemplate, "CharacterUseSkillTemplate");
            this.OnPropertyChangedWithValue((object)!this._characterRef.UseSkillTemplate, "CharacterUseSkillValue");
            //this.SetDefaultSkillTemplateSelection();
            this.OnPropertyChangedWithValue((object)this._characterRef.SkillTemplate, "CharacterSkillTemplate");
            this.OnPropertyChangedWithValue((object)this._characterRef.DefaultSkillValue.ToString(), "CharacterDefaultSkillValue");
            //this.SetDefaultFaceTemplateSelection();
            this.OnPropertyChangedWithValue((object)this._characterRef.FaceKeyTemplate, "CharacterFaceTemplate");
            this.OnPropertyChangedWithValue((object)this._characterRef.FirstUpgradeTarget, "CharacterFirstUpgradeTarget");
            this.OnPropertyChangedWithValue((object)this._characterRef.SecondUpgradeTarget, "CharacterSecondUpgradeTarget");
        }

        private void ExecuteOutputCharacter()
        {
            this.Close();
            this._onFinalizeCharacter();
        }

        private void ExecuteClose() => this.Close();

        private void OnChangeFormation(SelectorVM<FormationItemVM> formationSelection)
        {
            if (this._characterRef == null)
                return;
            this._characterRef.DefaultGroup = formationSelection.SelectedItem.FormationClass;
        }

        private void OnChangeOccupation(SelectorVM<OccupationItemVM> occupationSelection)
        {
            if (this._characterRef == null || occupationSelection.SelectedItem==null)
                return;
            this._characterRef.Occupation = occupationSelection.SelectedItem.Occupation;
        }

      
        [DataSourceProperty]
        public bool IsOpen
        {
            get => this._isOpen;
            set
            {
                if (value == this._isOpen)
                    return;
                this._isOpen = value;
                this.OnPropertyChangedWithValue((object)value, nameof(IsOpen));
            }
        }

        [DataSourceProperty]
        public string CharacterId
        {
            get => this._characterRef != null ? this._characterRef.Id : "";
            set
            {
                if (this._characterRef == null || !(value != this._characterRef.Id))
                    return;
                this._characterRef.Id = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterId));
            }
        }

        [DataSourceProperty]
        public string CharacterName
        {
            get => this._characterRef != null ? this._characterRef.Name : "";
            set
            {
                if (this._characterRef == null || !(value != this._characterRef.Name))
                    return;
                this._characterRef.Name = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterName));
            }
        }

        [DataSourceProperty]
        public SelectorVM<FormationItemVM> FormationSelection
        {
            get => this._formationSelection;
            set
            {
                if (value == this._formationSelection)
                    return;
                this._formationSelection = value;
                this.OnPropertyChangedWithValue((object)value, nameof(FormationSelection));
            }
        }

        [DataSourceProperty]
        public string CharacterLevel
        {
            get => this._characterRef != null ? this._characterRef.Level.ToString() : "0";
            set
            {
                int result;
                if (this._characterRef == null || !int.TryParse(value, out result) || result == this._characterRef.Level)
                    return;
                this._characterRef.Level = result;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterLevel));
            }
        }

        [DataSourceProperty]
        public bool CharacterIsBasicTroop
        {
            get => this._characterRef != null && this._characterRef.IsBasicTroop;
            set
            {
                if (this._characterRef == null || value == this._characterRef.IsBasicTroop)
                    return;
                this._characterRef.IsBasicTroop = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterIsBasicTroop));
            }
        }

        [DataSourceProperty]
        public bool CharacterIsFemale
        {
            get => this._characterRef != null && this._characterRef.IsFemale;
            set
            {
                if (this._characterRef == null || value == this._characterRef.IsFemale)
                    return;
                this._characterRef.IsFemale = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterIsFemale));
                OnGenderChange.Invoke(value);
            }
        }

        [DataSourceProperty]
        public bool CharacterIsHero
        {
            get => this._characterRef != null && this._characterRef.IsHero;
            set
            {
                if (this._characterRef == null || value == this._characterRef.IsHero)
                    return;
                this._characterRef.IsHero = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterIsHero));
            }
        }

        [DataSourceProperty]
        public bool CharacterUseUpgradeRequirement
        {
            get => this._characterRef != null && this._characterRef.UseUpgradeRequirement;
            set
            {
                if (this._characterRef == null || value == this._characterRef.UseUpgradeRequirement)
                    return;
                this._characterRef.UseUpgradeRequirement = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterUseUpgradeRequirement));
                this.OnPropertyChangedWithValue((object)this._characterRef.UpgradeRequirement, "CharacterUpgradeRequirement");
            }
        }

        [DataSourceProperty]
        public string CharacterUpgradeRequirement
        {
            get => this._characterRef != null ? this._characterRef.UpgradeRequirement : "";
            set
            {
                if (this._characterRef == null || !(value != this._characterRef.UpgradeRequirement))
                    return;
                this._characterRef.UpgradeRequirement = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterUpgradeRequirement));
            }
        }

        [DataSourceProperty]
        public SelectorVM<OccupationItemVM> OccupationSelection
        {
            get => this._occupationSelection;
            set
            {
                if (value == this._occupationSelection)
                    return;
                this._occupationSelection = value;
                this.OnPropertyChangedWithValue((object)value, nameof(OccupationSelection));
            }
        }

      
        [DataSourceProperty]
        public bool CharacterUseSkillTemplate
        {
            get => this._characterRef != null && this._characterRef.UseSkillTemplate;
            set
            {
                if (this._characterRef == null || value == this._characterRef.UseSkillTemplate)
                    return;
                this._characterRef.UseSkillTemplate = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterUseSkillTemplate));
                this.OnPropertyChangedWithValue((object)!value, "CharacterUseSkillValue");
                this.OnPropertyChangedWithValue((object)this._characterRef.SkillTemplate, "CharacterSkillTemplate");
                this.OnPropertyChangedWithValue((object)this._characterRef.DefaultSkillValue.ToString(), "CharacterDefaultSkillValue");
            }
        }
        [DataSourceProperty]
        public string CharacterSkillTemplate
        {
            get => this._characterRef != null ? this._characterRef.SkillTemplate : "";
            set
            {
                if (this._characterRef == null || !(value != this._characterRef.SkillTemplate))
                    return;
                this._characterRef.SkillTemplate = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterSkillTemplate));
            }
        }

        [DataSourceProperty]
        public bool CharacterUseSkillValue => this._characterRef != null && !this._characterRef.UseSkillTemplate;

        [DataSourceProperty]
        public string CharacterDefaultSkillValue
        {
            get => this._characterRef != null ? this._characterRef.DefaultSkillValue.ToString() : "0";
            set
            {
                int result;
                if (this._characterRef == null || !int.TryParse(value, out result) || result == this._characterRef.DefaultSkillValue)
                    return;
                this._characterRef.DefaultSkillValue = result;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterDefaultSkillValue));
            }
        }
        [DataSourceProperty]
        public string CharacterCulture
        {
            get => this._characterRef != null ? this._characterRef.Culture : "";
            set
            {
                if (this._characterRef == null || !(value != this._characterRef.Culture))
                    return;
                this._characterRef.Culture = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterCulture));
            }
        }
        [DataSourceProperty]
        public string CharacterFaceTemplate
        {
            get => this._characterRef != null ? this._characterRef.FaceKeyTemplate : "";
            set
            {
                if (this._characterRef == null || !(value != this._characterRef.FaceKeyTemplate))
                    return;
                this._characterRef.FaceKeyTemplate = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterFaceTemplate));
            }
        }

        [DataSourceProperty]
        public string CharacterFirstUpgradeTarget
        {
            get => this._characterRef != null ? this._characterRef.FirstUpgradeTarget : "";
            set
            {
                if (this._characterRef == null || !(value != this._characterRef.FirstUpgradeTarget))
                    return;
                this._characterRef.FirstUpgradeTarget = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterFirstUpgradeTarget));
            }
        }

        [DataSourceProperty]
        public string CharacterSecondUpgradeTarget
        {
            get => this._characterRef != null ? this._characterRef.SecondUpgradeTarget : "";
            set
            {
                if (this._characterRef == null || !(value != this._characterRef.SecondUpgradeTarget))
                    return;
                this._characterRef.SecondUpgradeTarget = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterSecondUpgradeTarget));
            }
        }
    }
}
