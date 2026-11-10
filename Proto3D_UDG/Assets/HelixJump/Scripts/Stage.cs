using System;
using System.Collections.Generic;
using UnityEngine;
namespace HelixJump
{
    [Serializable]
    public sealed class Level
    {
        [Range(1, 11)] public int partCount = 11;
        [Range(0, 11)] public int deathCount = 1;
    }
    [CreateAssetMenu(fileName = "Stage", menuName = "Helix Jump/Stage")]
    public sealed class Stage : ScriptableObject
    {
        public Color backgroundColor = Color.white;
        public Color ballColor = Color.white;
        public Color helixColor = Color.white;
        public List<Level> levels = new List<Level>();
    }
}
