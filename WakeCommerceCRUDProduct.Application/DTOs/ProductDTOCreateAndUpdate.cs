namespace WakeCommerceCRUDProduct.Application.DTOs
{
    public record ProductDTOCreateAndUpdate
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public string Value { get; set; }
    }
}
