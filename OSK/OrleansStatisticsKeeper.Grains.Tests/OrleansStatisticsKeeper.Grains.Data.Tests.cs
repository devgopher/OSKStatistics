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

namespace OrleansStatisticsKeeper.Grains.Tests
{
    public class Tests
    {
        private IAddStatisticsGrain<TestModel> _addStatisticsGrain;
        private IGetStatisticsGrain<TestModel> _getStatisticsGrain;
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
            _addStatisticsGrain = new MongoAddStatisticsGrain<TestModel>(_mongoUtils);
            _getStatisticsGrain = new MongoGetStatisticsGrain<TestModel>(_mongoUtils);

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
    }
}