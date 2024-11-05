using BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class CategoryDAO
    {
        private SilverJewelry2023DbContext context;
        private static CategoryDAO instance;
        public static CategoryDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CategoryDAO();
                }
                return instance;
            }
        }

        public CategoryDAO()
        {
            context = new SilverJewelry2023DbContext();
        }

        public List<Category> GetAll()
        {
            return context.Categories.ToList();
        }

        public Category GetById(String id)
        {
            return context.Categories.SingleOrDefault(x => x.CategoryId.Equals(id));
        }
    }
}
