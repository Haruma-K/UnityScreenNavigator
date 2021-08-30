using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Modal;

namespace Demo
{
    [RequireComponent(typeof(Button))]
    public class PopModalButtonPresenter : MonoBehaviour
    {
        [SerializeField] private string _containerName;
        [SerializeField] private bool _playAnimation = true;

        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            var modalContainer = string.IsNullOrEmpty(_containerName)
                ? ModalContainer.Of(transform)
                : ModalContainer.Find(_containerName);
            modalContainer.Pop(_playAnimation);
        }
    }
}