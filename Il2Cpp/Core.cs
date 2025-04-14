using MelonLoader;

[assembly: MelonInfo(typeof(BOSSIl2Cpp.Core), "[BOSSFramework]BOSSIl2Cpp", "1.0.7", "Ridingkeys", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace BOSSIl2Cpp
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                Il2CppInitHooks.Initialize();
            }
        }
    }
}