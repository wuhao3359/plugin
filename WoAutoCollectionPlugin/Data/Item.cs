namespace WoAutoCollectionPlugin.Data
{
    public class Item
    {
        public int Id;
        public int MaxBackPack;
        public string Name;
        public uint Job;
        public string JobName;
        public uint Lv;
        public bool QuickCraft;
        public Item children;

        public Item(int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, bool QuickCraft, Item children)
        {
            this.Id = Id;
            this.MaxBackPack = MaxBackPack;
            this.Name = Name;
            this.Job = Job;
            this.JobName = JobName;
            this.Lv = Lv;
            this.QuickCraft = QuickCraft;
            this.children = children;
        }
    }
}