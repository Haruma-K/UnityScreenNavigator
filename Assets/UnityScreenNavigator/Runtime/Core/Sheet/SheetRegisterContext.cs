using System;
using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation.AssetLoader;

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    public sealed class SheetRegisterContext
    {
        public Type SheetType { get; }
        public string ResourceKey { get; }
        public string SheetId { get; }
        public Sheet Sheet { get; private set; }
        public AssetLoadHandle<GameObject> AssetLoadHandle { get; private set; }

        public SheetRegisterContext(Type sheetType, string resourceKey, string sheetId = null)
        {
            SheetType = sheetType;
            ResourceKey = resourceKey;
            SheetId = sheetId ?? Guid.NewGuid().ToString();
        }

        public void SetSheet(Sheet sheet)
        {
            Sheet = sheet;
        }

        public void SetAssetLoadHandle(AssetLoadHandle<GameObject> handle)
        {
            AssetLoadHandle = handle;
        }
    }
}