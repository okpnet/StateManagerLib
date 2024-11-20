using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib
{
    public interface IStateful
    {
        void ToPrev();

        void ToNext();
    }
}
