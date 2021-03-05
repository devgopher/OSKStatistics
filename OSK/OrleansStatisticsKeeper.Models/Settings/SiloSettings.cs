﻿using System.Collections.Generic;

namespace OrleansStatisticsKeeper.Models.Settings
{
    public class SiloSettings
    {
        public int SiloPort { get; set; }
        public ICollection<string> SiloAddresses { get; set; }

        public List<string> ModelsAssemblies { get; set; }

        public int InitializeAttemptsBeforeFailing { get; set; } = 3;
        public int MaxCpuLoad { get; set; } = 100;
    }
}
