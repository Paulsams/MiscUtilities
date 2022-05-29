using UnityEngine;

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