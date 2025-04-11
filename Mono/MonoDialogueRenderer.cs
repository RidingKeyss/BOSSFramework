// BOSSFramework - Mono/MonoDialogueRenderer.cs
// Mono implementation of IDialogueRenderer

using BOSSCoreShared;
using ScheduleOne.UI;

namespace BOSSMono
{
    public class MonoDialogueRenderer : IDialogueRenderer
    {
        private readonly WorldspaceDialogueRenderer _renderer;

        public MonoDialogueRenderer(WorldspaceDialogueRenderer renderer)
        {
            _renderer = renderer;
        }

        public void ShowText(string text, float duration)
        {
            _renderer.ShowText(text, duration);
        }
    }
}
