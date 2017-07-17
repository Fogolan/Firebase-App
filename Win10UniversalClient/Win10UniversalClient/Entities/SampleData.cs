using Newtonsoft.Json;
using System.Collections.Generic;

namespace Win10UniversalClient.Entities
{
    public class DataSample
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
    }

    public class RootObject
    {
        public DataSample[] dataSample { get; set; }
    }
}
