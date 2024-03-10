using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abbaye.script.content
{
    public interface IAttack
    {

        int Damage { get; }

        void TemporaryDisable();

        void ConfirmHit();
    }
}
