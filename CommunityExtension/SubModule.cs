using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CommunityExtensions
{
    public class SubModule : MBSubModuleBase
    {
    public  static SubModule _instance;
        protected override void OnSubModuleLoad()
        {
            _instance = this;
        }
        public bool loaded = false;
        public override void OnGameInitializationFinished(Game game)
        {
            loaded = true;
            base.OnGameInitializationFinished(game);
        }
    }
}
