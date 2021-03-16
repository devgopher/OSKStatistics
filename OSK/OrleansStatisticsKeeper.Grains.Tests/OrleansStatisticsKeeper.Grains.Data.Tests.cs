using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OrleansStatisticsKeeper.Grains.Grains;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Tests.Models;
using OrleansStatisticsKeeper.Grains.Utils;
using System;
using System.Globalization;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Models.Settings;
using AsyncLogging;
using OrleansStatisticsKeeper.Grains.Tests.TestClasses;

namespace OrleansStatisticsKeeper.Grains.Tests
{
    public class Tests
    {
        private IManageStatisticsGrain<TestModel> _addStatisticsGrain;
        private IGetStatisticsGrain<TestModel> _getStatisticsGrain;
        private IExecutiveGrain _executiveGrain;
        private MongoUtils _mongoUtils;
        private OskSettings _oskSettings = new OskSettings();

        [SetUp]
        public async Task Setup()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            configuration.GetSection("OskSettings").Bind(_oskSettings);

            _mongoUtils = new MongoUtils(_oskSettings);
            _addStatisticsGrain = new MongoManageStatisticsGrain<TestModel>(_mongoUtils);
            _getStatisticsGrain = new MongoGetStatisticsGrain<TestModel>(_mongoUtils, new NLogLogger());
            _executiveGrain = new GenericExecutiveGrain();

            await FillData();
        }

        public async Task FillData()
        {
            if (!await _getStatisticsGrain.Any())
            {
                for (var i = 0; i < 1000000; ++i)
                    await _addStatisticsGrain.Put(new TestModel()
                    {
                        Text = $"TTTTT_{i}"
                    });
            }
        }

        [Test]
        public async Task GetStatisticsNotEmpty()
        {
           Assert.IsTrue(await _getStatisticsGrain.Any());
        }

        [Test]
        [TestCase("TEXT1")]
        public async Task PutStatistics(string text)
        {
            var insertText = text + DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            await _addStatisticsGrain.Put(new TestModel() {Text = insertText});
            Assert.IsTrue(await _getStatisticsGrain.Any(x => x.Text == insertText));
        }

        [Test]
        [TestCase("TEXT1")]
        public async Task ExecutiveGrainsTest(string text)
        {
            var insertText = text + DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            var ret = await _executiveGrain.Execute((t) => t, text);
            Assert.AreEqual(ret, text);
        }

        [Test]
        [TestCase(5, 3)]
        public async Task ExecutiveGrainsWithClassTest(int a, int b)
        {
            var tc = new TestExecutionContext();
            
            var ret = await _executiveGrain.Execute<int, int, double>((t1, t2) => tc.Test(t1, t2), a, b);
            Assert.AreEqual(ret, tc.Test(a,b));
        }
    }
}