#region header

// MouseJiggler - Program.cs
// Migrated to CommandLineParser

#endregion

#region using

using System;
using System.Threading;
using System.Windows.Forms;
using ArkaneSystems.MouseJiggler.Properties;
using JetBrains.Annotations;
using CommandLine;
using Windows.Win32;

#endregion

namespace ArkaneSystems.MouseJiggler;

[PublicAPI]
public static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        PInvoke.AttachConsole(Helpers.AttachParentProcess);

        var instance = new Mutex(false, "single instance: ArkaneSystems.MouseJiggler");

        try
        {
            if (instance.WaitOne(0))
            {
                // Parse arguments using CommandLineParser
                return Parser.Default.ParseArguments<Options>(args)
                    .MapResult(
                        (Options opts) => RootHandler(opts),
                        errs => 1);
            }
            else
            {
                Console.WriteLine(@"Mouse Jiggler is already running. Aborting.");
                return 1;
            }
        }
        finally
        {
            instance.Close();
            PInvoke.FreeConsole();
        }
    }

    private static int RootHandler(Options opts)
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var mainForm = new MainForm(
            opts.Jiggle,
            opts.Minimized,
            opts.Zen,
            opts.Seconds);

        Application.Run(mainForm);

        return 0;
    }
}

/// <summary>
/// Command line options for MouseJiggler
/// </summary>
public class Options
{
    [Option('j', "jiggle", Required = false, HelpText = "Start with jiggling enabled.")]
    public bool Jiggle { get; set; } = false;

    [Option('m', "minimized", Required = false, HelpText = "Start minimized.")]
    public bool Minimized { get; set; } = Settings.Default.MinimizeOnStartup;

    [Option('z', "zen", Required = false, HelpText = "Start with zen (invisible) jiggling enabled.")]
    public bool Zen { get; set; } = Settings.Default.ZenJiggle;

    [Option('s', "seconds", Required = false, HelpText = "Set X number of seconds for the jiggle interval.")]
    public int Seconds { get; set; } = Settings.Default.JigglePeriod;
}
