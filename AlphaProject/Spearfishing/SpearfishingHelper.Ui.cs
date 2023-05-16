using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Component.GUI;
using AlphaProject.Classes;
using AlphaProject.Enums;
using AlphaProject.SeFunctions;
using ImGuiNET;
using System.Threading.Tasks;
using AlphaProject.Utility;
using System;
using System.Threading;

namespace AlphaProject.Spearfishing;

public partial class SpearfishingHelper : Window
{
    private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoDecoration
      | ImGuiWindowFlags.NoInputs
      | ImGuiWindowFlags.AlwaysAutoResize
      | ImGuiWindowFlags.NoFocusOnAppearing
      | ImGuiWindowFlags.NoNavFocus
      | ImGuiWindowFlags.NoBackground;

    private        float            _uiScale       = 1;
    private        Vector2          _uiPos         = Vector2.Zero;
    private        Vector2          _uiSize        = Vector2.Zero;
    private unsafe SpearfishWindow* _addon         = null;
    private        Vector2          _listSizeText  = Vector2.Zero;
    private        Vector2          _listSizeIcons = Vector2.Zero;
    private const  float            _iconSize      = 30;

    private bool action = false;

    private Vector2 ListSize
        => ImGuiHelpers.GlobalScale * (true ? _listSizeText : _listSizeIcons);

    private unsafe void DrawFish(FishingSpot? spot, SpearfishWindow.Info info, AtkResNode* node, AtkResNode* fishLines, int idx)
    {
        if (!info.Available)
            return;

        if (!DalamudApi.ClientState.IsLoggedIn) {
            return;
        }
        
        var text = Identify(spot, info);
        var size = ImGui.CalcTextSize(text);
        var (x, y) = (node->X * _uiScale + node->Width * node->ScaleX * _uiScale / 2f,
                (node->Y + fishLines->Y + node->Height / 2f) * _uiScale - (size.Y + ImGui.GetStyle().FramePadding.Y) / 2f);
        //ImGui.SetCursorPos(new Vector2(x, y));
        //ImGuiUtil.DrawTextButton(text, Vector2.Zero, 0x40000000, 0xFFFFFFFF);

        //ImGui.SameLine();
        //ImGui.Text(info.Speed.ToName());

        if (Positions.IsNeedSpearfish(text)) {
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
                float r = new Random().Next(5, 10) / 100;
                x1 = size.X * (0.9 + r);
                x2 = size.X * (0.75 + r);
            }

            if (x > _uiSize.X / 2 - x1 && x < _uiSize.X / 2 + x2)
            {
                Task task = new(() =>
                {
                    if (!action)
                    {
                        action = true;
                        int r = new Random().Next(1, 100);
                        if (r >= 98)
                        {
                            Thread.Sleep(new Random().Next(0, 200));
                        }
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.r_key);
                        action = false;
                    }
                });
                task.Start();
            }
        }
    }

    private void DrawList()
    {
        //if (_currentSpot == null || _currentSpot.Items.Length == 0)
        //    return;

        //ImGui.SetCursorPos(_uiSize * Vector2.UnitX);
        //using var color = ImRaii.PushColor(ImGuiCol.ChildBg, 0x80000000);
        //using var style = ImRaii.PushStyle(ImGuiStyleVar.ChildRounding, 5 * ImGuiHelpers.GlobalScale);
        //using var child = ImRaii.Child("##ListChild", ListSize, true, ImGuiWindowFlags.NoScrollbar);
        //if (!child)
        //    return;

        //var iconSize = ImGuiHelpers.ScaledVector2(_iconSize, _iconSize);
        //foreach (var fish in _currentSpot.Items)
        //{
        //    var name = fish.Name[ClientLanguage.ChineseSimplified];
        //    ImGui.Image(IconId.FromSpeed(fish.Speed).ImGuiHandle, iconSize);
        //    ImGui.SameLine();
        //    ImGui.Image(IconId.FromSize(fish.Size).ImGuiHandle, iconSize);
        //    ImGui.SameLine();

        //    ImGui.Image(Icons.DefaultStorage[fish.ItemData.Icon].ImGuiHandle, iconSize);
        //    var pos = ImGui.GetCursorPos();
        //    ImGui.SameLine();
        //    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (iconSize.Y - ImGui.GetTextLineHeight()) / 2);
        //    ImGui.Text(name);
        //    ImGui.SetCursorPos(pos);
        //}
    }

    private unsafe void DrawFishOverlay()
    {
        DrawFish(_currentSpot, _addon->Fish1, _addon->Fish1Node, _addon->FishLines, 5);
        DrawFish(_currentSpot, _addon->Fish2, _addon->Fish2Node, _addon->FishLines, 3);
        DrawFish(_currentSpot, _addon->Fish3, _addon->Fish3Node, _addon->FishLines, 1);
    }

    private unsafe void DrawFishCenterLine()
    {
        var lineStart = _uiPos + new Vector2(_uiSize.X / 2, _addon->FishLines->Y * _uiScale);
        var lineEnd   = lineStart + new Vector2(0,          _addon->FishLines->Height * _uiScale);
        var list      = ImGui.GetWindowDrawList();
        list.AddLine(lineStart, lineEnd, 0xFF0000C0, 3 * ImGuiHelpers.GlobalScale);
    }

    private void ComputeListSize()
    {
        if (_currentSpot == null)
        {
            _listSizeIcons = Vector2.Zero;
            _listSizeText  = Vector2.Zero;
            return;
        }

        var padding = ImGui.GetStyle().WindowPadding / ImGuiHelpers.GlobalScale;
        var spacing = ImGui.GetStyle().ItemSpacing / ImGuiHelpers.GlobalScale;
        var y       = padding.Y * 1.5f + (spacing.Y + _iconSize) * _currentSpot.Items.Length;
        var xIcons = padding.X * 2
          + (spacing.X + _iconSize) * 3
          + _currentSpot.Items.Max(i => ImGui.CalcTextSize(i.Name[ClientLanguage.ChineseSimplified]).X) / ImGuiHelpers.GlobalScale;
        var xText = padding.X * 2
          + spacing.X
          + _iconSize
          + _currentSpot.Items.Max(i => ImGui.CalcTextSize($"{i.Name[ClientLanguage.ChineseSimplified]} ({i.Size.ToName()} and {i.Speed.ToName()})").X)
          / ImGuiHelpers.GlobalScale;
        _listSizeIcons = new Vector2(xIcons, y);
        _listSizeText  = new Vector2(xText,  y);
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
            //ComputeListSize();
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
        //DrawFishCenterLine();
        //DrawList();
    }
}
