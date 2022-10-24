Writer.Banner();

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Writer.Error("This release is only available for Windows.");
    Environment.Exit(1);
}

Writer.Information("Searching for the given process name...");
var processName = args.FirstOrDefault();
if (string.IsNullOrEmpty(processName))
{
    Writer.Error("You must provide the name of the target process.");
    Environment.Exit(1);
}

var processes = Process.GetProcesses()
    .Where(p => p.ProcessName.StartsWith(processName))
    .ToArray();

switch (processes.Length)
{
    case 0:
        Writer.Error($"Unable to find a running process of name {processName}.", true);
        Writer.Information("Ensure that you are running the correct build of Eye [dim](AMD64 or IA32)[/].");
        Environment.Exit(1);
        break;
    case > 1:
        Writer.Error($"Multiples process returned from the name {processName}.", true);
        Environment.Exit(1);
        break;
}

var process = processes.First();
Writer.Success($"Process {process.ProcessName} [dim](PID {process.Id})[/] found!");

var brain = new Brain(process);
var regions = brain.GetMemoryRegions().ToArray();
Writer.Information($"Found #{regions.Length} memory region(s)...");