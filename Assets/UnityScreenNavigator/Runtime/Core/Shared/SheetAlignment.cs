using System;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    public enum SheetAlignment
    {
        Left,
        Top,
        Right,
        Bottom,
        Center
    }

    public static class SheetAlignmentExtensions
    {
        public static Vector3 ToPosition(this SheetAlignment self, RectTransform rectTransform)
        {
            Vector3 position;
            var rect = rectTransform.rect;
            var width = rect.width;
            var height = rect.height;
            var z = rectTransform.localPosition.z;
            switch (self)
            {
                case SheetAlignment.Left:
                    position = new Vector3(-width, 0, z);
                    break;
                case SheetAlignment.Top:
                    position = new Vector3(0, height, z);
                    break;
                case SheetAlignment.Right:
                    position = new Vector3(width, 0, z);
                    break;
                case SheetAlignment.Bottom:
                    position = new Vector3(0, -height, z);
                    break;
                case SheetAlignment.Center:
                    position = new Vector3(0, 0, z);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(self), self, null);
            }

            return position;
        }
    }
}
