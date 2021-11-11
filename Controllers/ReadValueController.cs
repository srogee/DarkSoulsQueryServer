using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DarkSoulsMemoryReader;
using System.Linq;

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
                foreach (var valueId in ParseQuery(valuesToInspect)) {
                    if (DarkSouls3.KnownMemoryValues.TryGetValue(valueId, out MemoryValue value)) {
                        results.Add(new ReadValue(valueId, value.ReadValue(reader)));
                    }
                }
            }

            reader.Detach();

            return results;
        }

        // Allow consumers to do simple pattern matching, like "Bosses.*" or "*"
        private IEnumerable<string> ParseQuery(IEnumerable<string> valuesToInspect) {
            return valuesToInspect.SelectMany(value => {
                if (value.EndsWith("*")) {
                    var prefix = value.Substring(0, value.Length - 1);
                    return DarkSouls3.KnownMemoryValues.Keys.Where(key => key.StartsWith(prefix)).ToArray();
                } else {
                    return new string[] { value };
                }
            }).Distinct();
        }
    }
}
