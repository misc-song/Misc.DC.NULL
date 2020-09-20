using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Misc.DC.api.Models;
using Misc.DC.api.Utils;
using Misc.DC.Storage;

namespace Misc.DC.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadHistoryDataController : ControllerBase
    {
        private DcDbContext _dcDbContext { get; set; }
        public LoadHistoryDataController(DcDbContext dcDbContext)
        {
            _dcDbContext = dcDbContext;
        }

        [HttpGet("LoadData")]
        public IActionResult LoadData([FromQuery] string date, [FromQuery] string startTime, [FromQuery] string endTime)
        {
            //    var date = "2020-09-20";
            if (string.IsNullOrEmpty(date))
            {
                return new JsonResult(new { returnCode = ReturnCode.ServerError });
            }
            DateTime dateTime = DateTime.Parse(date);
            DateTime start = DateTime.Parse(date + " " + startTime + ":00:00");
            DateTime end = DateTime.Parse(date + " " + endTime + ":00:00");

            var tres = _dcDbContext.tempAndHumids.Where(u => u.addDateTime > start && u.addDateTime < end).OrderBy(u => u.id);
            var res = tres.DistinctBy(u=>u.addDateTime);
            return new JsonResult(new { serverData = "true", returnCode = ReturnCode.ServerOK, res });
        }

  
    }
}
