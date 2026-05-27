namespace Satur8.Persistence.Misc
{
    public class PresetDto
    {
        public Guid PresetId { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string AuthorLogin { get; set; } = "";
        public bool IsFavourite { get; set; }
        public PresetParameters Parameters { get; set; } = new();
    }
}
