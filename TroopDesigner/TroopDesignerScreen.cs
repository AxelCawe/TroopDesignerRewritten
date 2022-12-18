using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade;
using SandBox.View;
using TaleWorlds.GauntletUI.Data;
using TroopDesignerRewritten.ViewModel;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TroopDesignerRewritten.TroopDesigner
{
    [GameStateScreen(typeof(TroopDesignerState))]
    internal class TroopDesignerScreen :
    ScreenBase,
    IInventoryStateHandler,
    IGameStateListener,
    IChangeableScreen
    {
        private readonly TroopDesignerState _troopDesignerState;
        private IGauntletMovie _gauntletMovie;
        private TroopDesignerVM _dataSource;
        private GauntletLayer _gauntletLayer;
        private bool _closed;
        private bool _openedFromMission;
        private SpriteCategory _inventoryCategory;

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);
            if (!this._closed)
                LoadingWindow.DisableGlobalLoadingWindow();
            this._dataSource.IsFiveStackModifierActive = ((ScreenLayer)this._gauntletLayer).Input.IsHotKeyDown("FiveStackModifier");
            this._dataSource.IsEntireStackModifierActive = ((ScreenLayer)this._gauntletLayer).Input.IsHotKeyDown("EntireStackModifier");
            if (((ScreenLayer)this._gauntletLayer).Input.IsHotKeyReleased("SwitchAlternative") && this._dataSource != null)
                this._dataSource.CompareNextItem();
            else if (((ScreenLayer)this._gauntletLayer).Input.IsHotKeyReleased("Exit"))
                this.ExecuteCancel();
            else if (((ScreenLayer)this._gauntletLayer).Input.IsHotKeyReleased("Confirm"))
                this.ExecuteConfirm();
            else if (((ScreenLayer)this._gauntletLayer).Input.IsHotKeyPressed("SwitchToPreviousTab"))
                this.ExecuteSwitchToPreviousTab();
            else if (((ScreenLayer)this._gauntletLayer).Input.IsHotKeyPressed("SwitchToNextTab"))
                this.ExecuteSwitchToNextTab();
            else if (((ScreenLayer)this._gauntletLayer).Input.IsHotKeyPressed("TakeAll"))
            {
                this.ExecuteTakeAll();
            }
            else
            {
                if (!((ScreenLayer)this._gauntletLayer).Input.IsHotKeyPressed("GiveAll"))
                    return;
                this.ExecuteGiveAll();
            }
        }

        public TroopDesignerScreen(TroopDesignerState inventoryState)
        {
            this._troopDesignerState = inventoryState;
            this._troopDesignerState.Handler = (IInventoryStateHandler)this;
            this._troopDesignerState.RegisterListener(this);
        }

        protected override void OnInitialize()
        {
            SpriteData spriteData = UIResourceManager.SpriteData;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            ResourceDepot uiResourceDepot = UIResourceManager.UIResourceDepot;
            this._inventoryCategory = spriteData.SpriteCategories["ui_inventory"];
            this._inventoryCategory.Load((ITwoDimensionResourceContext)resourceContext, uiResourceDepot);
            InventoryLogic inventoryLogic = this._troopDesignerState.InventoryLogic;
            Mission current = Mission.Current;
            this._dataSource = new TroopDesignerVM(inventoryLogic, current != null && current.DoesMissionRequireCivilianEquipment, new Func<WeaponComponentData, ItemObject.ItemUsageSetFlags>(this.GetItemUsageSetFlag), this.GetFiveStackShortcutkeyText(), this.GetEntireStackShortcutkeyText());
            this._dataSource.SetGetKeyTextFromKeyIDFunc(new Func<string, TextObject>(Game.Current.GameTextManager.GetHotKeyGameTextFromKeyID));
            this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("InventoryHotKeyCategory").RegisteredHotKeys.FirstOrDefault<HotKey>((Func<HotKey, bool>)(g => g?.Id == "Exit")));
            this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("InventoryHotKeyCategory").RegisteredHotKeys.FirstOrDefault<HotKey>((Func<HotKey, bool>)(g => g?.Id == "Confirm")));
            this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("InventoryHotKeyCategory").RegisteredHotKeys.FirstOrDefault<HotKey>((Func<HotKey, bool>)(g => g?.Id == "Reset")));
            this._dataSource.SetPreviousCharacterInputKey(HotKeyManager.GetCategory("InventoryHotKeyCategory").RegisteredHotKeys.FirstOrDefault<HotKey>((Func<HotKey, bool>)(g => g?.Id == "SwitchToPreviousTab")));
            this._dataSource.SetNextCharacterInputKey(HotKeyManager.GetCategory("InventoryHotKeyCategory").RegisteredHotKeys.FirstOrDefault<HotKey>((Func<HotKey, bool>)(g => g?.Id == "SwitchToNextTab")));
            this._dataSource.SetBuyAllInputKey(HotKeyManager.GetCategory("InventoryHotKeyCategory").RegisteredHotKeys.FirstOrDefault<HotKey>((Func<HotKey, bool>)(g => g?.Id == "TakeAll")));
            this._dataSource.SetSellAllInputKey(HotKeyManager.GetCategory("InventoryHotKeyCategory").RegisteredHotKeys.FirstOrDefault<HotKey>((Func<HotKey, bool>)(g => g?.Id == "GiveAll")));
            GauntletLayer gauntletLayer = new GauntletLayer(15, "GauntletLayer", true);
            ((ScreenLayer)gauntletLayer).IsFocusLayer = true;
            this._gauntletLayer = gauntletLayer;
            ((ScreenLayer)this._gauntletLayer).InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            this.AddLayer((ScreenLayer)this._gauntletLayer);
            ScreenManager.TrySetFocus((ScreenLayer)this._gauntletLayer);
            ((ScreenLayer)this._gauntletLayer).Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("InventoryHotKeyCategory"));
            this._gauntletMovie = this._gauntletLayer.LoadMovie("TroopDesigner", this._dataSource);
            this._openedFromMission = ((GameState)this._troopDesignerState).Predecessor is MissionState;
            InformationManager.ClearAllMessages();
        }

        private string GetFiveStackShortcutkeyText() => !Input.IsControllerConnected || Input.IsMouseActive ? Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", "anyshift").ToString() : string.Empty;

        private string GetEntireStackShortcutkeyText() => !Input.IsControllerConnected || Input.IsMouseActive ? Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", "anycontrol").ToString() : (string)null;

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            this._closed = true;
            LoadingWindow.EnableGlobalLoadingWindow();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this._dataSource?.RefreshCallbacks();
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            this._gauntletMovie = (IGauntletMovie)null;
            this._inventoryCategory.Unload();
            this._dataSource.OnFinalize();
            this._dataSource = (TroopDesignerVM)null;
            this._gauntletLayer = (GauntletLayer)null;
        }

        void IGameStateListener.OnActivate() => Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(TutorialContexts.InventoryScreen));

        void IGameStateListener.OnDeactivate() => Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(TutorialContexts.None));

        void IGameStateListener.OnInitialize()
        {
        }

        void IGameStateListener.OnFinalize()
        {
        }

        void IInventoryStateHandler.FilterInventoryAtOpening(
          InventoryManager.InventoryCategoryType inventoryCategoryType)
        {
            switch ((int)inventoryCategoryType - 1)
            {
                case 0:
                    this._dataSource.ExecuteFilterArmors();
                    break;
                case 1:
                    this._dataSource.ExecuteFilterWeapons();
                    break;
                case 3:
                    this._dataSource.ExecuteFilterMounts();
                    break;
                case 4:
                    this._dataSource.ExecuteFilterMisc();
                    break;
            }
        }

        public void ExecuteLootingScript() => this._dataSource.ExecuteBuyAllItems();

        public void ExecuteSellAllLoot() => this._dataSource.ExecuteSellAllItems();

        public void ExecuteCancel()
        {
            if (this._dataSource.ItemPreview.IsSelected)
                this._dataSource.ClosePreview();
            else
                this._dataSource.ExecuteResetAndCompleteTranstactions();
        }

        public void ExecuteConfirm()
        {
            if (this._dataSource.ItemPreview.IsSelected)
                return;
            this._dataSource.ExecuteCompleteTranstactions();
        }

        public void ExecuteSwitchToPreviousTab()
        {
            if (this._dataSource.ItemPreview.IsSelected)
                return;
            this._dataSource.CharacterList.ExecuteSelectPreviousItem();
        }

        public void ExecuteSwitchToNextTab()
        {
            if (this._dataSource.ItemPreview.IsSelected)
                return;
            this._dataSource.CharacterList.ExecuteSelectNextItem();
        }

        public void ExecuteTakeAll()
        {
            if (this._dataSource.ItemPreview.IsSelected)
                return;
            this._dataSource.ExecuteBuyAllItems();
        }

        public void ExecuteGiveAll()
        {
            if (this._dataSource.ItemPreview.IsSelected)
                return;
            this._dataSource.ExecuteSellAllItems();
        }

        public void ExecuteBuyConsumableItem() => this._dataSource.ExecuteBuyItemTest();

        private ItemObject.ItemUsageSetFlags GetItemUsageSetFlag(WeaponComponentData item) => !string.IsNullOrEmpty(item.ItemUsage) ? MBItem.GetItemUsageSetFlags(item.ItemUsage) : (ItemObject.ItemUsageSetFlags)0;

        private void CloseInventoryScreen() => InventoryManager.Instance.CloseInventoryPresentation(false);

        bool IChangeableScreen.AnyUnsavedChanges() => this._troopDesignerState.InventoryLogic.IsThereAnyChanges();

        bool IChangeableScreen.CanChangesBeApplied() => this._troopDesignerState.InventoryLogic.CanPlayerCompleteTransaction();

        void IChangeableScreen.ApplyChanges()
        {
            this._dataSource.ItemPreview.Close();
            this._troopDesignerState.InventoryLogic.DoneLogic();
        }

        void IChangeableScreen.ResetChanges() => this._troopDesignerState.InventoryLogic.Reset(true);
    }
}
