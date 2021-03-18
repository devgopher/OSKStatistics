using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansStatisticsKeeper.Client.GrainsContext
{
    public class GenericGrainsContext : IOskRemoteExecutionContext
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>(20);
        public object GetValue(string name)
            => _values[name];

        public T GetValue<T>(string name) where T : class
            => _values[name] as T;

        public void SetValue(string name, object val)
            => _values[name] = val;
    }
}
