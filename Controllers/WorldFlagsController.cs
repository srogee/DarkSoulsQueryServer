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
                var flagsToInspect = new string[] {
                    "Bosses.IudexGundyr.Defeated",
                    "Bosses.AbyssWatchers.Defeated",
                    "Bosses.YhormTheGiant.Defeated",
                    "Bosses.Aldrich.Defeated",
                    "Bosses.TwinPrinces.Defeated",
                    "Bosses.SoulOfCinder.Defeated"
                };

                // Actually get the values
                foreach (var flagId in flagsToInspect) {
                    var address = DS3MemoryAddress.KnownAddresses[flagId];
                    var flag = new DS3MemoryValueBoolFlag(processInfo, address, (int)address.ExtraInfo, DS3AddressUpdateType.Automatic);
                    results.Add(new WorldFlag(flagId, flag.GetValueGeneric()));
                }
            }

            return results;
        }
    }
}
