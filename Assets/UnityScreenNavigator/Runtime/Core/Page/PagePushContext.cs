using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    internal readonly struct PagePushContext
    {
        public string EnterPageId { get; }
        public Page EnterPage { get; }

        public string ExitPageId { get; }
        public Page ExitPage { get; }

        public bool Stack { get; }
        public bool IsExitPageStacked { get; }
        public bool ShouldRemoveExitPage => ExitPage != null && !IsExitPageStacked;

        private PagePushContext(
            string enterPageId,
            Page enterPage,
            string exitPageId,
            Page exitPage,
            bool stack,
            bool isExitPageStacked
        )
        {
            EnterPageId = enterPageId;
            EnterPage = enterPage;
            ExitPageId = exitPageId;
            ExitPage = exitPage;
            Stack = stack;
            IsExitPageStacked = isExitPageStacked;
        }

        public static PagePushContext Create(
            string pageId,
            Page enterPage,
            List<string> orderedPageIds,
            Dictionary<string, Page> pages,
            bool stack,
            bool isExitPageStacked
        )
        {
            var hasExit = orderedPageIds.Count > 0;
            var exitPageId = hasExit ? orderedPageIds[orderedPageIds.Count - 1] : null;

            var exitPage = hasExit ? pages[exitPageId] : null;

            return new PagePushContext(pageId, enterPage, exitPageId, exitPage, stack, isExitPageStacked);
        }
    }
}