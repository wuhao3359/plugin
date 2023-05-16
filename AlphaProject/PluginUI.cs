﻿using ImGuiNET;
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
            if (ImGui.Begin("Operation Window", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                //ImGui.Text($"The random config bool is {this.configuration.SomePropertyToBeSavedAndWithADefault}");
                if (ImGui.Button("GetCurrentPosition"))
                {
                    Tasks.GetCurrentPosition();
                }
                ImGui.Spacing();
                if (ImGui.Button("FishInIsland Start"))
                {
                    // TODO args
                    String args = "100";
                    Tasks.FishInIsland(args);
                }
                ImGui.SameLine();
                if (ImGui.Button("FishInIsland End"))
                {
                    Tasks.FishInIsland("");
                }
                ImGui.Spacing();
                if (ImGui.Button("FishOnSea Start"))
                {
                    // TODO args
                    String args = "1";
                    Tasks.FishOnSea(args);
                }
                ImGui.SameLine();
                if (ImGui.Button("FishOnSea End"))
                {
                    Tasks.FishInIsland("");
                }
                ImGui.Spacing();
                if (ImGui.Button("AutoDaily Start"))
                {
                    // TODO args
                    String args = "duration:3 level:90-90 bagLimit:1 otherTask:5";
                    Tasks.AutoDaily(args);
                }
                ImGui.SameLine();
                if (ImGui.Button("AutoDaily End"))
                {
                    Tasks.AutoDaily("");
                }
                ImGui.Spacing();
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
