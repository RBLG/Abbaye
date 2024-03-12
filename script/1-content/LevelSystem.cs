using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abbaye.script.misc {
    public class LevelSystem {

        private int level_threshold = 5;

        public int Xp { get; private set; }

        public void AddXp(int value) {
            Xp += value;
            if (Xp >= level_threshold) {
                Level += 1;
                Xp -= level_threshold;
                level_threshold = GetThreshold(Level);
                OnLevelUp?.Invoke(Level);
            }
        }

        public int Level { get; set; } = 0;



        public static int GetThreshold(int level) {

            return (int)Math.Min(20 + 20L * level * level, int.MaxValue);
            //0:20
            //1:40
            //2:90
            //3:190
            //4:330
            //5:510
            //6:730
        }

        public float GetCompletionRatio() {
            return Xp / (float)level_threshold;
        }

        public delegate void OnLevelUpEventHandler(int lvl);
        public OnLevelUpEventHandler? OnLevelUp { get; set; }

    }
}
