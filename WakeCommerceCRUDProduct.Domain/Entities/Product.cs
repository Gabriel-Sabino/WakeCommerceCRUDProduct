using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeCommerceCRUDProduct.Domain.Entities
{
    public class Product : EntityBase
    {
        public string Name { get; private set; }
        public int Stock { get; private set; }
        public decimal Value { get; private set; }

        public Product(string name, int stock, decimal value)
        {
            Name = name;
            Stock = stock;
            Value = value;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Nome do produto não pode ser vazio.");
            }
            Name = name;
        }

        public void UpdateStock(int stock)
        {
            if (stock < 0)
            {
                throw new ArgumentException("Estoque do produto não pode ser negativo.");
            }
            Stock = stock;
        }

        public void UpdateValue(decimal value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Valor do produto não pode ser negativo.");
            }
            Value = value;
        }
    }
}
