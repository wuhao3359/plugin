using System;

using ClickLib.Attributes;
using ClickLib.Bases;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace ClickLib.Clicks;

/// <summary>
/// Adddon SalvageDialog.
/// </summary>
public sealed unsafe class ClickCollectablesShop : ClickBase<ClickCollectablesShop>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectablesShop"/> class.
    /// </summary>
    /// <param name="addon">Addon pointer.</param>
    public ClickCollectablesShop(IntPtr addon = default)
        : base("CollectablesShop", addon)
    {
    }

    public static implicit operator ClickCollectablesShop(IntPtr addon) => new(addon);
}
