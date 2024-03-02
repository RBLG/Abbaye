using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abbaye.misc {
    public static class NodeExtensions {
        public static RTN GetNodeAs<RTN>(this Node self, string path) where RTN : Node {
            return self.GetNode(path) as RTN
                ?? throw new NodeNotFoundException(path);
        }

        public static RTN GetNodeAs<RTN>(this Node _self, Func<Node> func) where RTN : Node {
            return func() as RTN
               ?? throw new NodeNotFoundException("!Custom query!");
        }

        /// <summary>
        /// loop frames on a sprite on a timing defined by a timer
        /// </summary>
        public static void AnimOnTimer(this Sprite2D sprite, Timer timer, bool enabled) {
            if (enabled && timer.IsStopped()) {
                //loop frames for anim
                sprite.Frame = (sprite.Frame < sprite.Hframes-1) ? sprite.Frame + 1 : 0;
                timer.Start();
            }
        }

        /// <summary>
        /// flip the sprite based on the direction
        /// </summary>
        public static void FlipOnDir(this Sprite2D sprite, float dir) {
            sprite.FlipH = dir < 0;
        }
    }
}
