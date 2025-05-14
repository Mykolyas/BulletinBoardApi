namespace BulletinBoardApi.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; } // true = Active, false = Inactive
        public string Category { get; set; } = null!;
        public string SubCategory { get; set; } = null!;
    }
}
