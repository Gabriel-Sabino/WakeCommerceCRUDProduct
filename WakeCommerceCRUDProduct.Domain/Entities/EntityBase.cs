namespace WakeCommerceCRUDProduct.Domain.Entities
{
    public class EntityBase
    {
        public int Id { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public DateTime ModifiedAt { get; private set; } = DateTime.Now;

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
