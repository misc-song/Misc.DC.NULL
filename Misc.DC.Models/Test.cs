using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc.DC.Models
{
    public class TempAndHumid
    {
        [Key]
        public int id { get; set; }                             //id
        public decimal temperature { get; set; }                //温度
        public decimal humidity { get; set; }                   //湿度
        public DateTime addDateTime { get; set; }               //添加时间

    }
}
