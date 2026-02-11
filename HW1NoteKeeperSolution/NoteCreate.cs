using System.ComponentModel.DataAnnotations;

namespace HW1NoteKeeperSolution
{
    public class NoteCreate
    {
        //public DateOnly Date { get; set; }

        //public int TemperatureC { get; set; }

        //public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        //public string? Summary { get; set; }

        [Required]
        [MinLength(length: 1)]
        [MaxLength(length: 60)]
        public string? Summary { get; set; } = null!;

        [Required]
        [MinLength(length: 1)]
        [MaxLength(length: 1024)]
        public string? Details { get; set; } = null!;

        //public List<string> Tags { get; set; } = null!;

    }
}
