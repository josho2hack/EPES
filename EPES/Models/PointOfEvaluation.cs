using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPES.Models
{
    public class PointOfEvaluation
    {
        public int Id { get; set; }

        [DisplayName("ตัวชี้วัด")]
        public string Name { get; set; }

        [DisplayName("หน่วยวัด")]
        public string Unit { get; set; }

        [DisplayName("น้ำหนักสำหรับ กอง สำนัก ศูนย์")]
        [Column(TypeName = "decimal(7, 4)")]
        public decimal WeightForHQ { get; set; }

        [DisplayName("น้ำหนักสำหรับ สภ.")]
        [Column(TypeName = "decimal(7, 4)")]
        public decimal WeightForSP { get; set; }

        [DisplayName("น้ำหนักสำหรับ สท.")]
        [Column(TypeName = "decimal(7, 4)")]
        public decimal WeightForST { get; set; }

        [DisplayName("กำหนดค่าที่ได้คะแนน 1")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Rate1 { get; set; }

        [DisplayName("กำหนดค่าที่ได้คะแนน 2")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Rate2 { get; set; }

        [DisplayName("กำหนดค่าที่ได้คะแนน 3")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Rate3 { get; set; }

        [DisplayName("กำหนดค่าที่ได้คะแนน 4")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Rate4 { get; set; }

        [DisplayName("กำหนดค่าที่ได้คะแนน 5")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Rate5 { get; set; }

        [DisplayName("หน่วยงานควบคุมตัวชี้วัด")]
        public Office AuditOffice { get; set; }
    }
}
