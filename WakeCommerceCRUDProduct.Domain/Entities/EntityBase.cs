using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WakeCommerceCRUDProduct.Domain.Entities
{
    public class EntityBase
    {
        public int Id { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public DateTime ModifiedAt { get; private set; } = DateTime.Now;

        //public EntityBase(int id) {
        //Id = id;
        //}

        public void ReceiveId(int id)
        {
            Id = id;
        }

        public void UpdateModifiedAt()
        {
            ModifiedAt = DateTime.Now;
        }
    }


}
