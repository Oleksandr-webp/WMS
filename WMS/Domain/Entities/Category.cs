namespace WMS.Domain.Entities
{
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool isActive { get; set; }
    }
}
