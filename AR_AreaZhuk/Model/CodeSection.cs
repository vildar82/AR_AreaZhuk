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
      public int CountFlats { get; set; }
      public List<int> IdSections = new List<int>();
      public int NumberSection { get; set; }
      public  string SpotOwner { get; set; }

      public Code(string code, int idSection,int countFlats,int numberSection,string spotName)
      {
           
          this.CodeStr = code;
          IdSections.Add(idSection);
          this.CountFlats = countFlats;
          this.NumberSection = numberSection;
          this.SpotOwner = spotName;
      }
    }

    public class CodeSection
    {
        public int CountFloors { get; set; }
        public List<FlatsInSection> SectionsByCountFlats = new List<FlatsInSection>(); 
    }

    public class FlatsInSection
    {
        public int Count { get; set; }
        public List<Code> SectionsByCode = new List<Code>(); 
    }
}
