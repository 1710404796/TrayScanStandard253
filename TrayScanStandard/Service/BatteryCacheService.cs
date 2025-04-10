using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using TrayScanStandard.Models;

namespace TrayScanStandard.Service
{
    public record BatteryLevelRecord(BatteryLevel BatteryLevel, DateTime Time);
    public class BatteryCacheService
    {
        //Map<string, BatteryLevelRecord> _batteryLevels = new();

        ConcurrentDictionary<string, BatteryLevelRecord> _batteryLevels = new();

        public void AddBatteryLevel(string batteryId, BatteryLevel batteryLevel)
        {
            _batteryLevels.AddOrUpdate(batteryId, new BatteryLevelRecord(batteryLevel, DateTime.Now), (key, oldValue) => new BatteryLevelRecord(batteryLevel, DateTime.Now));
        }

        public Option<BatteryLevelRecord> GetBatteryLevel(string batteryId)
        {
            return (_batteryLevels as IDictionary<string, BatteryLevelRecord>).TryGetValue(batteryId);
        }

        public void RemoveBatteryLevel(string batteryId)
        {
            _batteryLevels.TryRemove(batteryId, out _);
        }

    }
}
