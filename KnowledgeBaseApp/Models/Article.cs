using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnowledgeBaseApp.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Заглавието е задължително")]
        [MaxLength(100)]
        [Display(Name = "Заглавие")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Съдържание")]
        public string Content { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public int ViewCount { get; set; } = 0;

        [Required]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual IdentityUser? Author { get; set; }

        public virtual ICollection<Vote>? Votes { get; set; }
    }
}
