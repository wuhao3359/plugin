using ImGuiNET;
using System;
using System.Numerics;

namespace AlphaProject
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Configuration configuration;

        //private ImGuiScene.TextureWrap goatImage;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;

        public int selectedButton = 1;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get { return this.settingsVisible; }
            set { this.settingsVisible = value; }
        }

        // passing in the image here just for simplicity
        public PluginUI()
        {
            //this.configuration = configuration;
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            visible = true;
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            DrawMainWindow();
            //DrawSettingsWindow();
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(200, 200), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 200), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("Operation Window", ref this.visible))
            {
                //ImGui.Text($"The random config bool is {this.configuration.SomePropertyToBeSavedAndWithADefault}");
                if (ImGui.Button("GetCurrentPosition"))
                {
                    Tasks.GetCurrentPosition();
                }
                ImGui.Spacing();

                if (ImGui.Button("FishInIsland Start"))
                {
                    String args = "100";
                    Tasks.FishInIsland(args);
                }
                ImGui.SameLine();
                if (ImGui.Button("FishInIsland End"))
                {
                    Tasks.FishInIsland("");
                }

                if (ImGui.Button("FishOnSea Start"))
                {
                    String args = "1";
                    Tasks.FishOnSea(args);
                }
                ImGui.SameLine();

                if (ImGui.Button("FishOnSea End"))
                {
                    Tasks.FishOnSea("");
                }

                if (ImGui.Button("AutoDaily Start"))
                {
                    String args = "duration:3 level:90-90 bagLimit:1 otherTask:5";
                    Tasks.AutoDaily(args);
                }
                ImGui.SameLine();
                if (ImGui.Button("AutoDaily End"))
                {
                    Tasks.AutoDaily("");
                }

                if (ImGui.Button("CollectibleFish Start"))
                {
                    String args = "ftype:1 fexchangeItem:102";
                    Tasks.CollectibleFish(args);
                }
                ImGui.SameLine();
                if (ImGui.Button("CollectibleFish End"))
                {
                    Tasks.CollectibleFish("");
                }
                ImGui.Spacing();

                // 新UI测试
                float windowHeight = ImGui.GetWindowHeight();
                float topHeight = windowHeight * 0.2f;
                ImGui.BeginChild("TopRegion", new Vector2(0, topHeight), true);
                if (ImGui.Button("Button 1"))
                {
                    // Button 1 被点击
                    // 在这里处理 Button 1 点击事件
                    selectedButton = 1;
                }
                ImGui.SameLine();
                if (ImGui.Button("ygather"))
                {
                    selectedButton = 2;
                }
                ImGui.EndChild();

                ImGui.BeginChild("BottomRegion", new Vector2(0, windowHeight * 0.5f), true);
                string input1 = "input1";
                string ygather = "1";
                if (selectedButton == 1)
                {
                    ImGui.Text("Content for Button 1");
                    // 添加 Button 1 对应的下面区域内容
                    ImGui.InputText("Input 1", ref input1, 100); // 假设 input1 是存储输入框内容的变量
                    if (ImGui.Button("Submit 1"))
                    {
                        // 处理 Button 1 对应的 Submit 1 按钮点击事件
                    }
                }
                else if (selectedButton == 2)
                {
                    ImGui.Text("ygather");
                    ImGui.InputText("ygather", ref ygather, 100);
                    if (ImGui.Button("run"))
                    {
                        Tasks.GatherInisland(ygather);
                    }
                }
                ImGui.EndChild();
            }
            ImGui.End();
        }

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(232, 75), ImGuiCond.Always);
            if (ImGui.Begin("A Wonderful Configuration Window", ref this.settingsVisible,
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                // can't ref a property, so use a local copy
                var configValue = this.configuration.SomePropertyToBeSavedAndWithADefault;
                if (ImGui.Checkbox("Random Config Bool", ref configValue))
                {
                    this.configuration.SomePropertyToBeSavedAndWithADefault = configValue;
                    // can save immediately on change, if you don't want to provide a "Save and Close" button
                    this.configuration.Save();
                }
            }
            ImGui.End();
        }
    }
}
