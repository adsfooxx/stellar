﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Dtos
{
    public class GetMakeOrderUseDataResult
    {
        public int UserId {  get; set; }
        public int ProductId {  get; set; }
        public string ProductName { get; set; }
        public decimal Discount {  get; set; }
        public decimal SalesPrice {  get; set; }

    }
}
