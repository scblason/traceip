using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TraceIp;
using TraceIp.Api;
using TraceIp.Builders;
using TraceIp.Model;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using TraceIpWebApi.Repositories;
using TraceIpWebApi.Service;

namespace TraceIpWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraceIpController : ControllerBase
    {
        private readonly ITraceIpService _traceIpservice;

        public TraceIpController(ITraceIpService traceIpservice)
        {
            this._traceIpservice = traceIpservice;
        }

        // GET api/traceip?{ip}
        [HttpGet]
        public async Task<ActionResult> TraceIp([FromQuery]string ip)
        {
            //string ip = "23.17.255.255"; // Canada
            //string ip = "2.155.255.255"; // Spain
            try
            {
                if (String.IsNullOrEmpty(ip))
                {
                    return BadRequest("Missing IP.");
                }

                TraceIpReport report = await _traceIpservice.GetTraceReport(ip);
                if (report == null)
                {
                    return BadRequest("Invalid or missing IP address.");
                }

                return Ok(report.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        // GET api/traceip/stats/nearest
        [HttpGet("stats/nearest")]
        public ActionResult GetNearestStat()
        {
            TraceIpReport report = _traceIpservice.GetReportByNearestCountry();
            if (report != null)
            {
                return Ok(report.ToString());
            }
            else
            {
                return Ok("Theres no data in the TraceIp Report database. Call /api/traceip?... to generate a new report.");
            }
        }

        // GET api/traceip/stats/farest
        [HttpGet("stats/farest")]
        public ActionResult GetFarestStat()
        {
            TraceIpReport report = _traceIpservice.GetReportByFarestCountry();
            if (report != null)
            {
                return Ok(report.ToString());
            }
            else
            {
                return Ok("Theres no data in the TraceIp Report database. Call /api/traceip?... to generate a new report.");
            }
        }

        // GET api/traceip/stats/average
        [HttpGet("stats/average")]
        public ActionResult GetAverageStat()
        {
            long? distance = _traceIpservice.GetAverageDistance();
            if (distance != null)
            {
                return Ok(String.Format("Distancia promedio de todas las ejecuciones: {0}", distance));
            }
            else
            {
                return Ok("Theres no data in the TraceIp Report database. Call /api/traceip?... to generate a new report.");
            }
        }
    }
}
