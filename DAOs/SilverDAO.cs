using BOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class SilverDAO
    {
        private SilverJewelry2023DbContext context;
        private static SilverDAO instance;

        public static SilverDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SilverDAO();
                }
                return instance;
            }
        }

        public SilverDAO()
        {
            context = new SilverJewelry2023DbContext();
        }

        public bool AddJewelry(SilverJewelry jewelry)
        {
            Category existedCategory = this.context.Categories
                    .FirstOrDefault(o => o.CategoryId == jewelry.CategoryId);
            if (existedCategory == null)
            {
                throw new Exception("Category not exist");
            }
            bool result = false;
            SilverJewelry check = this.GetSilverJewelryById(jewelry.SilverJewelryId);
            if (check == null)
            {

                try
                {
                    /* SilverJewelry silverJewelry = new SilverJewelry()
                     {
                         SilverJewelryId = jewelry.SilverJewelryId,
                         SilverJewelryName = jewelry.SilverJewelryName,
                         MetalWeight = jewelry.MetalWeight,
                         Price = jewelry.Price,
                         ProductionYear = jewelry.ProductionYear,
                         SilverJewelryDescription = jewelry.SilverJewelryDescription,
                         CategoryId = jewelry.CategoryId,
                         CreatedDate = DateTime.Now

                     };*/
                    jewelry.CreatedDate = DateTime.Now;
                    context.SilverJewelries.Add(jewelry);
                    result = context.SaveChanges() > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine(ex.ToString());
                }
            }
            return result;
        }
        public bool RemoveJewelry(String id)
        {
            bool result = false;
            SilverJewelry check = this.GetSilverJewelryById(id);
            if (check != null)
            {

                try
                {
                    context.SilverJewelries.Remove(check);
                    result = context.SaveChanges() > 0 ? true : false;
                }
                catch (Exception ex)
                {
                }
            }
            return result;
        }
        public bool UpdateJewelry(SilverJewelry silverJewelry)
        {
            bool result = false;
            SilverJewelry check = this.GetSilverJewelryById(silverJewelry.SilverJewelryId);
            if (check != null)
            {
                context.Entry(check).State = EntityState.Detached;

                try
                {
                    /*                    context.Entry(check).CurrentValues.SetValues(silverJewelry);*/
                    context.Entry(silverJewelry).State = EntityState.Modified;
                    result = context.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine(ex.ToString());
                }
            }
            return result;
        }
        private SilverJewelry GetSilverJewelryById(String id)
        {
            return context.SilverJewelries.SingleOrDefault(x => x.SilverJewelryId.Equals(id));
        }
        public List<SilverJewelry> GetAll()
        {
            return context.SilverJewelries.ToList();
        }
        public SilverJewelry GetById(String id)
        {
            return context.SilverJewelries.SingleOrDefault(x => x.SilverJewelryId.Equals(id));
        }

    }
}
