namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    public sealed class SheetHideContext
    {
        private SheetHideContext(Sheet exitSheet, bool playAnimation)
        {
            ExitSheet = exitSheet;
            PlayAnimation = playAnimation;
        }

        public Sheet ExitSheet { get; }
        public bool PlayAnimation { get; }

        public static SheetHideContext Create(Sheet exitSheet, bool playAnimation)
        {
            return new SheetHideContext(exitSheet, playAnimation);
        }
    }
}