using System;
using OrleansStatisticsKeeper.Models.Attributes;

namespace OrleansStatisticsKeeper.Grains.Models
{
    public class DataChunk
    {
        public DataChunk()
        {
            Id = Guid.NewGuid();
           SetDateTime(DateTime.UtcNow);
        }
        
        [Indexed]
        public Guid Id { get; set; }
        [Indexed]
        public long DateTimeTicks { get; set; }

        public DateTime GetDateTime => new DateTime(DateTimeTicks);

        public void SetDateTime(DateTime dt) => DateTimeTicks = dt.Ticks;
    }
}
