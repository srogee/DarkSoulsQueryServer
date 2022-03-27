using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DarkSoulsMemoryReader;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using System.IO;
using System;

namespace DarkSoulsQueryServer
{
    public struct SpoilerLog
    {
        public string Path;
        public DateTime? DateTime;
    }

    public class SpoilerLogRequest
    {
        public string RandomizerPath { get; set; }
    }

    public class SpoilerLogResponse
    {
        public bool IsValid { get; set; }
        public DateTime DateTime { get; set; } // UTC
        public IEnumerable<string> Content { get; set; }
    }
}

namespace DarkSoulsQueryServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetLatestSpoilerLogController : ControllerBase
    {
        private readonly ILogger<GetLatestSpoilerLogController> _logger;

        public GetLatestSpoilerLogController(ILogger<GetLatestSpoilerLogController> logger) {
            _logger = logger;
        }

        [HttpPost]
        public SpoilerLogResponse Get(SpoilerLogRequest request) {
            string dir = Path.GetDirectoryName(request?.RandomizerPath);
            if (!string.IsNullOrWhiteSpace(dir)) {
                string spoilerLogDir = Path.Combine(dir, "spoiler_logs");

                if (Directory.Exists(spoilerLogDir)) {
                    var spoilerLogs = GetSortedSpoilerLogs(spoilerLogDir);

                    if (spoilerLogs.Count() > 0) {
                        var mostRecentSpoilerLog = spoilerLogs.First();
                        return new SpoilerLogResponse() {
                            IsValid = true,
                            DateTime = (DateTime)mostRecentSpoilerLog.DateTime,
                            Content = GetSpoilerLogContents(mostRecentSpoilerLog)
                        };
                    }
                }
            }

            return new SpoilerLogResponse() {
                IsValid = false
            };
        }

        private static IEnumerable<string> GetSpoilerLogContents(SpoilerLog mostRecentSpoilerLog) {
            return System.IO.File.ReadAllLines(mostRecentSpoilerLog.Path, System.Text.Encoding.UTF8)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .TakeWhile(line => !line.StartsWith("Finished ")); // Stop at this line because everything after it is irrelevant
        }

        private static IOrderedEnumerable<SpoilerLog> GetSortedSpoilerLogs(string spoilerLogDir) {
            var files = Directory.GetFiles(spoilerLogDir, "*.txt", SearchOption.TopDirectoryOnly);
            var spoilerLogs = files
                .Select(file => new SpoilerLog() { Path = file, DateTime = ParseSpoilerLogDate(file) })
                .Where(spoilerLog => spoilerLog.DateTime != null)
                .OrderByDescending(spoilerLog => spoilerLog.DateTime);
            return spoilerLogs;
        }

        private static DateTime? ParseSpoilerLogDate(string spoilerLogPath) {
            var basename = Path.GetFileName(spoilerLogPath);
            try {
                var split = basename.Split('_');
                var dateComponents = split[0].Split('-');
                var timeComponents = split[1].Split('.');
                var year = int.Parse(dateComponents[0]);
                var month = int.Parse(dateComponents[1]);
                var day = int.Parse(dateComponents[2]);
                var hour = int.Parse(timeComponents[0]);
                var minute = int.Parse(timeComponents[1]);
                var second = int.Parse(timeComponents[2]);
                return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local).ToUniversalTime();
            } catch {
                return null;
            }
        }
    }
}
