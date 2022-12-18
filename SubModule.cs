using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TroopDesignerRewritten.Behavior;
using TroopDesignerRewritten.Manager;
namespace TroopDesignerRewritten
{
    public class SubModule : MBSubModuleBase
    {
        internal static string GetPath(string fileName) => Path.Combine(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..")), "ModuleData", fileName);

        private TroopDesignerTroopRosterCampaignBehavior _troopDesignerBehavior = null;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(new InitialStateOption("TroopDesigner", new TextObject("{=mainMenu_Button}Troop Designer"), 5000, (() => MBGameManager.StartNewGame(new TroopDesignerManager())), () => (false, null)));
           
        
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (gameStarterObject as CampaignGameStarter == null)
                return;
            CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
            _troopDesignerBehavior = new TroopDesignerTroopRosterCampaignBehavior();
            campaignGameStarter.AddBehavior(_troopDesignerBehavior);
        }

        internal static void TryOutputLines(List<string> lines, string fileName, string SucceedMessage, string FailedMessage)
        {
            try
            {
                File.AppendAllLines(GetPath(fileName + ".xml"), (IEnumerable<string>)lines);
                InformationManager.DisplayMessage(new InformationMessage(SucceedMessage));
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage(FailedMessage));
            }
        }

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);
            this._troopDesignerBehavior = null;
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            if (this._troopDesignerBehavior == null)
                return;
            this._troopDesignerBehavior.TickCampaignBehavior();
        }
    }
}
