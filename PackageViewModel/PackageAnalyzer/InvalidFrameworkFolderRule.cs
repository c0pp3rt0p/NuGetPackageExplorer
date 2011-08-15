﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using NuGet;
using NuGetPackageExplorer.Types;

namespace PackageExplorerViewModel.Rules {

    [Export(typeof(IPackageRule))]
    internal class InvalidFrameworkFolderRule : IPackageRule {
        public string Name {
            get {
                return "InvalidFrameworkFolder";
            }
        }

        public IEnumerable<PackageIssue> Check(IPackage package) {
            var set = new HashSet<string>();
            foreach (var file in package.GetFiles()) {
                string path = file.Path;
                string[] parts = path.Split(Path.DirectorySeparatorChar);
                if (parts.Length >= 3 && parts[0].Equals("lib", StringComparison.OrdinalIgnoreCase)) {
                    set.Add(parts[1]);
                }
            }

            return set.Where(IsInvalidFrameworkName).Select(CreatePackageIssue);
        }

        private bool IsInvalidFrameworkName(string name) {
            var frameworkName = VersionUtility.ParseFrameworkName(name);
            return frameworkName == VersionUtility.UnsupportedFrameworkName;
        }

        private PackageIssue CreatePackageIssue(string target) {
            return new PackageIssue(
                PackageIssueLevel.Warning,
                "Invalid framework folder",
                "The folder '" + target + "' under 'lib' is not recognized as a valid framework name.",
                "Rename it to a valid framework name."
            );
        }
    }
}