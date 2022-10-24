// ReSharper disable MemberCanBePrivate.Global
namespace Eye.Shared.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct MemoryBasicInformation
{
    public readonly IntPtr BaseAddress;
    public readonly IntPtr AllocationBase;
    public readonly uint AllocationProtect;
    public readonly ushort PartitionId;
    public readonly IntPtr RegionSize;
    public readonly uint State;
    public readonly MemoryProtection Protect;
    public readonly uint Type;

    public MemoryBasicInformation(
        IntPtr baseAddress,
        IntPtr allocationBase,
        uint allocationProtect,
        ushort partitionId,
        IntPtr regionSize,
        uint state,
        MemoryProtection protect,
        uint type
    )
    {
        BaseAddress = baseAddress;
        AllocationBase = allocationBase;
        AllocationProtect = allocationProtect;
        PartitionId = partitionId;
        RegionSize = regionSize;
        State = state;
        Protect = protect;
        Type = type;
    }
}