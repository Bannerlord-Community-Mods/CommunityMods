using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
namespace CommunityExtension.FbxLoader
{
    public class FBXMissionObject : SynchedMissionObject
    {
        public string FileName;
        protected override void OnPreInit()
        {
            base.OnPreInit();
            List<Mesh> meshes = FBXFileCache.GetByFileName(Scene, FileName);
            foreach(var mesh in meshes)
            {
                this.GameEntity.AddMesh(mesh);
               
            }
        }
    }
}
