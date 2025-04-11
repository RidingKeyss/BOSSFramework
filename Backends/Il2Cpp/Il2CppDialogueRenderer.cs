// BOSSFramework - Backends/Il2Cpp/Il2CppDialogueRenderer.cs
// IL2CPP implementation of IDialogueRenderer
using BOSSFramework.Shared;
using Il2CppScheduleOne.UI;

namespace BOSSFramework.Backends.Il2Cpp
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
