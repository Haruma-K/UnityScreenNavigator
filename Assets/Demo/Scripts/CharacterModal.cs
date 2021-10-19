using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Sheet;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace Demo.Scripts
{
    public class CharacterModal : Modal
    {
        private const int ImageCount = 3;
        [SerializeField] private SheetContainer _imageContainer;
        [SerializeField] private CharacterModalThumbnailGrid thumbnailGrid;
        [SerializeField] private Button _expandButton;
        
        public RectTransform CharacterImageRectTransform => (RectTransform)_imageContainer.transform;
        
        private readonly (int sheetId, CharacterModalImageSheet sheet)[] _imageSheets =
            new (int sheetId, CharacterModalImageSheet sheet)[ImageCount];

        private int _characterId;
        private int _selectedRank;

        public void Setup(int characterId)
        {
            _characterId = characterId;
        }

        public override IEnumerator Initialize()
        {
            var imageSheetHandles = new List<AsyncProcessHandle>();
            for (var i = 0; i < ImageCount; i++)
            {
                var index = i;
                var handle = _imageContainer.Register(ResourceKey.CharacterModalImageSheetPrefab(), x =>
                {
                    _imageSheets[index] = (x.sheetId, (CharacterModalImageSheet) x.instance);
                });
                imageSheetHandles.Add(handle);
            }

            foreach (var handle in imageSheetHandles) yield return handle;
            
            _expandButton.onClick.AddListener(OnExpandButtonClicked);
        }

        public override IEnumerator WillPushEnter()
        {
            for (var i = 0; i < ImageCount; i++)
            {
                _imageSheets[i].sheet.Setup(_characterId, i + 1);
            }
            
            yield return _imageContainer.Show(_imageSheets[0].sheetId, false);
            _selectedRank = 1;
            
            thumbnailGrid.Setup(_characterId);
            thumbnailGrid.ThumbnailClicked += x =>
            {
                if (_imageContainer.IsInTransition)
                {
                    return;
                }

                var targetSheet = _imageSheets[x];
                if (_imageContainer.ActiveSheet.Equals(targetSheet.sheet))
                {
                    return;
                }
                
                var sheetId = targetSheet.sheetId;
                _imageContainer.Show(sheetId, true);
                _selectedRank = x + 1;
            };
        }

        public override IEnumerator Cleanup()
        {
            _expandButton.onClick.RemoveListener(OnExpandButtonClicked);
            yield break;
        }

        private void OnExpandButtonClicked()
        {
            ModalContainer.Find(ContainerKey.MainModalContainer)
                .Push(ResourceKey.CharacterImageModalPrefab(), true, modal =>
                {
                    var characterImageModal = (CharacterImageModal) modal;
                    characterImageModal.Setup(_characterId, _selectedRank);
                });
        }
    }
}