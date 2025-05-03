using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    internal readonly struct PagePopContext
    {
        public IReadOnlyList<string> OrderedPageIds { get; }
        public IReadOnlyDictionary<string, Page> Pages { get; }

        public IReadOnlyList<string> ExitPageIds { get; }
        public IReadOnlyList<Page> ExitPages { get; }

        public string ExitPageId => ExitPageIds[0];
        public Page ExitPage => ExitPages[0];

        public string EnterPageId { get; }
        public Page EnterPage { get; }

        private PagePopContext(
            IReadOnlyList<string> orderedPageIds,
            IReadOnlyDictionary<string, Page> pages,
            IReadOnlyList<string> exitPageIds,
            IReadOnlyList<Page> exitPages,
            string enterPageId,
            Page enterPage
        )
        {
            OrderedPageIds = orderedPageIds;
            Pages = pages;
            ExitPageIds = exitPageIds;
            ExitPages = exitPages;
            EnterPageId = enterPageId;
            EnterPage = enterPage;
        }

        public static PagePopContext Create(
            IReadOnlyList<string> orderedPageIds,
            IReadOnlyDictionary<string, Page> pages,
            int popCount
        )
        {
            var unusedIds = new List<string>();
            var unusedPages = new List<Page>();

            for (var i = orderedPageIds.Count - 1; i >= orderedPageIds.Count - popCount; i--)
            {
                var id = orderedPageIds[i];
                unusedIds.Add(id);
                unusedPages.Add(pages[id]);
            }

            var enterIndex = orderedPageIds.Count - popCount - 1;
            var enterId = enterIndex >= 0 ? orderedPageIds[enterIndex] : null;
            var enter = enterId != null ? pages[enterId] : null;

            return new PagePopContext(orderedPageIds, pages, unusedIds, unusedPages, enterId, enter);
        }
    }
}