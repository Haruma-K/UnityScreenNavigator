namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    public sealed class SheetHideContext
    {
        private SheetHideContext(Sheet exitSheet)
        {
            ExitSheet = exitSheet;
        }

        public Sheet ExitSheet { get; }

        public static SheetHideContext Create(Sheet exitSheet)
        {
            return new SheetHideContext(exitSheet);
        }
    }
}