using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Demo.Core.Scripts.View.Overlay
{
    public sealed class ConnectingView : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        private void Start()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public UniTask ShowAsync()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            return UniTask.CompletedTask;
        }

        public UniTask HideAsync()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            return UniTask.CompletedTask;
        }
    }
}
