using SQLite;

namespace BeePassKeeper.Models
{
    [Table("Passwords")]
    public class PasswordModel
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [MaxLength(256)]
        public string Password { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(99)]
        public string Description { get; set; }

        [MaxLength(99)]
        public string DateAdded { get; set; }

        [MaxLength(50)]
        public string EncryptionKey { get; set; }
    }
}