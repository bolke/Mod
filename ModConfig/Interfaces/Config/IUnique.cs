using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Interfaces.Config
{
    public interface IUnique
    {
        Guid UniqueId
        {
            get;
        }
    }
}
