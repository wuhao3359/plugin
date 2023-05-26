using System.Numerics;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Component.GUI;
using AlphaProject.Classes;
using AlphaProject.SeFunctions;
using ImGuiNET;
using System.Threading.Tasks;
using AlphaProject.Utility;
using System;
using System.Threading;
using Dalamud.Logging;

namespace AlphaProject.Spearfishing;

public partial class SpearfishingHelper : Window
{
    private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoDecoration
      | ImGuiWindowFlags.NoInputs
      | ImGuiWindowFlags.AlwaysAutoResize
      | ImGuiWindowFlags.NoFocusOnAppearing
      | ImGuiWindowFlags.NoNavFocus
      | ImGuiWindowFlags.NoBackground;

    private object lockObject = new object();

    private        float            _uiScale       = 1;
    private        Vector2          _uiPos         = Vector2.Zero;
    private        Vector2          _uiSize        = Vector2.Zero;
    private unsafe SpearfishWindow* _addon         = null;
    private        Vector2          _listSizeText  = Vector2.Zero;
    private        Vector2          _listSizeIcons = Vector2.Zero;
    private const  float            _iconSize      = 30;

    private bool action = false;

    private unsafe void DrawFish(FishingSpot? spot, SpearfishWindow.Info info, AtkResNode* node, AtkResNode* fishLines, int idx)
    {
        if (!info.Available)
            return;

        if (!DalamudApi.ClientState.IsLoggedIn) {
            return;
        }

        AlphaProject.GameData.CommonBot.canUseNaturesBounty(true);

        var text = Identify(spot, info);
        var size = ImGui.CalcTextSize(text);
        var (x, y) = (node->X * _uiScale + node->Width * node->ScaleX * _uiScale / 2f,
                (node->Y + fishLines->Y + node->Height / 2f) * _uiScale - (size.Y + ImGui.GetStyle().FramePadding.Y) / 2f);

        if (Positions.IsNeedSpearfish(text)) {
            AlphaProject.GameData.CommonBot.canUseNaturesBounty(false);
            double x1 = size.X * 0.9;
            double x2 = size.X * 0.75;
            if (idx == 5)
            {   // 第三行
                float r = new Random().Next(5, 10) / 100;
                x1 = size.X * (0.7 + r);
                x2 = size.X * (0.55 + r);
            }
            else if (idx == 3)
            {   // 第二行
                float r = new Random().Next(5, 10) / 100;
                x1 = size.X * (0.8 + r);
                x2 = size.X * (0.65 + r);
            }
            else if (idx == 1)
            {   // 第一行
                float r = new Random().Next(5, 15) / 100;
                x1 = size.X * (0.9 + r);
                x2 = size.X * (0.75 + r);
            }

            if (x > _uiSize.X / 2 - x1 && x < _uiSize.X / 2 + x2)
            {
                lock (lockObject) {
                    Task task = new(() =>
                    {
                        if (!action)
                        {
                            action = true;
                            //int r = new Random().Next(1, 100);
                            //if (r >= 98)
                            //{
                            //    Thread.Sleep(new Random().Next(100, 200));
                            //}
                            //PluginLog.Log("刺一下...");
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.r_key);
                            Thread.Sleep(new Random().Next(100, 200));
                            action = false;
                        }
                    });
                    task.Start();
                }
            }
        }
    }

    private unsafe void DrawFishOverlay()
    {
        DrawFish(_currentSpot, _addon->Fish1, _addon->Fish1Node, _addon->FishLines, 5);
        DrawFish(_currentSpot, _addon->Fish2, _addon->Fish2Node, _addon->FishLines, 3);
        DrawFish(_currentSpot, _addon->Fish3, _addon->Fish3Node, _addon->FishLines, 1);
    }

    public override unsafe bool DrawConditions()
    {
        var oldOpen = _isOpen;
        _addon  = (SpearfishWindow*)DalamudApi.GameGui.GetAddonByName("SpearFishing", 1);
        _isOpen = _addon != null && _addon->Base.WindowNode != null;
        if (!_isOpen)
            return false;

        if (_isOpen != oldOpen)
        {
            _currentSpot = GetTargetFishingSpot();
        }

        var drawOverlay =  _currentSpot != null && _currentSpot.Items.Length > 0;
        return drawOverlay;
    }

    public override void PreOpenCheck()
    {
        IsOpen = true;
    }

    public override unsafe void PreDraw()
    {
        _uiScale = _addon->Base.Scale;
        _uiPos   = new Vector2(_addon->Base.X, _addon->Base.Y);
        _uiSize = new Vector2(_addon->Base.WindowNode->AtkResNode.Width * _uiScale,
            _addon->Base.WindowNode->AtkResNode.Height * _uiScale);

        Position = _uiPos;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = _uiSize,
            MaximumSize = Vector2.One * 10000,
        };
    }

    public override void Draw()
    {
        DrawFishOverlay();
    }
}
