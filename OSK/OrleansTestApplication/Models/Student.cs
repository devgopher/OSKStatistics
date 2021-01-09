using System;
using System.Collections.Generic;
using System.Text;
using OrleansStatisticsKeeper.Grains.Models;

namespace OrleansTestApplication.Models
{
    [Serializable]
    public class Student : DataChunk
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
