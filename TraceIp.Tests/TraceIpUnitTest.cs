using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using TraceIp.Model;
using TraceIpWebApi.Repositories;
using TraceIpWebApi.Service;

namespace TraceIp.Tests
{
    [TestClass]
    public class TraceIpUnitTest
    {
        IConnectionMultiplexer _redis;
        ITraceIpService _service;

        /**      
            Brazil: 18.229.255.255, 2824kms
            Japan: 1.79.255.255, 18018km
            Uruguay: 186.55.255.255, 750kms, Nearest
            China: 14.127.255.255, 19017kms, Farest
            UK: 3.11.255.255, 11454kms
         */
        [ClassInitialize]
        [DeploymentItem("@test_data.txt")]
        public static async Task InitDb(TestContext context)
        {
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            redis.GetServer(redis.GetEndPoints()[0]).FlushAllDatabases();
            ITraceIpService service = new TraceIpService(new TraceReportRepositorie(redis), new StatsRepositorie(redis));

            string testData = System.IO.File.ReadAllText(@"test_data.txt");
            var IPs = testData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (var ip in IPs)
            {
                TraceIpReport report = await service.GetTraceReport(ip);
                Assert.IsNotNull(report);
            }
        }

        [TestInitialize]
        public void InitService()
        {
            _redis = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            _service = new TraceIpService(new TraceReportRepositorie(_redis), new StatsRepositorie(_redis));
        }

        [TestMethod]
        public void TestNearest()
        {
            TraceIpReport nearest = _service.GetReportByNearestCountry();
            Assert.AreEqual(nearest.CountryCode, "UY");
        }

        [TestMethod]
        public void TestFarest()
        {
            TraceIpReport farest = _service.GetReportByFarestCountry();
            Assert.AreEqual(farest.CountryCode, "CN");
        }

        /**      
           Brazil: 18.229.255.255, 2824kms, 2 hits
           Japan: 1.79.255.255, 18018km
           Uruguay: 186.55.255.255, 750kms
           China: 14.127.255.255, 19017kms, 2 hits
           UK: 3.11.255.255, 11454kms
        */
        [TestMethod]
        public async Task TestAverage()
        {
            // Add one more hit to Brazil and China
            Assert.IsNotNull(await _service.GetTraceReport("18.229.255.255"));
            Assert.IsNotNull(await _service.GetTraceReport("14.127.255.255"));

            //( (2824 * 2) + (18018) + (750) + (19017 * 2) + (11454) ) / 7 = 10557kms

            long? avgDistance = _service.GetAverageDistance();
            Assert.AreEqual(avgDistance.Value, 10557);
        }
    }
}
