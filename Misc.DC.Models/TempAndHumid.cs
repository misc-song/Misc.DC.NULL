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
        public decimal value { get; set; }                      //值
        public string name { get; set; }                        //名称
        public DateTime addDateTime { get; set; }               //添加时间

    }
}
