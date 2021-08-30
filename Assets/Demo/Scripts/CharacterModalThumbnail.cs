using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Demo.Scripts
{
    public class CharacterModalThumbnail : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;

        public event Action Clicked;

        private void Awake()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        public void Setup(int id, int rank)
        {
            var sprite = Resources.Load<Sprite>(ResourceKey.CharacterThumbnailSprite(id, rank));
            _image.sprite = sprite;
        }

        private void OnClicked()
        {
            Clicked?.Invoke();
        }
    }
}