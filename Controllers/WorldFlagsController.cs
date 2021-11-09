using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DarkSoulsMemoryReader;

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
            var reader = new ProcessMemoryReader("DarkSoulsIII");
            reader.Attach();

            if (reader.IsAttached) {
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
                    var flag = DarkSouls3.KnownMemoryValues[flagId];
                    results.Add(new WorldFlag(flagId, (bool)flag.ReadValue(reader)));
                }
            }

            reader.Detach();

            return results;
        }
    }
}
