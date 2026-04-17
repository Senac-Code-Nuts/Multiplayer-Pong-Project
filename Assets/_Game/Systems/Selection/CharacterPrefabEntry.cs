using System;
using UnityEngine;

namespace Pong.Systems.Selection
{
    [Serializable]
    public struct CharacterPrefabEntry
    {
        public CharacterType CharacterType;
        public GameObject Prefab;
    }
}