using System;

namespace DarkSoulsQueryServer
{
    public class ReadValue
    {
        public string Id { get; set; }
        public dynamic Value { get; set; }
        public ReadValue(string id, dynamic value) {
            Id = id;
            Value = value;
        }
    }
}
