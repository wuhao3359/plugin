using System;

using ClickLib.Attributes;
using ClickLib.Bases;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace ClickLib.Clicks;

/// <summary>
/// Adddon SalvageDialog.
/// </summary>
public sealed unsafe class ClickGathering : ClickBase<ClickGathering, AddonGathering>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClickGathering"/> class.
    /// </summary>
    /// <param name="addon">Addon pointer.</param>
    public ClickGathering(IntPtr addon = default)
        : base("Gathering", addon)
    {
    }

    public static implicit operator ClickGathering(IntPtr addon) => new(addon);

    /// <summary>
    /// Click the desynthesize checkbox button.
    /// </summary>
    [ClickName("quick_gathering")]
    public void QuickGatheringComponentCheckBox()
    => this.ClickAddonCheckBox(this.Addon->QuickGatheringComponentCheckBox, 0);

    /// <summary>
    /// Click the desynthesize checkbox button.
    /// </summary>
    [ClickName("gathering_checkbox1")]
    public void CheckBox1()
        => this.ClickAddonCheckBox(this.Addon->GatheredItemComponentCheckBox1, 0);

    /// <summary>
    /// Click the desynthesize checkbox button.
    /// </summary>
    [ClickName("gathering_checkbox2")]
    public void CheckBox2()
    => this.ClickAddonCheckBox(this.Addon->GatheredItemComponentCheckBox2, 1);

    /// <summary>
    /// Click the desynthesize checkbox button.
    /// </summary>
    [ClickName("gathering_checkbox3")]
    public void CheckBox3()
    => this.ClickAddonCheckBox(this.Addon->GatheredItemComponentCheckBox3, 2);

    /// <summary>
    /// Click the desynthesize checkbox button.
    /// </summary>
    [ClickName("gathering_checkbox4")]
    public void CheckBox4()
    => this.ClickAddonCheckBox(this.Addon->GatheredItemComponentCheckBox4, 3);
}
