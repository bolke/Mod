using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Interfaces.Containers
{
    public interface IObjectContainer
    {
        object PopObject();
        bool PushObject(object element);
    }
}
