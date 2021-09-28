using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechStore.DAL;

namespace TechStore.Models.Home
{
    public class Item
    {
        public Product products { get; set; }
        public int Quantity { get; set; }
    }
}