
namespace FramboyanSchedulerApi.Models
{
    public class ClassModel
    {
        public int Id { get; set; } // Class ID (primary key)
        public required string Name { get; set; } // Class name
        public DateTime Time { get; set; } // Class time
    }
}