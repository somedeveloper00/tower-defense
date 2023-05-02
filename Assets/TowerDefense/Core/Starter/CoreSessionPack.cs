using System;
using System.Collections.Generic;

namespace TowerDefense.Core.Starter {
    /// <summary>
    /// data for a core session. like coins and defenders etc.
    /// </summary>
    [Serializable]
    public class CoreSessionPack {
        public int life = 20;
        public ulong coins;
        public List<string> defenders = new();
    }
}