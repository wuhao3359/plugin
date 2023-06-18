using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace AlphaProject
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        // 制作类型 1-根据名称普通制作 2-根据名称快速制作
        public int CraftType { get; set; } = 1;

        public bool AutoGather { get; set; } = false;

        public bool AutoMarket{ get; set; } = false;


        public int MaxPercentage { get; set; } = 100;
        public string GatherName { get; set; } = "";


        public string RecipeName { get; set; } = "";

        public string ExchangeItem { get; set; } = "";

        public bool QuickSynth { get; set; } = false;

        // 任务状态 1-进行任务 2-暂停任务
        public int state { get; set; } = 1;

        public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize() { }

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}
