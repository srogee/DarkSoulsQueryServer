using System;

namespace DarkSoulsQueryServer
{
    public class WorldFlag
    {
        public string Id { get; set; }
        public bool Value { get; set; }
        public WorldFlag(string id, bool value) {
            Id = id;
            Value = value;
        }
    }
}
