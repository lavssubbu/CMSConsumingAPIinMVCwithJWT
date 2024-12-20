using System.ComponentModel.DataAnnotations;

namespace CMSMVCConsumingAPI.Models
{
    public class Book
    {
        [Key]
        public int? BookId { get; set; }

        [MaxLength(100)]
        public string? BookName { get; set; }
        [MaxLength(40)]
        public string? AuthName { get; set; }
        [Range(500,5000,ErrorMessage ="Price should range from  500 to 5000")]
        public decimal Price { get; set; }

      //  [Range(1999,2024,ErrorMessage ="Year must be between 1999 and 2024")]
        public DateTime PublishedYear { get; set; }

        //Navigation Property
        public ICollection<BookAuthor>? bookAuthors { get; set; }
    }
}
