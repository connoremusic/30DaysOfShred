namespace _30DaysOfShred.Models
{
    public class GuitarTab
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<string> TabCategories { get; set; }

        public GuitarTab(int id, string title, List<string> tabCategories)
        {
            Id = id;
            Title = title;
            TabCategories = tabCategories;
        }
    }
}
