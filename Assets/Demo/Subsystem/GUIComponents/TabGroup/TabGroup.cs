using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Sheet;

namespace Demo.Subsystem.GUIComponents.TabGroup
{
    public sealed class TabGroup : MonoBehaviour
    {
        public SheetContainer sheetContainer;
        public List<TabSource> sources = new List<TabSource>();
        public int initialIndex;

        private readonly Dictionary<int, string> _indexToSheetIdMap = new Dictionary<int, string>();
        private readonly Subject<TabLoadedEvent> _onTabLoadedSubject = new Subject<TabLoadedEvent>();

        public bool IsInitializing { get; private set; }
        public bool IsInitialized { get; private set; }

        public IObservable<TabLoadedEvent> OnTabLoaded => _onTabLoadedSubject;

        private void Awake()
        {
            _onTabLoadedSubject.AddTo(this);
            _onTabLoadedSubject.Subscribe(x => { x.Sheet.AddLifecycleEvent(); });
        }

        public async UniTask InitializeAsync()
        {
            if (IsInitialized)
                throw new InvalidOperationException($"{nameof(TabGroup)} is Already initialized.");

            if (IsInitializing)
                throw new InvalidOperationException($"{nameof(TabGroup)} is initializing.");

            IsInitializing = true;

            var registerTasks = new List<UniTask>();
            for (var i = 0; i < sources.Count; i++)
            {
                // Register sheets.
                var source = sources[i];
                var index = i;
                var registerTask = sheetContainer.Register(source.sheetResourceKey, y =>
                {
                    var sheetId = y.sheetId;
                    var sheet = y.sheet;
                    _indexToSheetIdMap.Add(index, sheetId);
                    _onTabLoadedSubject.OnNext(new TabLoadedEvent(index, sheetId, sheet));

                    if (sheet is ITabContent tabContent)
                        tabContent.SetTabIndex(index);
                }).ToUniTask();
                registerTasks.Add(registerTask);

                // Setup buttons.
                source.button
                    .onClick
                    .AsObservable()
                    .Subscribe(_ =>
                    {
                        if (sheetContainer.IsInTransition)
                            return;

                        if (sheetContainer.ActiveSheetId == _indexToSheetIdMap[index])
                            return;

                        SetActiveTabAsync(index, true).Forget();
                    })
                    .AddTo(this);
            }

            await UniTask.WhenAll(registerTasks);

            // Set initial sheet.
            await SetActiveTabAsync(initialIndex, false);

            IsInitialized = true;
            IsInitializing = false;
        }

        public string GetSheetIdFromIndex(int index)
        {
            return _indexToSheetIdMap[index];
        }

        public async UniTask SetActiveTabAsync(int index, bool playAnimation)
        {
            var sheetId = GetSheetIdFromIndex(index);
            await sheetContainer.Show(sheetId, playAnimation).ToUniTask();
        }

        public readonly struct TabLoadedEvent
        {
            public TabLoadedEvent(int index, string sheetId, Sheet sheet)
            {
                Index = index;
                SheetId = sheetId;
                Sheet = sheet;
            }

            public int Index { get; }
            public string SheetId { get; }
            public Sheet Sheet { get; }
        }

        [Serializable]
        public sealed class TabSource
        {
            public Button button;
            public string sheetResourceKey;
        }
    }
}
