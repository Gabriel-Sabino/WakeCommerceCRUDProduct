using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeCommerceCRUDProduct.Application.DTOs
{
    public record ProductDTO
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal Value { get; set; }
    }
}
