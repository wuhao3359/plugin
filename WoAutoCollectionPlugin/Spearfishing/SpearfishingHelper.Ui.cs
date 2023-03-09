﻿using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Component.GUI;
using WoAutoCollectionPlugin.Classes;
using WoAutoCollectionPlugin.Enums;
using WoAutoCollectionPlugin.SeFunctions;
using ImGuiNET;

namespace WoAutoCollectionPlugin.Spearfishing;

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

    //private Vector2 ListSize
    //    => ImGuiHelpers.GlobalScale * (WoAutoCollectionPlugin.Config.ShowSpearfishListIconsAsText ? _listSizeText : _listSizeIcons);

    private unsafe void DrawFish(FishingSpot? spot, SpearfishWindow.Info info, AtkResNode* node, AtkResNode* fishLines, int idx)
    {
        if (!info.Available)
            return;

        var text = Identify(spot, info);
        var size = ImGui.CalcTextSize(text);
        var (x, y) = (node->X * _uiScale + node->Width * node->ScaleX * _uiScale / 2f - size.X / 2f,
                (node->Y + fishLines->Y + node->Height / 2f) * _uiScale - (size.Y + ImGui.GetStyle().FramePadding.Y) / 2f);
        ImGui.SetCursorPos(new Vector2(x, y));
        //ImGuiUtil.DrawTextButton(text, Vector2.Zero, ColorId.SpearfishHelperBackgroundFish.Value(), ColorId.SpearfishHelperTextFish.Value());

        ImGui.SameLine();
        ImGui.Text(info.Speed.ToName());
    }

    private void DrawList()
    {
        if (_currentSpot == null || _currentSpot.Items.Length == 0)
            return;

        ImGui.SetCursorPos(_uiSize * Vector2.UnitX);
        //using var color = ImRaii.PushColor(ImGuiCol.ChildBg, ColorId.SpearfishHelperBackgroundList.Value());
        //using var style = ImRaii.PushStyle(ImGuiStyleVar.ChildRounding, 5 * ImGuiHelpers.GlobalScale);
        //using var child = ImRaii.Child("##ListChild", ListSize, true, ImGuiWindowFlags.NoScrollbar);
        //if (!child)
        //    return;

        

        var iconSize = ImGuiHelpers.ScaledVector2(_iconSize, _iconSize);
        foreach (var fish in _currentSpot.Items)
        {
            var name = fish.Name[ClientLanguage.ChineseSimplified];
            name = $"{name} ({fish.Size.ToName()} and {fish.Speed.ToName()})";

            //ImGui.Image(Icons.DefaultStorage[fish.ItemData.Icon].ImGuiHandle, iconSize);
            var pos = ImGui.GetCursorPos();
            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (iconSize.Y - ImGui.GetTextLineHeight()) / 2);
            ImGui.Text(name);
            ImGui.SetCursorPos(pos);
        }
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
        //list.AddLine(lineStart, lineEnd, ColorId.SpearfishHelperCenterLine.Value(), 3 * ImGuiHelpers.GlobalScale);
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
            ComputeListSize();
        }

        //var drawOverlay = GatherBuddy.Config.ShowSpearfishNames
        // || GatherBuddy.Config.ShowSpearfishCenterLine
        // || GatherBuddy.Config.ShowAvailableSpearfish && _currentSpot != null && _currentSpot.Items.Length > 0;
        return true;
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
        DrawFishCenterLine();
        DrawList();
    }
}
