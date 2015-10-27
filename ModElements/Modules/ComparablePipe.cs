using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Modules
{
  public class ComparablePipe<T>: Pipe<T>{
    public override bool Push(T element)
    {
      lock(Padlock)
      {
        IComparable compare = element as IComparable;
        if(compare != null)
        {
          for(int i = 0; i < Count; i++)
          {
            if(compare.CompareTo(Data[i]) <= 0)
            {
              Insert(i, element);
              return true;
            }
          }
        }
        base.Push(element);
        return true;
      }
    }
  }
}
