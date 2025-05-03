using System;
using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    internal readonly struct PagePushContext
    {
        public string EnterPageId { get; }
        public Page EnterPage { get; }

        public string ExitPageId { get; }
        public Page ExitPage { get; }

        public bool IsStacked { get; }

        private PagePushContext(string enterPageId, Page enterPage, string exitPageId, Page exitPage, bool isStacked)
        {
            EnterPageId = enterPageId;
            EnterPage = enterPage;
            ExitPageId = exitPageId;
            ExitPage = exitPage;
            IsStacked = isStacked;
        }

        public static PagePushContext Create(
            string pageId,
            Page enterPage,
            List<string> orderedPageIds,
            Dictionary<string, Page> pages,
            bool isStacked
        )
        {
            var hasExit = orderedPageIds.Count > 0;
            var exitPageId = hasExit ? orderedPageIds[orderedPageIds.Count - 1] : null;

            var exitPage = hasExit ? pages[exitPageId] : null;

            var resolvedPageId = pageId ?? Guid.NewGuid().ToString();

            return new PagePushContext(resolvedPageId, enterPage, exitPageId, exitPage, isStacked);
        }
    }
}