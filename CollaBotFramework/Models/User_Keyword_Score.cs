using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollaBotFramework.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User_Keyword_Score
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string UserLogin { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string Keyword { get; set; }

        public double? Score { get; set; }
    }
}