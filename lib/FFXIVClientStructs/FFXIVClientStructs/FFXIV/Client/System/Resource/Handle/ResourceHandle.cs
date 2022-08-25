﻿namespace FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
// Client::System::Resource::Handle::ResourceHandle
//   Client::System::Common::NonCopyable

// size = 0xB0
// ctor E8 ? ? ? ? 81 A3 ? ? ? ? ? ? ? ? 48 8D 05 ? ? ? ? 
[StructLayout(LayoutKind.Explicit, Size = 0xB0)]
public partial struct ResourceHandle
{
    [FieldOffset(0x08)] public ResourceCategory Category;
    [FieldOffset(0x0C)] public uint FileType; // "txt" "uld" etc from the header
    [FieldOffset(0x10)] public uint Id;
    [FieldOffset(0x48)] public StdString FileName; // std::string
    [FieldOffset(0xAC)] public uint RefCount;

    [MemberFunction("E8 ?? ?? ?? ?? 48 C7 03 ?? ?? ?? ?? C6 83")]
    public partial bool DecRef();

    [MemberFunction("E8 ?? ?? ?? ?? 41 8B 46 30 C1 E0 05")]
    public partial bool IncRef();
}