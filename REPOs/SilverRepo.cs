using BOs;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOs
{
    public class SilverRepo : ISilverRepo
    {
        public bool AddJewelry(SilverJewelry jewelry) => SilverDAO.Instance.AddJewelry(jewelry);

        public bool RemoveJewelry(string id) => SilverDAO.Instance.RemoveJewelry(id);

        public bool UpdateJewelry(SilverJewelry silverJewelry) => SilverDAO.Instance.UpdateJewelry(silverJewelry);
        public List<SilverJewelry> GetSilvers() => SilverDAO.Instance.GetAll();

        public SilverJewelry GetSilver(string id) => SilverDAO.Instance.GetById(id);
    }
}
