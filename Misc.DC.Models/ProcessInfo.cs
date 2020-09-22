using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc.DC.Models
{
    public class ProcessInfo
    {
        [Key]
        public int id { get; set; }                        //表id 自增
        public int processId { get; set; }              //进程id
        public string processName { get; set; }            //进程名称
        public string comName { get; set; }                 //串口名称

    }
}
