using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class SearchKeyWord
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof (CV))]
        public int cvId { get; set; }
        public CV CV { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
