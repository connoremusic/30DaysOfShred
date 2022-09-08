namespace _30DaysOfShred.Models
{
    public class GuitarTab
    {
        public Guid uniqueId { get; set; }
        public string Title { get; set; }
        [System.ComponentModel.DisplayName("Tags")]
        public List<string> TabCategories { get; set; }

        public GuitarTab(Guid uniqueId, string title, List<string> tabCategories)
        {
            this.uniqueId = uniqueId;
            Title = title;
            TabCategories = tabCategories;
        }

        public GuitarTab()
        {
            this.uniqueId = Guid.Empty;
            this.Title = "";
            this.TabCategories = new List<string>();
        }
    }
}
