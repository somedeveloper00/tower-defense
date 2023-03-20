using UnityEngine;

namespace TowerDefense.Transport.Editor
{
    internal class TransportableConvertObjectHolder : ScriptableObject
    {
        [SerializeReference] public ITransportable Object;
    }
}