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
using System;
using System.Linq;
using Cube.FileSystem;
using Cube.Mixin.Logging;

namespace Cube.Pdf.Editor
{
    /* --------------------------------------------------------------------- */
    ///
    /// Backup
    ///
    /// <summary>
    /// Provides functionality to backup files.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public sealed class Backup
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// DirectoryMonitor
        ///
        /// <summary>
        /// Initializes a new instance of the Backup class with the
        /// specified arguments.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Backup()
        {
            var app = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            Directory = Io.Combine(app, "CubeSoft", "CubePdfUtility2", "Backup");
            KeepDays  = 5;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Directory
        ///
        /// <summary>
        /// Gets or sets the backup directory.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Directory { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// KeepDays
        ///
        /// <summary>
        /// Gets the days to keep backup files.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int KeepDays { get; set; }

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Invoke
        ///
        /// <summary>
        /// Copies the specified file to the backup directory.
        /// </summary>
        ///
        /// <param name="src">Source file.</param>
        ///
        /* ----------------------------------------------------------------- */
        public void Invoke(Entity src)
        {
            if (!src.Exists) return;

            var date = DateTime.Today.ToString("yyyyMMdd");
            var dest = Io.Combine(Directory, date, src.Name);

            if (!Io.Exists(dest)) Io.Copy(src.FullName, dest, false);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Cleanup
        ///
        /// <summary>
        /// Deletes expired files.
        /// </summary>
        ///
        /// <remarks>
        /// 保持日数と同数のディレクトリまでは削除せずに保持する事とします。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public void Cleanup()
        {
            var src = Io.GetDirectories(Directory);
            var n   = src.Count() - KeepDays;

            if (n <= 0) return;
            foreach (var f in src.OrderBy(e => e).Take(n)) GetType().LogWarn(() => Io.Delete(f));
        }

        #endregion
    }
}
