using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinxUniverse.CST;
using LinxUniverse.Utils;
using TrayScanStandard.Attritubes;
using TrayScanStandard.Models;

namespace TrayScanStandard
{
    public static class MainStorage
    {
        public static SaveManager<WcsSaves> SaveManager = new();
        public static WcsSaves Saves => SaveManager.SaveFile;

        public static bool IsWcsEnable { get; set; }
        public static int[] DefaultExp = [7500, 15000, 22500, 30000];
        public static PowerEnum[] PowerEnums => Enum.GetValues<PowerEnum>().ToArray();
        public static RoleEnum[] RoleEnums => Enum.GetValues<RoleEnum>().SkipLast(1).ToArray();

        public static IEnumerable<LightCST> CST { get; set; } = [];

        public static void Init()
        {
            SaveManager.Load();
        }
    }
}
