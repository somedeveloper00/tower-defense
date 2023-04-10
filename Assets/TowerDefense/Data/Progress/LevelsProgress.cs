﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TowerDefense.Transport;

namespace TowerDefense.Data.Progress {
    [Serializable]
    public class LevelProgress : ITransportable {
        
        public List<Level> levels = new(0);

        [Serializable]
        public class Level {
            public int id;
            public LevelStatus status;
            public int stars = 0;
            public int playCount = 0;
        }

        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) => JsonConvert.PopulateObject( json, this );

        [Flags]
        public enum LevelStatus {
            Unlocked = 1 << 1,
            Finished = 1 << 2,
        }
    }
}