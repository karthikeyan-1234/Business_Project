﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.DTOs
{
    public class InventoryDTO
    {
        public int id { get; set; }
        public int itemId { get; set; }
        public float qty { get; set; }
        public DateTime lastUpdated { get; set; }
    }
}
