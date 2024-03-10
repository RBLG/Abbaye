using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abbaye.script.misc
{
    public class LevelSystem
    {

        private int level_threshold = 5;

        public int Xp { get; private set; }

        public void AddXp(int value)
        {
            Xp += value;
            if (Xp >= level_threshold)
            {
                Level = +1;
                Xp -= level_threshold;
                level_threshold = GetThreshold(Level);
                OnLevelUp?.Invoke(Level);
            }
        }

        public int Level { get; set; } = 0;



        public static int GetThreshold(int level)
        {
            return 10 + level * (1 + level / 2);
            /*return 5 + level * (level switch {
                < 6 => 2,
                < 11 => 5,
                < 16 => 10,
                _ => 20
            });*/
        }

        public float GetCompletionRatio()
        {
            return Xp / (float)level_threshold;
        }

        public delegate void OnLevelUpEventHandler(int lvl);
        public OnLevelUpEventHandler? OnLevelUp { get; set; }

    }
}
