using Pinspaces.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Pinspaces.Plugins
{
    internal class PluginManager
    {
        private readonly List<Type> pinControlTypes = new();
        private readonly string pluginBasePath;

        public PluginManager()
        {
            pluginBasePath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(AppContext.BaseDirectory)), "plugins");
            LoadPlugins();
        }

        public IEnumerable<Type> PinControlTypes => pinControlTypes;

        private static Assembly LoadPlugin(string pluginPath)
        {
            var loadContext = new PluginLoadContext(pluginPath);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginPath)));
        }

        private void FindPluginTypes(Assembly plugin)
        {
            foreach (var type in plugin.GetTypes())
            {
                if (type.IsAbstract)
                {
                    continue;
                }
                if (typeof(IPinControl).IsAssignableFrom(type))
                {
                    pinControlTypes.Add(type);
                }
            }
        }

        private void LoadPlugins()
        {
            if (!Directory.Exists(pluginBasePath))
            {
                return;
            }
            var options = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = false,
                MatchCasing = MatchCasing.CaseInsensitive
            };
            var pluginDirectories = new List<string>(Directory.EnumerateDirectories(pluginBasePath, "*", options));
            foreach (var pluginDirectory in pluginDirectories)
            {
                var pluginDepsFiles = Directory.GetFiles(pluginDirectory, "*.deps.json", options);
                foreach (var depsFile in pluginDepsFiles)
                {
                    var depsContents = File.ReadAllText(depsFile);
                    var assemblyFileName = depsFile.Replace(".deps.json", ".dll", StringComparison.InvariantCultureIgnoreCase);
                    if (!depsContents.Contains("Pinspaces.Core") || !File.Exists(assemblyFileName))
                    {
                        continue;
                    }
                    var plugin = LoadPlugin(assemblyFileName);
                    FindPluginTypes(plugin);
                }
            }
        }
    }
}
