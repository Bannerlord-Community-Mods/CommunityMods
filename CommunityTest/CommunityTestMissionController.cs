using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TL = TaleWorlds.Library;

namespace CommunityTest
{
    
    public class CommunityTestMissionController : MissionLogic
    {
        private Game _game;
        public CommunityTestParams CaptureTheBannerLordParams;
        private bool _started;
        public TL.Vec3 freeCameraPosition;
        public Agent _playerAgent;
        private Team playerTeam;
        private Team enemyTeam;

        public CommunityTestMissionController(CommunityTestParams p)
        {
            this._game = Game.Current;
            this.CaptureTheBannerLordParams = p;
        }

        public override void AfterStart()
        {
            try
            {
                this.AfterStart2();
            }
            catch (System.Exception e)
            {
                ModuleLogger.Log("{0}", e);
            }
        }
      
        private List<Agent> playerTeamAgents;
        public override void OnAgentDeleted(Agent affectedAgent)
        {
            base.OnAgentDeleted(affectedAgent);
            if (playerTeamAgents.Contains(affectedAgent))
            {
                playerTeamAgents.Remove(affectedAgent);
            }
        }
        public void RespawnUnits()
        {

            var scene = this.Mission.Scene;
            var xInterval = this.CaptureTheBannerLordParams.soldierXInterval;
            var yInterval = this.CaptureTheBannerLordParams.soldierYInterval;
            var soldiersPerRow = this.CaptureTheBannerLordParams.soldiersPerRow;

            var startPos = this.CaptureTheBannerLordParams.FormationPosition;
            var xDir = this.CaptureTheBannerLordParams.formationDirection;
            var yDir = this.CaptureTheBannerLordParams.formationDirection.LeftVec();
            var agentDefaultDir = new TL.Vec2(0, 1);
            var useFreeCamera = this.CaptureTheBannerLordParams.useFreeCamera;

            BasicCharacterObject soldierCharacter = this._game.ObjectManager.GetObject<BasicCharacterObject>(this.CaptureTheBannerLordParams.playerSoldierCharacterId);
            var playerSoldierFormationClass = soldierCharacter.CurrentFormationClass;
            this.playerTeam = this.Mission.Teams.Add(BattleSideEnum.Attacker, 0xff3f51b5);
            var mapHasNavMesh = false;
            {
                var formation = playerTeam.GetFormation(playerSoldierFormationClass);
                var width = this.getInitialFormationWidth(playerTeam, playerSoldierFormationClass);
                var centerPos = startPos + yDir * (width / 2);
                var wp = new WorldPosition(scene, centerPos.ToVec3());
                formation.SetPositioning(wp, xDir, null);
                formation.FormOrder = FormOrder.FormOrderCustom(width);
                mapHasNavMesh = wp.GetNavMesh() != System.UIntPtr.Zero;
            }
            if (this.playerTeamAgents.Count < this.CaptureTheBannerLordParams.playerSoldierCount)
            {
                var formation = playerTeam.GetFormation(playerSoldierFormationClass);

                AgentBuildData soldierBuildData = new AgentBuildData(new BasicBattleAgentOrigin(soldierCharacter))
                    .ClothingColor1(playerTeam.Color)
                    .ClothingColor2(playerTeam.Color2)
                    .Banner(playerTeam.Banner)
                    .IsFemale(false)
                    .Team(playerTeam)
                    .Formation(formation);

                if (!mapHasNavMesh)
                {
                    var x = this.playerTeamAgents.Count / soldiersPerRow;
                    var y = this.playerTeamAgents.Count % soldiersPerRow;
                    var mat = TL.Mat3.Identity;
                    var pos = startPos + xDir * (-xInterval * x) + yDir * yInterval * y;
                    mat.RotateAboutUp(agentDefaultDir.AngleBetween(xDir));
                    var agentFrame = new TaleWorlds.Library.MatrixFrame(mat, new TL.Vec3(pos.x, pos.y, 30));
                    soldierBuildData.InitialFrame(agentFrame);
                }

                var agent = this.Mission.SpawnAgent(soldierBuildData);
                agent.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
                this.playerTeamAgents.Add(agent);
            }


        }
        public void AfterStart2()
        {
            this._started = true;
            playerTeamAgents = new List<Agent>();
            var scene = this.Mission.Scene;

            if (this.CaptureTheBannerLordParams.skyBrightness >= 0)
            {
                scene.SetSkyBrightness(this.CaptureTheBannerLordParams.skyBrightness);
            }

            if (this.CaptureTheBannerLordParams.rainDensity >= 0)
            {
                scene.SetRainDensity(this.CaptureTheBannerLordParams.rainDensity);
            }

            this.Mission.MissionTeamAIType = Mission.MissionTeamAITypeEnum.FieldBattle;
            this.Mission.SetMissionMode(MissionMode.Battle, true);

            var xInterval = this.CaptureTheBannerLordParams.soldierXInterval;
            var yInterval = this.CaptureTheBannerLordParams.soldierYInterval;
            var soldiersPerRow = this.CaptureTheBannerLordParams.soldiersPerRow;

            var startPos = this.CaptureTheBannerLordParams.FormationPosition;
            var xDir = this.CaptureTheBannerLordParams.formationDirection;
            var yDir = this.CaptureTheBannerLordParams.formationDirection.LeftVec();
            var agentDefaultDir = new TL.Vec2(0, 1);
            var useFreeCamera = this.CaptureTheBannerLordParams.useFreeCamera;

            BasicCharacterObject soldierCharacter = this._game.ObjectManager.GetObject<BasicCharacterObject>(this.CaptureTheBannerLordParams.playerSoldierCharacterId);
            var playerSoldierFormationClass = soldierCharacter.CurrentFormationClass;
            this.playerTeam = this.Mission.Teams.Add(BattleSideEnum.Attacker, 0xff3f51b5);

            playerTeam.AddTeamAI(new TeamAIGeneral(this.Mission, playerTeam));
            playerTeam.AddTacticOption(new TacticCharge(playerTeam));
            // playerTeam.AddTacticOption(new TacticFullScaleAttack(playerTeam));
            playerTeam.ExpireAIQuerySystem();
            playerTeam.ResetTactic();
            this.Mission.PlayerTeam = playerTeam;

            var playerPosVec2 = startPos + xDir * -10 + yDir * -10;
            var playerPos = new TL.Vec3(playerPosVec2.x, playerPosVec2.y, 30);
            if (!useFreeCamera)
            {
                var playerMat = TL.Mat3.Identity;
                playerMat.RotateAboutUp(agentDefaultDir.AngleBetween(xDir));
                BasicCharacterObject playerCharacter = this._game.ObjectManager.GetObject<BasicCharacterObject>(this.CaptureTheBannerLordParams.playerCharacterId);
                AgentBuildData agentBuildData = new AgentBuildData(new BasicBattleAgentOrigin(playerCharacter))
                    .ClothingColor1(0xff3f51b5)
                    .ClothingColor2(0xff3f51b5)
                    .Banner(Banner.CreateRandomBanner())
                    .IsFemale(false)
                    .InitialFrame(new TL.MatrixFrame(playerMat, playerPos));
                Agent player = this.Mission.SpawnAgent(agentBuildData, false, 0);
                player.Controller = Agent.ControllerType.Player;
                player.WieldInitialWeapons();
                player.AllowFirstPersonWideRotation();

                Mission.MainAgent = player;
                player.SetTeam(playerTeam, true);
                playerTeam.GetFormation(playerSoldierFormationClass).PlayerOwner = player;
                playerTeam.PlayerOrderController.Owner = player;
                this._playerAgent = player;
            }
            else
            {
                var c = this.CaptureTheBannerLordParams.playerSoldierCount;
                if (c <= 0)
                {
                    this.freeCameraPosition = new TL.Vec3(startPos.x, startPos.y, 30);
                }
                else
                {
                    var rowCount = (c + soldiersPerRow - 1) / soldiersPerRow;
                    var p = startPos + (System.Math.Min(soldiersPerRow, c) - 1) / 2 * yInterval * yDir - rowCount * xInterval * xDir;
                    this.freeCameraPosition = new TL.Vec3(p.x, p.y, 5);
                }
            }


            BasicCharacterObject enemyCharacter = this._game.ObjectManager.GetObject<BasicCharacterObject>(this.CaptureTheBannerLordParams.enemySoldierCharacterId);
            enemyTeam = this.Mission.Teams.Add(BattleSideEnum.Defender, 0xffff6090);
            enemyTeam.AddTeamAI(new TeamAIGeneral(this.Mission, enemyTeam));
            enemyTeam.AddTacticOption(new TacticCharge(enemyTeam));
            // enemyTeam.AddTacticOption(new TacticFullScaleAttack(enemyTeam));
            enemyTeam.SetIsEnemyOf(playerTeam, true);
            playerTeam.SetIsEnemyOf(enemyTeam, true);
            enemyTeam.ExpireAIQuerySystem();
            enemyTeam.ResetTactic();

            var enemyFormationClass = enemyCharacter.CurrentFormationClass;
            var enemyFormation = enemyTeam.GetFormation(FormationClass.Ranged);
            {
                float width = this.getInitialFormationWidth(enemyTeam, enemyFormationClass);
                var centerPos = startPos + yDir * (width / 2) + xDir * this.CaptureTheBannerLordParams.distance;
                var wp = new WorldPosition(scene, centerPos.ToVec3());
                enemyFormation.SetPositioning(wp, -xDir, null);
                enemyFormation.FormOrder = FormOrder.FormOrderCustom(width);
            }

            for (var i = 0; i < this.CaptureTheBannerLordParams.enemySoldierCount; i += 1)
            {
                AgentBuildData enemyBuildData = new AgentBuildData(new BasicBattleAgentOrigin(enemyCharacter))
                    .ClothingColor1(enemyTeam.Color)
                    .ClothingColor2(enemyTeam.Color2)
                    .Banner(enemyTeam.Banner)
                    .Formation(enemyFormation);


                var agent = this.Mission.SpawnAgent(enemyBuildData);
                agent.SetTeam(enemyTeam, true);
                agent.Formation = enemyFormation;
                agent.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
            }
            {
                var a = this.Mission.IsOrderShoutingAllowed();
                var b = this.Mission.IsAgentInteractionAllowed();
                var c = GameNetwork.IsClientOrReplay;
                var d = playerTeam.PlayerOrderController.Owner == null;
                ModuleLogger.Log("mission allowed shouting: {0} interaction: {1} {2} {3}", a, b, c, d);
            }
        }

