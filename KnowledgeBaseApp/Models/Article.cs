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
        public string Content { get; set; } // Тук ще пазим HTML от редактора

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public int ViewCount { get; set; } = 0;

        // Връзка с Категория
        [Required]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        // Връзка с Автор (Потребител)
        // Използваме IdentityUser, който идва с ASP.NET Core
        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual IdentityUser? Author { get; set; }

        // Връзка с Рейтинги
        public virtual ICollection<Vote>? Votes { get; set; }
    }
}
