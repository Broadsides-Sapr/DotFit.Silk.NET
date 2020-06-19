// This file is part of Silk.NET.
// 
// You may modify and distribute Silk.NET under the terms
// of the MIT license. See the LICENSE file for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Silk.NET.BuildTools.Common;

namespace Silk.NET.BuildTools
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.SetOut(new ConsoleWriter(Console.Out));
            
            if (args.Length == 1 && args[0] == "jsonex")
            {
                // get a template json file
                Console.WriteLine
                (
                    JsonConvert.SerializeObject
                    (
                        ExampleJsonFile, Formatting.Indented
                    )
                );

                return;
            }
            
            foreach (var arg in args)
            {
                var abs = Path.GetFullPath(arg);
                Environment.CurrentDirectory = Path.GetDirectoryName
                    (abs) ?? throw new NullReferenceException("Dir path null.");
                Generator.Run(JsonConvert.DeserializeObject<Config>(File.ReadAllText(abs)));
            }
        }

        internal class ConsoleWriter : TextWriter
        {
            public Dictionary<int, string> Tasks { get; set; } = new Dictionary<int, string>();
            
            private readonly TextWriter _base;

            public ConsoleWriter(TextWriter @base)
            {
                _base = @base;
                Encoding = _base.Encoding;
            }
            public override Encoding Encoding { get; }
            public override void WriteLine(string? value)
            {
                Tasks.TryGetValue(Task.CurrentId ?? -1, out var val);
                _base.WriteLine($"[{DateTime.Now:T}] {val} {Task.CurrentId}> " + value);
            }
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////
        // The meaningful part of the file ends here, from here it's just an example structure.
        ///////////////////////////////////////////////////////////////////////////////////////

        private static Config ExampleJsonFile = new Config
        {
            Tasks = new[]
            {
                new BindTask
                {
                    BakeryOpts = new BakerySettings
                    {
                        Include = new[] {"glcore"}
                    },
                    CacheFolder = "/path/to/cacheFolder",
                    CacheKey = "glcoreCacheKey",
                    Controls = new[]
                    {
                        "control-variables-to-define-how-gernation-runs",
                        "convert-windows-only",
                        "convert-macos-only",
                        "convert-linux-only"
                    },
                    ConverterOpts = new ConverterOptions
                    {
                        Reader = "gl",
                        Constructor = "gl",
                        FunctionPrefix = "gl",
                        AdditionalArgs = new []{"--clang-args"},
                        ClassName = "GL"
                    },
                    ExtensionsNamespace = "MyNamespace.ForExtensions",
                    Namespace = "MyNamespace",
                    Name = "Profile Name",
                    Sources = new[] {"/path/to/sourceFile.xml", "/path/to/header.h"},
                    NameContainer = new NameContainer
                    {
                        ClassName = "MyNameContainer",
                        Android = "libapi.so",
                        IOS = "libapi.dylib",
                        Linux = "libapi.so",
                        MacOS = "libapi.dylib",
                        Windows = "api.dll"
                    },
                    OutputOpts = new OutputOptions
                    {
                        Folder = "/path/to/outputFolder", License = "/path/to/licenseFile",
                        Props = "/path/to/customMSBuild.props"
                    },
                    TypeMaps = new List<Dictionary<string, string>>
                        {new Dictionary<string, string> {{"HWND", "nint"}}}
                }
            }
        };
    }
}
