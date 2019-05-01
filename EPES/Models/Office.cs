using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPES.Models
{
    public class Office
    {
        public int Id { get; set; }

        [Display(Name="รหัสหน่วยงาน")]
        [Column(TypeName = "nvarchar(8)")]
        public string Code { get; set; }

        [Display(Name = "หน่วยงาน")]
        public string Name { get; set; }

        [DisplayName("หมายเหตุ")]
        public string Remark { get; set; }
    }
}
