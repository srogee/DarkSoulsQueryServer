using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DarkSoulsMemoryReader;

namespace DarkSoulsQueryServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReadArbitraryValuesController : ControllerBase
    {
        private readonly ILogger<ReadArbitraryValuesController> _logger;

        public ReadArbitraryValuesController(ILogger<ReadArbitraryValuesController> logger) {
            _logger = logger;
        }

        [HttpPost]
        public IEnumerable<ReadValue> Get(IEnumerable<string> valuesToInspect) {
            return ReadValues(valuesToInspect);
        }

        private List<ReadValue> ReadValues(IEnumerable<string> valuesToInspect) {
            var results = new List<ReadValue>();

            // Will automatically find the process for us
            var reader = new ProcessMemoryReader("DarkSoulsIII");
            reader.Attach();

            if (reader.IsAttached) {
                // Actually get the values
                foreach (var valueId in valuesToInspect) {
                    if (DarkSouls3.KnownMemoryValues.TryGetValue(valueId, out MemoryValue value)) {
                        results.Add(new ReadValue(valueId, value.ReadValue(reader)));
                    }
                }
            }

            reader.Detach();

            return results;
        }
    }
}
