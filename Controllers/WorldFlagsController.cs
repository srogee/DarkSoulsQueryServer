using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DS3MemoryReader;

namespace DarkSoulsQueryServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorldFlagsController : ControllerBase
    {
        // Base addresses for lots of information in DS3
        const int gameFlagData = 0x473BE28;

        private readonly ILogger<WorldFlagsController> _logger;

        public WorldFlagsController(ILogger<WorldFlagsController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WorldFlag> Get() {
            return ReadFlagValues();
        }

        private List<WorldFlag> ReadFlagValues() {
            var results = new List<WorldFlag>();

            // Will automatically find the process for us
            var processInfo = new DS3ProcessInfo();
            processInfo.FindDS3Process();

            if (processInfo.IsValid) {
                // Define values we want to inspect
                var flagsToInspect = new Dictionary<string, DS3MemoryValueBoolFlag>() {
                    ["Bosses.IudexGundyr.Defeated"] = new DS3MemoryValueBoolFlag(processInfo, new DS3MemoryAddress(gameFlagData, 0, 0x5A67), 7),
                    ["Bosses.AbyssWatchers.Defeated"] = new DS3MemoryValueBoolFlag(processInfo, new DS3MemoryAddress(gameFlagData, 0, 0x2D67), 7),
                    ["Bosses.YhormTheGiant.Defeated"] = new DS3MemoryValueBoolFlag(processInfo, new DS3MemoryAddress(gameFlagData, 0, 0x5567), 7),
                    ["Bosses.Aldrich.Defeated"] = new DS3MemoryValueBoolFlag(processInfo, new DS3MemoryAddress(gameFlagData, 0, 0x4B67), 7),
                    ["Bosses.TwinPrinces.Defeated"] = new DS3MemoryValueBoolFlag(processInfo, new DS3MemoryAddress(gameFlagData, 0, 0x3764), 1),
                    ["Bosses.SoulOfCinder.Defeated"] = new DS3MemoryValueBoolFlag(processInfo, new DS3MemoryAddress(gameFlagData, 0, 0x5F67), 7),
                };

                // Regenerate addresses for the values we want to get
                DS3MemoryValue.RegenerateAddresses(flagsToInspect.Values.ToArray());

                // Actually get the values
                foreach (var kvp in flagsToInspect) {
                    results.Add(new WorldFlag(kvp.Key, kvp.Value.GetValueGeneric()));
                }
            }

            return results;
        }
    }
}
