using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    internal readonly struct SheetShowContext
    {
        public string SheetId { get; }
        public Sheet EnterSheet { get; }

        public string ExitSheetId { get; }
        public Sheet ExitSheet { get; }

        private SheetShowContext(string sheetId, Sheet enterSheet, string exitSheetId, Sheet exitSheet)
        {
            SheetId = sheetId;
            EnterSheet = enterSheet;
            ExitSheetId = exitSheetId;
            ExitSheet = exitSheet;
        }

        public static SheetShowContext Create(
            string sheetId,
            string currentSheetId,
            Dictionary<string, Sheet> sheets
        )
        {
            var enterSheet = sheets[sheetId];
            var exitSheetId = currentSheetId;
            var exitSheet = exitSheetId != null ? sheets[exitSheetId] : null;

            return new SheetShowContext(sheetId, enterSheet, exitSheetId, exitSheet);
        }
    }
}