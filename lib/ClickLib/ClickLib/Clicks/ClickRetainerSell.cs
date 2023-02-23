using System;

using ClickLib.Attributes;
using ClickLib.Bases;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace ClickLib.Clicks;

/// <summary>
/// Addon RetainerTaskResult.
/// </summary>
public sealed unsafe class ClickRetainerSell : ClickBase<ClickRetainerSell, AddonRetainerSell>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClickRetainerSell"/> class.
    /// </summary>
    /// <param name="addon">Addon pointer.</param>
    public ClickRetainerSell(IntPtr addon = default)
        : base("RetainerSell", addon)
    {
    }

    public static implicit operator ClickRetainerSell(IntPtr addon) => new(addon);

    /// <summary>
    /// Instantiate this click using the given addon.
    /// </summary>
    /// <param name="addon">Addon to reference.</param>
    /// <returns>A click instance.</returns>
    public static ClickRetainerSell Using(IntPtr addon) => new(addon);

    [ClickName("confirm")]
    public void confirm()
    => this.ClickAddonButton(this.Addon->Confirm, 21);

}
