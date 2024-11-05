using BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOs
{
    public interface ISilverRepo
    {
        public SilverJewelry GetSilver(string id);
        public List<SilverJewelry> GetSilvers();
        public bool AddJewelry(SilverJewelry silver);
        public bool RemoveJewelry(string id);
        public bool UpdateJewelry(SilverJewelry silver);
    }
}
