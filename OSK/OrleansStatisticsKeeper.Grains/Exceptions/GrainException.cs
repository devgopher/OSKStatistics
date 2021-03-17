using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansStatisticsKeeper.Grains.Exceptions
{
    public class GrainException : Exception
    {
        public GrainException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}
