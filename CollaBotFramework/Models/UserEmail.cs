namespace CollaBotFramework.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserEmail")]
    public partial class UserEmail
    {
        [Key]
        [StringLength(100)]
        public string Login { get; set; }

        [StringLength(100)]
        public string Email { get; set; }
    }
}
