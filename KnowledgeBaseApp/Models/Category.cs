using System.ComponentModel.DataAnnotations;

namespace KnowledgeBaseApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Име на категория")]
        public string Name { get; set; }

        [MaxLength(200)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        // Връзка: Една категория има много статии
        public virtual ICollection<Article>? Articles { get; set; }
    }
}
