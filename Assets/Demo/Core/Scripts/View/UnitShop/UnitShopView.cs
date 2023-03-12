using System;
using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation.Binders;
using Demo.Subsystem.GUIComponents.TabGroup;
using Demo.Subsystem.PresentationFramework;
using UniRx;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.UnitShop
{
    public sealed class UnitShopView : AppView<UnitShopViewState>
    {
        public Button backButton;
        public TabGroup itemSetTabGroup;

        protected override async UniTask Initialize(UnitShopViewState viewState)
        {
            backButton.SetOnClickDestination(viewState.InvokeBackButtonClicked).AddTo(this);

            itemSetTabGroup.OnTabLoaded
                .Subscribe(x =>
                {
                    var itemSetSheet = (UnitShopItemSetSheet)x.Sheet;

                    switch (x.Index)
                    {
                        case 0:
                            itemSetSheet.Setup(viewState.RegularItems);
                            break;
                        case 1:
                            itemSetSheet.Setup(viewState.SpecialItems);
                            break;
                        case 2:
                            itemSetSheet.Setup(viewState.SaleItems);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                })
                .AddTo(this);

            await itemSetTabGroup.InitializeAsync();
        }
    }
}
