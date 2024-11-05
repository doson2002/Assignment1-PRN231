using BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOs
{
    public interface ICategoryRepo
    {
        public Category GetCategory(string id);
        public List<Category> GetCategories();
    }
}
