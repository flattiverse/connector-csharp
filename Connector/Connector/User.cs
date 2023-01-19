using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse
{
    public class User
    {
        public readonly long Id;

        internal User(long id) 
        {
            Id = id;
        }
    }
}
