using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetWebApi.Helpers
{
    public class CachedSettings
    {
        public bool Enabled { get; set; }

        public string ConnectionString { get; set; }
    }
}