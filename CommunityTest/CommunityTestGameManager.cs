// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomGameManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D5D21862-28AB-45FC-8C12-16AF95A20751
// Assembly location: D:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord - Beta\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
namespace CommunityTest
{
    public class CommunityTestGameManager : MBGameManager
    {
        private int _stepNo;
        private CommunityTestParams _params;

        public CommunityTestGameManager(CommunityTestParams p)
            : base()
        {
            this._params = p;
        }

       
        protected override void  DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
        {
            nextStep = GameManagerLoadingSteps.None;
            switch (gameManagerLoadingStep)
            {
                case GameManagerLoadingSteps.PreInitializeZerothStep:
                    MBGameManager.LoadModuleData(false);
                    MBGlobals.InitializeReferences();
                    new Game(new CommunityTestGame(), this).DoLoading();
                    nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
                    return;
                case GameManagerLoadingSteps.FirstInitializeFirstStep:
                    {
                        bool flag = true;
                        foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
                        {
                            flag = (flag && mbsubModuleBase.DoLoading(Game.Current));
                        }
                        nextStep = (flag ? GameManagerLoadingSteps.WaitSecondStep : GameManagerLoadingSteps.FirstInitializeFirstStep);
                        return;
                    }
                case GameManagerLoadingSteps.WaitSecondStep:
                    MBGameManager.StartNewGame();
                    nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
                    return;
                case GameManagerLoadingSteps.SecondInitializeThirdState:
                    nextStep = (Game.Current.DoLoading() ? GameManagerLoadingSteps.PostInitializeFourthState : GameManagerLoadingSteps.SecondInitializeThirdState);
                    return;
                case GameManagerLoadingSteps.PostInitializeFourthState:
                    nextStep = GameManagerLoadingSteps.FinishLoadingFifthStep;
                    return;
                case GameManagerLoadingSteps.FinishLoadingFifthStep:
                    nextStep = GameManagerLoadingSteps.None;
                    return;
                default:
                    return;
            }
        }

        public override void OnLoadFinished()
        {
            ModuleLogger.Writer.WriteLine("CaptureTheBannerLordGameManager.OnLoadFinished");
            ModuleLogger.Writer.Flush();
            // string scene = "mp_test_bora";
            string scene = "mp_fbx_test";
            // string scene = "battle_test";
            // string scene = "mp_duel_001_winter";
            // string scene = "mp_sergeant_map_001";
            // string scene = "mp_tdm_map_001";
            // string scene = "scn_world_map";
            // string scene = "mp_compact";
            MissionState.OpenNew(
                "CaptureTheBannerLord",
                new MissionInitializerRecord(scene),
                missionController => new MissionBehaviour[3] {
                    new CommunityTestMissionController(this._params),
                    // new BattleTeam1MissionController(),
                    // new TaleWorlds.MountAndBlade.Source.Missions.SimpleMountedPlayerMissionController(),
                    new AgentBattleAILogic(),
                    new FieldBattleController(),
                    // new MissionBoundaryPlacer(),
                }
            );
        }
    }

}