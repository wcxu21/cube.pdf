﻿/* ------------------------------------------------------------------------- */
//
// Copyright (c) 2010 CubeSoft, Inc.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
/* ------------------------------------------------------------------------- */
using Cube.FileSystem;
using Cube.Generics;
using System.Reflection;

namespace Cube.Pdf.App.Editor
{
    /* --------------------------------------------------------------------- */
    ///
    /// SettingsFolder
    ///
    /// <summary>
    /// Represents the application and/or user settings.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class SettingsFolder : SettingsFolder<Settings>
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// SettingsFolder
        ///
        /// <summary>
        /// Initializes a new instance of the SettingsFolder with the
        /// specified parameters.
        /// </summary>
        ///
        /// <param name="assembly">Assembly information.</param>
        /// <param name="io">I/O handler</param>
        ///
        /* ----------------------------------------------------------------- */
        public SettingsFolder(Assembly assembly, IO io) :
            this(assembly, Cube.DataContract.Format.Registry, @"CubeSoft\CubePDF Utility2", io) { }

        /* ----------------------------------------------------------------- */
        ///
        /// SettingsFolder
        ///
        /// <summary>
        /// Initializes a new instance of the SettingsFolder with the
        /// specified parameters.
        /// </summary>
        ///
        /// <param name="assembly">Assembly information.</param>
        /// <param name="format">Serialized format.</param>
        /// <param name="location">Location for the settings.</param>
        /// <param name="io">I/O handler</param>
        ///
        /* ----------------------------------------------------------------- */
        public SettingsFolder(Assembly assembly, Cube.DataContract.Format format, string location, IO io) :
            base(assembly, format, location, io)
        {
            var asm = assembly.GetReader();
            Title           = asm.Title;
            AutoSave        = false;
            Version.Digit   = 3;
            Version.Suffix  = Properties.Resources.VersionSuffix;

            var dir = IO.Get(asm.Location).DirectoryName;
            Startup.Name    = $"{Title} UpdateChecker";
            Startup.Command = IO.Combine(dir, $"UpdateChecker.exe").Quote();
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Title
        ///
        /// <summary>
        /// Gets the title of the application.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Title { get; }

        #endregion
    }
}
