namespace Eye.Shared.Helpers;

public static class Writer
{
    public static void Banner()
    {
        const string phase = "[b gray] universal wallhack[/]";
        AnsiConsole.MarkupLine(@"[green]   _ \                      ___|  |     [/]");
        AnsiConsole.MarkupLine(@"[green]  |   | __ \   _ \  __ \   |      |     [/]");
        AnsiConsole.MarkupLine(@"[green]  |   | |   |  __/  |   |  |   |  |     [/]");
        AnsiConsole.MarkupLine(@"[green] \___/  .__/ \___| _|  _| \____| _____| [/]");
        AnsiConsole.MarkupLine($@"[green]       _|     {phase}                  [/]");
        AnsiConsole.WriteLine();
    }
    
    public static void Information(string message, bool scape = false)
    {
        var content = scape ? Markup.Escape(message) : message;
        AnsiConsole.MarkupLine($"[b gray]inf:[/] {content}");
    }
    
    public static void Success(string message, bool scape = false)
    {
        var content = scape ? Markup.Escape(message) : message;
        AnsiConsole.MarkupLine($"[b green]suc:[/] {content}");
    }
    
    public static void Error(string message, bool scape = false)
    {
        var content = scape ? Markup.Escape(message) : message;
        AnsiConsole.MarkupLine($"[b red]err:[/] {content}");
    }
}