namespace HW1NoteKeeperSolution
{
    public class Note
    {
        public string NoteId { get; set; }

        public string Summary { get; set; }
        public string Details { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public DateTime? ModifiedDateUtc { get; set; }
        public string[] Tags { get; set; }
    }
}