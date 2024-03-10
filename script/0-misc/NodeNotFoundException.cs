using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abbaye.script.misc
{
    public class NodeNotFoundException : Exception
    {
        public NodeNotFoundException(string nodename) : base("node " + nodename + " missing") { }
    }
}
