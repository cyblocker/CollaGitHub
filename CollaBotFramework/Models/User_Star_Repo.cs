namespace CollaBotFramework.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User_Star_Repo
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string UserLogin { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RepoID { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime TimeStamp { get; set; }
    }
}
