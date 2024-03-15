using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeCommerceCRUDProduct.Application.DTOs
{
    public record ProductDTO
    {
        public string Name { get; init; }
        public int Stock { get; init; }
        public decimal Value { get; init; }
    }
}
