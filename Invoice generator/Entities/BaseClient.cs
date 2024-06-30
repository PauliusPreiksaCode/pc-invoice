using Invoice_generator.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoice_generator.Entities
{
    public class BaseClient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Address { get; set; }

        public ClientType ClientType { get; set; }

        public required string CountryCode {  get; set; }
    }
}
