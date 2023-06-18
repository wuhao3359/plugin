using System;
using System.Linq;
using AlphaProject.Utility;
using System.Threading;

namespace AlphaProject.Helper
{
    public unsafe static class CommonHelper
    {

        public unsafe static void test() { 
        
        }

        public unsafe static bool SetTarget(string targetName)
        {
            var target = DalamudApi.ObjectTable.FirstOrDefault(obj => obj.Name.TextValue.ToLowerInvariant() == targetName);
            if (target == default)
            {
                return false;
            }

            DalamudApi.TargetManager.SetTarget(target);
            Thread.Sleep(200 + new Random().Next(1000, 2000));
            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(1000 + new Random().Next(500, 1000));
            return true;
        }
    }
}
