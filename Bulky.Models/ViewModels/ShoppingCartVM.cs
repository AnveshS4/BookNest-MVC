﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
        /// <summary>
        /// public double OrderTotal { get; set; }
        /// </summary>
        public OrderHeader OrderHeader { get; set; }    

    }
}
