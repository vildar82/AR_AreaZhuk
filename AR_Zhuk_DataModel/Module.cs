using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_Zhuk_DataModel
{
    /// <summary>
    /// Модуль - ячейка дома
    /// </summary>
    public class Module : IEquatable<Module>
    {   
        public string InsValue { get; private set; }
        public double Length { get; private set; }

        private Module (string insValue, double length)
        {            
            InsValue = insValue;
            Length = length;
        }

        public static Module Create (string insValue, double length)
        {
            var newModule = new Module(insValue, length);
            return newModule;
        }

        public bool Equals (Module other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            var res = InsValue == other.InsValue &&
                      Length == other.Length;
            return res;
        }
    }
}
