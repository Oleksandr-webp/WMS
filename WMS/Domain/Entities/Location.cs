namespace WMS.Domain.Entities
{
    public class Location
    {
        public long Id { get; set; }
        public long WarehouseId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
