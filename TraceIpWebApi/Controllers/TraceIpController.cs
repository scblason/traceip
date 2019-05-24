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
        public async Task<ActionResult> GetNearestStat()
        {
            TraceIpReport report = _traceIpservice.GetReportByNearestCountry();
            return Ok(report);
        }

        [HttpGet("stats/farest")]
        public async Task<ActionResult> GetFarestStat()
        {
            TraceIpReport report = _traceIpservice.GetReportByFarestCountry();
            return Ok(report);
        }

        [HttpGet("stats/average")]
        public async Task<ActionResult> GetAverageStat()
        {
            return Ok();
        }
    }
}
