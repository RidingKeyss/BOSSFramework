// BOSSFramework - Il2Cpp/Il2CppDialogueRenderer.cs
// IL2CPP implementation of IDialogueRenderer
using BOSSCoreShared;
using Il2CppScheduleOne.UI;

namespace BOSSIl2Cpp
{
    public class Il2CppDialogueRenderer : IDialogueRenderer
    {
        private readonly WorldspaceDialogueRenderer _renderer;

        public Il2CppDialogueRenderer(WorldspaceDialogueRenderer renderer)
        {
            _renderer = renderer;
        }

        public void ShowText(string text, float duration)
        {
            _renderer.ShowText(text, duration);
        }
    }
}
