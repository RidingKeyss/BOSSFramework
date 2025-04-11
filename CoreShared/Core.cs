using MelonLoader;

[assembly: MelonInfo(typeof(BOSSCoreShared.Core), "[BOSSFramework]CoreShared", "1.0.7", "chris", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace BOSSCoreShared
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
    }
}