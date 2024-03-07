using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abbaye.myway {
    public interface IAttack {

        int Damage { get; }

        void TemporaryDisable();

        void ConfirmHit();
    }
}
