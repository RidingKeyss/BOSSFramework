using MelonLoader;

[assembly: MelonInfo(typeof(BOSSMono.Core), "[BOSSFramework]BOSSMono", "1.0.7", "chris", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace BOSSMono
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            if (!MelonUtils.IsGameIl2Cpp())
            {
                MonoInitHooks.Initialize();
            }
        }
    }
}