        float getInitialFormationWidth(Team team, FormationClass fc)
        {
            var bp = this.CaptureTheBannerLordParams;
            var formation = team.GetFormation(fc);
            var mounted = fc == FormationClass.Cavalry || fc == FormationClass.HorseArcher;
            var unitDiameter = Formation.GetDefaultUnitDiameter(mounted);
            var unitSpacing = 1;
            var interval = mounted ? Formation.CavalryInterval(unitSpacing) : Formation.InfantryInterval(unitSpacing);
            var actualSoldiersPerRow = System.Math.Min(bp.soldiersPerRow, bp.playerSoldierCount);
            var width = (actualSoldiersPerRow - 1) * (unitDiameter + interval) + unitDiameter + 0.1f;
            return width;
        }

        public override bool MissionEnded(ref MissionResult missionResult)
        {
            return this.Mission.InputManager.IsKeyPressed(TaleWorlds.InputSystem.InputKey.Tab);
        }

      

        void SwitchCamera()
        {
            ModuleLogger.Log("SwitchCamera");
            if (this._playerAgent == null || !this._playerAgent.IsActive())
            {
                this.displayMessage("no player agent");
                return;
            }
            if (this.Mission.MainAgent == null)
            {
                this._playerAgent.Controller = Agent.ControllerType.Player;
                this.displayMessage("switch to player agent");
            }
            else
            {
                this.Mission.MainAgent = null;
                this._playerAgent.Controller = Agent.ControllerType.AI;
                var wp = this._playerAgent.GetWorldPosition();
                this._playerAgent.SetScriptedPosition(ref wp, Agent.AIScriptedFrameFlags.DoNotRun, "camera switch");
                this.displayMessage("switch to free camera");
            }
        }

        void displayMessage(string msg)
        {
            InformationManager.DisplayMessage(new InformationMessage(new TaleWorlds.Localization.TextObject(msg, null).ToString()));
        }
    }
}