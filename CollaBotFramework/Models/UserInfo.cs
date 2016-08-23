namespace CollaBotFramework.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserInfo")]
    public partial class UserInfo
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(50)]
        public string channel { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string userID { get; set; }

        [Required]
        [StringLength(50)]
        public string GitHubLogin { get; set; }

        [StringLength(100)]
        public string Email { get; set; }
    }
}
