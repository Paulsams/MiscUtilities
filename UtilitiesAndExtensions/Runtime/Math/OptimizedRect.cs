using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Paulsams.MicsUtil
{
    public struct OptimizedRect
    {
        public readonly float2 Position;
        public readonly float2 Size;

        private readonly float _xMin;
        private readonly float _yMin;

        private readonly float _xMax;
        private readonly float _yMax;

        public OptimizedRect(float x, float y, float width, float height)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);

            _xMin = x;
            _yMin = y;

            _xMax = _xMin + width;
            _yMax = _yMin + height;
        }

        public OptimizedRect(float2 position, float2 size)
        {
            Position = position;
            Size = size;

            _xMin = position.x;
            _yMin = position.y;

            _xMax = position.x + size.x;
            _yMax = position.y + size.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(OptimizedRect other)
        {
            return other._xMax > _xMin && other._xMin < _xMax && other._yMax > _yMin && other._yMin < _yMax;
        }

        public static explicit operator Rect(OptimizedRect myRect) => new Rect(myRect.Position, myRect.Size);
    }
}