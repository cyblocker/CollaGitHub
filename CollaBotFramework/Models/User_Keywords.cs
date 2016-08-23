namespace CollaBotFramework.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User_Keywords
    {
        [Key]
        [StringLength(100)]
        public string UserLogin { get; set; }

        public string KeywordsList { get; set; }
    }
}