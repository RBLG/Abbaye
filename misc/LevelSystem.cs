using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abbaye.misc {
    public class LevelSystem {

        private int level_threshold = 10;

        public int Xp { get; private set; }

        public void AddXp(int value) {
            Xp += value;
            if (Xp >= level_threshold) {
                Level = +1;
                Xp -= level_threshold;
                level_threshold = GetThreshold(Level);
            }
        }

        public int Level { get; set; } = 1;

        public static int GetThreshold(int level) {
            return 10 * level;
        }

    }
}
