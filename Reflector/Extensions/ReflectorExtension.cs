using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflector.Extensions
{
    public static class ReflectorExtension
    {
        public static IEnumerable<Type> GetParentTypes(this Type type) => Reflector.GetParentTypes(type);
    }
}
