namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    public sealed class SheetRegisterContext
    {
        public string SheetId { get; }
        public Sheet Sheet { get; private set; }

        public SheetRegisterContext(string sheetId, Sheet sheet)
        {
            SheetId = sheetId;
            Sheet = sheet;
        }
    }
}