﻿using AlphaProject.Craft;
using AlphaProject.Data;
using Dalamud.Logging;
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
        public bool refresh = true;

        public int selectedButton = 1;

        int craftSelectedOption = 0;

        int ygatherSelectedOption = 0;
        string ygather = "1";

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
        public PluginUI(Configuration Configuration)
        {
            this.configuration = Configuration;
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
            DrawSettingsWindow();
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(250, 250), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 200), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("Operation Window", ref this.visible))
            {
                //ImGui.Text($"The random config bool is {this.configuration.SomePropertyToBeSavedAndWithADefault}");
                if (ImGui.Button("GetCurrentPosition"))
                {
                    Tasks.GetCurrentPosition();
                }
                ImGui.SameLine();
                if (ImGui.Button("Setting"))
                {
                    if (settingsVisible)
                    {
                        settingsVisible = false;
                    }
                    else {
                        settingsVisible = true;
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("Test"))
                {
                    PluginLog.Log($"configuration: {configuration.CraftType}");
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
                float topHeight = windowHeight * 0.15f;
                ImGui.BeginChild("TopRegion", new Vector2(0, topHeight), true);
                if (ImGui.Button("Button 1"))
                {
                    refresh = true;
                    selectedButton = 1;
                }
                ImGui.SameLine();
                if (ImGui.Button("ygather"))
                {
                    refresh = true;
                    selectedButton = 2;
                }
                ImGui.SameLine();
                if (ImGui.Button("craft"))
                {
                    refresh = true;
                    selectedButton = 3;
                }
                ImGui.EndChild();



                ImGui.BeginChild("BottomRegion", new Vector2(0, windowHeight * 0.5f), true);
                string input1 = "input1";
                if (selectedButton == 1)
                {
                    refresh = true;
                    ImGui.Text("Content for Button 1");
                    ImGui.InputText("Input 1", ref input1, 100); // 假设 input1 是存储输入框内容的变量
                    if (ImGui.Button("Submit 1"))
                    {
                        // 处理 Button 1 对应的 Submit 1 按钮点击事件
                    }
                }
                else if (selectedButton == 2)
                {
                    DrawYgatherOptionsWindow();
                }
                else if (selectedButton == 3)
                {
                    string recipeName = configuration.RecipeName;
                    ImGui.Text("RecipeName");
                    ImGui.SameLine();
                    if (ImGui.InputText("##recipeName", ref recipeName, 50))
                    {
                        configuration.RecipeName = recipeName;
                        configuration.Save();
                    }

                    string exchangeItem = configuration.ExchangeItem;
                    ImGui.Text("ExchangeItem");
                    ImGui.SameLine();
                    if (ImGui.InputText("##exchangeItem", ref exchangeItem, 50))
                    {
                        configuration.ExchangeItem = exchangeItem;
                        configuration.Save();
                    }

                    if (ImGui.Button("run"))
                    {
                        Tasks.GeneralCraft("start");
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("stop"))
                    {
                        Tasks.GeneralCraft("");
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("test"))
                    {
                        PluginLog.Log($"Current Step: {CurrentCraft.CurrentStep}");
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

            ImGui.SetNextWindowSize(new Vector2(250, 200), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 200), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("Settings", ref this.settingsVisible)) {
                string[] options = ImGuiSettings.CraftTypeOptions;
                float maxWidth = 0f;
                for (int i = 0; i < options.Length; i++)
                {
                    Vector2 textSize = ImGui.CalcTextSize(options[i]);
                    if (textSize.X > maxWidth)
                    {
                        maxWidth = textSize.X;
                    }
                }

                ImGui.Text("Craft: ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(maxWidth * 1.8f);
                if (ImGui.Combo("##CraftType", ref craftSelectedOption, options, options.Length))
                {
                    switch (craftSelectedOption)
                    {
                        case 0:
                            configuration.CraftType = 1;
                            break;
                        case 1:
                            configuration.CraftType = 2;
                            break;
                        default:
                            break;
                    }
                }
            }

            int maxQuality = configuration.MaxPercentage;
            if (ImGui.SliderInt("##SliderMaxQuality", ref maxQuality, 0, 100, $"%d%%"))
            {
                configuration.MaxPercentage = maxQuality;
            }

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            if (ImGui.Button("save"))
            {
                configuration.Save();
            }
            ImGui.End();
        }

        public void DrawYgatherOptionsWindow()
        {
            string[] options = ImGuiSettings.YGatherOptions;
            float maxWidth = 0f;
            for (int i = 0; i < options.Length; i++)
            {
                Vector2 textSize = ImGui.CalcTextSize(options[i]);
                if (textSize.X > maxWidth)
                {
                    maxWidth = textSize.X;
                }
            }

            ImGui.Text("YGather: ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(maxWidth * 1.8f);
            if (ImGui.Combo("##YGatherCombo", ref ygatherSelectedOption, options, options.Length))
            {
                switch (ygatherSelectedOption)
                {
                    case 0:
                        ygather = "0";
                        break;
                    case 1:
                        ygather = "1";
                        break;
                    case 2:
                        ygather = "2";
                        break;
                    case 3:
                        ygather = "3";
                        break;
                    case 4:
                        ygather = "4";
                        break;
                    case 5:
                        ygather = "11";
                        break;
                    case 6:
                        ygather = "12";
                        break;
                    case 7:
                        ygather = "13";
                        break;
                    case 8:
                        ygather = "14";
                        break;
                    default:
                        break;
                }
            }

            if (ImGui.Button("run"))
            {
                if ("0".Equals(ygather))
                {
                    PluginLog.Warning($"参数错误: {ygather}");
                }
                else
                {
                    Tasks.GatherInisland(ygather);
                }
            }
            ImGui.SameLine();
            if (ImGui.Button("stop"))
            {
                Tasks.GatherInisland("");
            }
        }
    }
}
