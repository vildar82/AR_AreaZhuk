using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Model
{
  public  class Code
    {
    
      public string CodeStr { get; set; }
      public List<int> IdSections = new List<int>();

      public Code(string code, int idSection)
      {
          this.CodeStr = code;
          IdSections.Add(idSection);
      }
    }

    public class CodeSection
    {
        public int CountFloors { get; set; }
        public List<Code> Codes = new List<Code>(); 
    }
}
