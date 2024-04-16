using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeePassKeeper.Database.Models
{
    [Table("EncryptionKey")]

    class EncryptionKeyModel
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Key { get; set; }

        [MaxLength(50)] 
        public string Password { get; set; }
    }
}
