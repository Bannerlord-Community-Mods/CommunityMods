using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace CommunityExtensions.FbxLoader
{
    public class FBXMissionObject : ScriptComponentBehaviour
    {
        public string FileName;

        protected override void OnInit()
        {
            base.OnInit();
        }

        private bool initialized = false;
        private int tick = 0;

        protected override void OnTick(float dt)
        {
            base.OnTick(dt);
            bool isLoading = !SubModule._instance.loaded;
            if (!isLoading && !initialized)
            {
                tick++;

                if (tick > 10)
                {
                    List<Mesh> meshes = FBXFileCache.GetByFileName(Scene, BasePath.Name + FileName);
                    // MetaMesh metaMesh = MetaMesh.CreateMetaMesh();
                    /*  GameEntity ent = GameEntity.CreateEmpty(base.Scene, false);
                      ent.SetGlobalFrame(this.GameEntity.GetGlobalFrame());
                      */
                    foreach (var mesh in meshes)
                    {
                        this.GameEntity.AddMesh(mesh, false);
                        //        ent.AddMesh(mesh);
                    }
                    initialized = true;
                }
            }
        }
    }
}