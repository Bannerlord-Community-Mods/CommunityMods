// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomGame
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D5D21862-28AB-45FC-8C12-16AF95A20751
// Assembly location: D:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord - Beta\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CommunityTest
{
    public class CommunityTestGame : TaleWorlds.Core.GameType
    {
        private int _stepNo;

        public static CommunityTestGame Current
        {
            get
            {
                return Game.Current.GameType as CommunityTestGame;
            }
        }

        public CommunityTestGame()
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Game currentGame = this.CurrentGame;
            currentGame.FirstInitialize();
            IGameStarter gameStarter = (IGameStarter)new BasicGameStarter();
            this.InitializeGameModels(gameStarter);
            this.GameManager.OnGameStart(this.CurrentGame, gameStarter);
            MBObjectManager objectManager = currentGame.ObjectManager;
            currentGame.SecondInitialize(gameStarter.Models);
            currentGame.CreateGameManager();
            this.GameManager.BeginGameStart(this.CurrentGame);
            this.CurrentGame.RegisterBasicTypes();
            this.CurrentGame.ThirdInitialize();
            currentGame.CreateObjects();
            currentGame.InitializeDefaultGameObjects();
            currentGame.LoadBasicFiles(false);
            this.ObjectManager.LoadXML("Items", (Type)null);
            this.ObjectManager.LoadXML("MPCharacters", (Type)null);
            this.ObjectManager.LoadXML("BasicCultures", (Type)null);
            this.ObjectManager.LoadXML("MPClassDivisions", (Type)null);
            objectManager.ClearEmptyObjects();
            currentGame.SetDefaultEquipments((IReadOnlyDictionary<string, Equipment>)new Dictionary<string, Equipment>());

            if (currentGame.BasicModels.SkillList == null)
            {
                throw new Exception("haha");
            }
            currentGame.CreateLists();
            objectManager.ClearEmptyObjects();
            this.AddGameTexts();
            this.GameManager.OnCampaignStart(this.CurrentGame, (object)null);
            this.GameManager.OnAfterCampaignStart(this.CurrentGame);
            this.GameManager.OnGameInitializationFinished(this.CurrentGame);
        }

        protected override void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState)
        {
            nextState = GameTypeLoadingStates.None;
            switch (gameTypeLoadingState)
            {
                case GameTypeLoadingStates.InitializeFirstStep:
                    base.CurrentGame.Initialize();
                    nextState = GameTypeLoadingStates.WaitSecondStep;
                    return;
                case GameTypeLoadingStates.WaitSecondStep:
                    nextState = GameTypeLoadingStates.LoadVisualsThirdState;
                    return;
                case GameTypeLoadingStates.LoadVisualsThirdState:
                    nextState = GameTypeLoadingStates.PostInitializeFourthState;
                    break;
                case GameTypeLoadingStates.PostInitializeFourthState:
                    break;
                default:
                    return;
            }
        }


        public override void OnDestroy()
        {
        }

        private void InitializeGameModels(IGameStarter basicGameStarter)
        {
            basicGameStarter.AddModel((GameModel)new MultiplayerAgentDecideKilledOrUnconsciousModel());
            basicGameStarter.AddModel((GameModel)new CustomBattleAgentStatCalculateModel());
            basicGameStarter.AddModel((GameModel)new MultiplayerAgentApplyDamageModel());
            basicGameStarter.AddModel((GameModel)new DefaultRidingModel());
            basicGameStarter.AddModel((GameModel)new DefaultStrikeMagnitudeModel());
            basicGameStarter.AddModel((GameModel)new CaptureTheBannerLordSkillList());
            basicGameStarter.AddModel((GameModel)new MultiplayerBattleMoraleModel());

            // We should use these, but some classes are internal.
            // basicGameStarter.AddModel((GameModel) new MultiplayerSkillList());
            // basicGameStarter.AddModel((GameModel) new MultiplayerRidingModel());
            // basicGameStarter.AddModel((GameModel) new MultiplayerStrikeMagnitudeModel());
            // basicGameStarter.AddModel((GameModel) new MultiplayerAgentDecideKilledOrUnconsciousModel());
            // basicGameStarter.AddModel((GameModel) new MultiplayerAgentStatCalculateModel());
            // basicGameStarter.AddModel((GameModel) new MultiplayerAgentApplyDamageModel());
            basicGameStarter.AddModel((GameModel)new MultiplayerBattleMoraleModel());
        }

        protected override void OnRegisterTypes()
        {
            base.OnRegisterTypes();
            base.OnRegisterTypes();
            base.ObjectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "MPCharacters", true);
            base.ObjectManager.RegisterType<BasicCultureObject>("Culture", "BasicCultures", true);
            base.ObjectManager.RegisterType<MultiplayerClassDivisions.MPHeroClass>("MPClassDivision", "MPClassDivisions", true);
        }

        private void AddGameTexts()
        {
            this.CurrentGame.GameTextManager.LoadGameTexts(BasePath.Name + "Modules/Native/ModuleData/multiplayer_strings.xml");
            this.CurrentGame.GameTextManager.LoadGameTexts(BasePath.Name + "Modules/Native/ModuleData/global_strings.xml");
            this.CurrentGame.GameTextManager.LoadGameTexts(BasePath.Name + "Modules/Native/ModuleData/module_strings.xml");
            // this.CurrentGame.GameTextManager.LoadGameTexts(BasePath.Name + "Modules/CaptureTheBannerLord/ModuleData/module_strings.xml");
        }
    }

    class CaptureTheBannerLordSkillList : SkillList
    {
        internal CaptureTheBannerLordSkillList()
        {
        }

        public override IEnumerable<SkillObject> GetSkillList()
        {
            yield return DefaultSkills.OneHanded;
            yield return DefaultSkills.TwoHanded;
            yield return DefaultSkills.Polearm;
            yield return DefaultSkills.Bow;
            yield return DefaultSkills.Crossbow;
            yield return DefaultSkills.Throwing;
            yield return DefaultSkills.Riding;
            yield return DefaultSkills.Athletics;
            yield return DefaultSkills.Tactics;
            yield return DefaultSkills.Scouting;
            yield return DefaultSkills.Roguery;
            yield return DefaultSkills.Crafting;
            yield return DefaultSkills.Charm;
            yield return DefaultSkills.Trade;
            yield return DefaultSkills.Leadership;
            yield return DefaultSkills.Steward;
            yield return DefaultSkills.Medicine;
            yield return DefaultSkills.Engineering;
        }
    }
}