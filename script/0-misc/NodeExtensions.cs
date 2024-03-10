
using Godot;
using System;

namespace Abbaye.script.misc
{
    public static class NodeExtensions
    {
        public static RTN GetNodeAs<RTN>(this Node self, string path) where RTN : Node
        {
            return self.GetNode(path) as RTN
                ?? throw new NodeNotFoundException(path);
        }

        public static RTN GetNodeAs<RTN>(this Node _self, Func<Node> func) where RTN : Node
        {
            return func() as RTN
               ?? throw new NodeNotFoundException("!Custom query!");
        }

        public static RTN GetFirstNodeInGroupAs<RTN>(this Node self, string group) where RTN : Node
        {
            return self.GetTree().GetFirstNodeInGroup(group) as RTN
                ?? throw new NodeNotFoundException("Group-" + group);
        }

        /// <summary>
        /// loop frames on a sprite on a timing defined by a timer
        /// </summary>
        public static void AnimOnTimer(this Sprite2D sprite, Timer timer, bool enabled)
        {
            if (enabled && timer.IsStopped())
            {
                sprite.Animate();
                timer.Start();
            }
        }

        public static void Animate(this Sprite2D sprite)
        {
            //loop frames for anim
            sprite.Frame = sprite.Frame < sprite.Hframes - 1 ? sprite.Frame + 1 : 0;
        }

        /// <summary>
        /// flip the sprite based on the direction
        /// </summary>
        public static void FlipOnDir(this Sprite2D sprite, float dir)
        {
            sprite.FlipH = dir < 0;
        }

        public static void UpdateAlpha(this Sprite2D sprite, float nalpha)
        {
            Color color = sprite.SelfModulate;
            color.A = nalpha;
            sprite.SelfModulate = color;
        }
    }
}
