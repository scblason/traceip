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
         * We initialize every unit test with a set of data to populate the RedisDB
         * NOTE: For simplicity, we use the RedisDB as a store instance to perform the tests, but the databases
         * could (and should) be mocked, implementing the IStatsRepositorie and ITraceReportRepositorie.
         * Data used in the tests (from test_data.txt file):
            Brazil: 18.229.255.255, 2824kms
            Japan: 1.79.255.255, 18018km
            Uruguay: 186.55.255.255, 750kms, Nearest
            China: 14.127.255.255, 19017kms, Farest
            UK: 3.11.255.255, 11454kms
         */
        [TestInitialize]
        [DeploymentItem("@test_data.txt")]
        public async Task InitServiceAsync()
        {
            _redis = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            _redis.GetServer(_redis.GetEndPoints()[0]).FlushAllDatabases();
            _service = new TraceIpService(new TraceReportRepositorie(_redis), new StatsRepositorie(_redis));

            string testData = System.IO.File.ReadAllText(@"test_data.txt");
            var IPs = testData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (var ip in IPs)
            {
                TraceIpReport report = await _service.GetTraceReport(ip);
                Assert.IsNotNull(report);
            }
        }

        /**
         * Testing nearest country service.
         */
        [TestMethod]
        public async Task TestNearest()
        {
            TraceIpReport nearest = _service.GetReportByNearestCountry();
            Assert.IsNotNull(nearest);
            Assert.AreEqual(nearest.CountryCode, "UY");

            // We add Argentina (distance 0), but still nearest country is UY
            Assert.IsNotNull(await _service.GetTraceReport("24.232.255.255"));

            nearest = _service.GetReportByNearestCountry();
            Assert.IsNotNull(nearest);
            Assert.AreEqual(nearest.CountryCode, "UY");
        }

        /**
         * Testing farest country service.
         */
        [TestMethod]
        public void TestFarest()
        {
            TraceIpReport farest = _service.GetReportByFarestCountry();
            Assert.AreEqual(farest.CountryCode, "CN");
        }

        /**      
         * Testing average distance country. We add more hits to some countries to perform the test
         * and see different results.       
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

            // And to Uruguay, one more hit too...
            //( (2824 * 2) + (18018) + (750 * 2) + (19017 * 2) + (11454) ) / 8 = 10557kms
            Assert.IsNotNull(await _service.GetTraceReport("186.55.255.255"));
            avgDistance = _service.GetAverageDistance();
            Assert.AreEqual(avgDistance.Value, 9331);
        }
    }
}
