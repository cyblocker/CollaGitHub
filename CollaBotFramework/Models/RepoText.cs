namespace CollaBotFramework.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RepoText")]
    public partial class RepoText
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RepoID { get; set; }

        public string Description { get; set; }

        [Required]
        public string RepoName { get; set; }
    }
}
