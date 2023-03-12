using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation;
using Demo.Core.Scripts.View.Foundation.Binders;
using Demo.Core.Scripts.View.Parts.UnitPortrait;
using Demo.Core.Scripts.View.Parts.UnitThumbnail;
using Demo.Subsystem.GUIComponents.TabGroup;
using Demo.Subsystem.PresentationFramework;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Sheet;

namespace Demo.Core.Scripts.View.UnitTypeInformation
{
    public sealed class UnitTypeInformationView : AppView<UnitTypeInformationViewState>
    {
        public TMP_Text titleText;
        public UnitThumbnailView rank1ThumbnailView;
        public UnitThumbnailView rank2ThumbnailView;
        public UnitThumbnailView rank3ThumbnailView;
        public TabGroup unitPortraitTabGroup;
        public TMP_Text descriptionText;
        public Button closeButton;
        public Button expandButton;

        public RectTransform UnitPortraitRectTransform => (RectTransform)unitPortraitTabGroup.sheetContainer.transform;

        protected override async UniTask Initialize(UnitTypeInformationViewState viewState)
        {
            var internalState = (IUnitTypeInformationState)viewState;

            // Bind state to view.
            titleText.SetTextSource(viewState.Title).AddTo(this);
            viewState.TabIndex.Subscribe(x =>
            {
                switch (x)
                {
                    case 0:
                        descriptionText.text = viewState.Rank1Description.Value;
                        break;
                    case 1:
                        descriptionText.text = viewState.Rank2Description.Value;
                        break;
                    case 2:
                        descriptionText.text = viewState.Rank3Description.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }).AddTo(this);
            closeButton.SetOnClickDestination(internalState.InvokeCloseButtonClicked).AddTo(this);
            expandButton.SetOnClickDestination(internalState.InvokeExpandButtonClicked).AddTo(this);

            // Bind state from child views.
            unitPortraitTabGroup.OnTabLoaded
                .Subscribe(x =>
                {
                    Task WillEnter()
                    {
                        viewState.TabIndex.Value = x.Index;
                        return Task.CompletedTask;
                    }

                    var unitImageSheet = (UnitPortraitSheet)x.Sheet;

                    switch (x.Index)
                    {
                        case 0:
                            unitImageSheet.Setup(viewState.Rank1Portrait);
                            break;
                        case 1:
                            unitImageSheet.Setup(viewState.Rank2Portrait);
                            break;
                        case 2:
                            unitImageSheet.Setup(viewState.Rank3Portrait);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    x.Sheet.AddLifecycleEvent(onWillEnter: WillEnter);
                })
                .AddTo(this);

            await UniTask.WhenAll(
                rank1ThumbnailView.InitializeAsync(viewState.Rank1Thumbnail),
                rank2ThumbnailView.InitializeAsync(viewState.Rank2Thumbnail),
                rank3ThumbnailView.InitializeAsync(viewState.Rank3Thumbnail),
                unitPortraitTabGroup.InitializeAsync()
            );
        }
    }
}
