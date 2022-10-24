namespace Eye.Shared;

public class Brain
{
    private readonly SafeProcessHandle _handle;

    public Brain(Process process)
    {
        _handle = process.SafeHandle;
    }

    public IEnumerable<MemoryBasicInformation> GetMemoryRegions(IntPtr baseAddress = new(), IntPtr finalAddress = new(),
        Func<MemoryProtection, bool>? hasRestrictivePermissions = null, bool reThrownWin32Exceptions = false)
    {
        if (finalAddress != IntPtr.Zero && finalAddress.ToInt64() <= baseAddress.ToInt64())
        {
            throw new ArgumentOutOfRangeException(
                nameof(baseAddress),
                "Your base address is less or equal than your final address."
            );
        }

        hasRestrictivePermissions ??= HasRestrictivePermissions;

        var systemInfo = new SystemInfo();

        if (Environment.Is64BitProcess) Kernel32.GetNativeSystemInfo(ref systemInfo);
        else Kernel32.GetSystemInfo(ref systemInfo);

        var memoryInformation = GetMemoryRegion(baseAddress);
        if (!hasRestrictivePermissions(memoryInformation.Protect)) yield return memoryInformation;

        var minimumAppAddress = systemInfo.lpMinimumApplicationAddress;
        var maximumAppAddress = systemInfo.lpMaximumApplicationAddress;

        if (memoryInformation.BaseAddress != IntPtr.Zero &&
            memoryInformation.BaseAddress.ToInt64() < minimumAppAddress.ToInt64())
        {
            throw new ArgumentOutOfRangeException(
                nameof(baseAddress),
                "Your base address resolved to a memory region bellow the minimum."
            );
        }

        var maximumRange = new IntPtr(memoryInformation.BaseAddress.ToInt64() + memoryInformation.RegionSize.ToInt64());
        if (maximumRange.ToInt64() > maximumAppAddress.ToInt64())
        {
            throw new ArgumentOutOfRangeException(
                nameof(baseAddress),
                "Your base address resolved to a memory region after the limit."
            );
        }

        while (true)
        {
            try
            {
                var deeperAddress = new IntPtr(
                    memoryInformation.BaseAddress.ToInt64() + memoryInformation.RegionSize.ToInt64()
                );
                if (finalAddress != IntPtr.Zero && deeperAddress.ToInt64() > finalAddress.ToInt64()) break;

                memoryInformation = GetMemoryRegion(deeperAddress);
            }
            catch (Win32Exception)
            {
                if (reThrownWin32Exceptions)
                {
                    var error = Marshal.GetLastWin32Error();
                    throw new Win32Exception($"Unable to query memory region, returned {error}.");
                }
                break;
            }

            if (!hasRestrictivePermissions(memoryInformation.Protect)) yield return memoryInformation;
        }
    }

    public MemoryBasicInformation GetMemoryRegion(IntPtr baseAddress = new())
    {
        var systemInfo = new SystemInfo();

        if (Environment.Is64BitProcess) Kernel32.GetNativeSystemInfo(ref systemInfo);
        else Kernel32.GetSystemInfo(ref systemInfo);

        var minimumAppAddress = systemInfo.lpMinimumApplicationAddress;

        // Maybe I should do a unsafe pointer arithmetic instead of fighting with vALU?
        if (baseAddress != IntPtr.Zero && baseAddress.ToInt64() < minimumAppAddress.ToInt64())
        {
            throw new ArgumentOutOfRangeException(
                nameof(baseAddress),
                "Your base address resolved to a memory region bellow the minimum."
            );
        }

        var size = Marshal.SizeOf<MemoryBasicInformation>();
        if (Kernel32.VirtualQueryEx(_handle, baseAddress, out var memoryInformation, size)) return memoryInformation;

        var error = Marshal.GetLastWin32Error();
        throw new Win32Exception($"Unable to query memory region, returned {error}.");
    }

    private static bool HasRestrictivePermissions(MemoryProtection memoryProtection) => memoryProtection switch
    {
        MemoryProtection.ZeroAccess => true,
        MemoryProtection.Guard => true,
        MemoryProtection.NoAccess => true,
        MemoryProtection.ReadWriteGuard => true,
        MemoryProtection.ReadOnly => false,
        MemoryProtection.ReadWrite => false,
        MemoryProtection.WriteCopy => false,
        MemoryProtection.Execute => false,
        MemoryProtection.ExecuteRead => false,
        MemoryProtection.ExecuteReadWrite => false,
        MemoryProtection.ExecuteWriteCopy => false,
        MemoryProtection.NoCache => false,
        _ => false
    };
}