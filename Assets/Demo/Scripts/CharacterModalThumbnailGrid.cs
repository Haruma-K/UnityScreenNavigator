using System;
using UnityEngine;

namespace Demo.Scripts
{
    public class CharacterModalThumbnailGrid : MonoBehaviour
    {
        [SerializeField] private CharacterModalThumbnail _firstThumb;
        [SerializeField] private CharacterModalThumbnail _secondThumb;
        [SerializeField] private CharacterModalThumbnail _thirdThumb;

        public event Action<int> ThumbnailClicked;

        public void Setup(int characterId)
        {
            _firstThumb.Setup(characterId, 1);
            _firstThumb.Clicked += () => ThumbnailClicked?.Invoke(0);
            _secondThumb.Setup(characterId, 2);
            _secondThumb.Clicked += () => ThumbnailClicked?.Invoke(1);
            _thirdThumb.Setup(characterId, 3);
            _thirdThumb.Clicked += () => ThumbnailClicked?.Invoke(2);
        }
    }
}