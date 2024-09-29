using UnityEngine;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Аllows you to select a layer in the inspector without bit shift operators, that is,
    /// for comparison with <see cref="P:UnityEngine.GameObject.layer"/>.
    /// </summary>
    [System.Serializable]
    public struct GameObjectLayer
    {
        [SerializeField] private int _layer;

        public static implicit operator int(GameObjectLayer layer)
        {
            return layer._layer;
        }

        public static implicit operator GameObjectLayer(int x)
        {
            return new GameObjectLayer { _layer = x };
        }
    }
}