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
using Cube.Mixin.Collections;
using Cube.Mixin.Logging;
using Cube.Mixin.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Cube.Pdf.Converter
{
    /* --------------------------------------------------------------------- */
    ///
    /// Facade
    ///
    /// <summary>
    /// Represents the facade of converting operations.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public sealed class Facade : DisposableBase
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// Facade
        ///
        /// <summary>
        /// Initializes a new instance of the specified settings.
        /// </summary>
        ///
        /// <param name="settings">User settings.</param>
        ///
        /* ----------------------------------------------------------------- */
        public Facade(SettingsFolder settings)
        {
            Settings = settings;
            Locale.Set(settings.Value.Language);
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Settings
        ///
        /// <summary>
        /// Gets the user settings.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public SettingsFolder Settings { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// IO
        ///
        /// <summary>
        /// Gets the I/O handler.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IO IO => Settings.IO;

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Convert
        ///
        /// <summary>
        /// Invokes the conversion with the provided settings.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Convert() => Invoke(() =>
        {
            var format = Settings.Value.Format;
            var dest   = Settings.Value.Destination;
            var work   = Settings.WorkDirectory;

            this.LogDebug($"{nameof(Settings.WorkDirectory)}:{work}");

            using (var fs = new FileTransfer(format, dest, work, IO))
            {
                fs.AutoRename = Settings.Value.SaveOption == SaveOption.Rename;
                InvokeGhostscript(fs.Value);
                InvokeDecorator(fs.Value);
                InvokeTransfer(fs, out var paths);
                InvokePostProcess(paths);
            }
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        ///
        /// <summary>
        /// Saves the current settings.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Save() => Settings.Save();

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// Dispose
        ///
        /// <summary>
        /// Releases the unmanaged resources used by the object and
        /// optionally releases the managed resources.
        /// </summary>
        ///
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.
        /// </param>
        ///
        /* ----------------------------------------------------------------- */
        protected override void Dispose(bool disposing)
        {
            Poll(10).Wait();
            IO.TryDelete(Settings.WorkDirectory);
            if (Settings.Value.DeleteSource) IO.TryDelete(Settings.Value.Source);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetDigest
        ///
        /// <summary>
        /// Gets the message digest of the specified file.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private string GetDigest(string src)
        {
            using (var stream = IO.OpenRead(src))
            {
                return new SHA256CryptoServiceProvider()
                    .ComputeHash(stream)
                    .Join("", b => $"{b:x2}");
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Poll
        ///
        /// <summary>
        /// Waits until the any operations are terminated.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private async Task Poll(int sec)
        {
            for (var i = 0; i < sec; ++i)
            {
                if (!Settings.Value.Busy) return;
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        #region Invoke

        /* ----------------------------------------------------------------- */
        ///
        /// Invoke
        ///
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        ///
        /// <remarks>
        /// 処理中は IsBusy プロパティが true に設定されます。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        private void Invoke(Action action)
        {
            try
            {
                Settings.Value.Busy = true;
                action();
            }
            finally { Settings.Value.Busy = false; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// InvokeUnlessDisposed
        ///
        /// <summary>
        /// Invokes the specified action unless the object is not Disposed.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void InvokeUnlessDisposed(Action action)
        {
            if (!Disposed) action();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// InvokeGhostscript
        ///
        /// <summary>
        /// Invokes the Ghostscript API.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void InvokeGhostscript(string dest) => InvokeUnlessDisposed(() =>
        {
            var cmp = GetDigest(Settings.Value.Source);
            if (!Settings.Digest.FuzzyEquals(cmp)) throw new CryptographicException();

            var gs = GhostscriptFactory.Create(Settings);
            gs.Invoke(Settings.Value.Source, dest);
            gs.LogDebug();
        });

        /* ----------------------------------------------------------------- */
        ///
        /// InvokeDecorator
        ///
        /// <summary>
        /// Invokes additional operations against the file generated by
        /// Ghostscript API.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void InvokeDecorator(string dest) =>
            InvokeUnlessDisposed(() => new FileDecorator(Settings).Invoke(dest));

        /* ----------------------------------------------------------------- */
        ///
        /// InvokeTransfer
        ///
        /// <summary>
        /// Moves files from the working directory.
        /// </summary>
        ///
        /// <remarks>
        /// out 引数の都合で、このメソッドのみ InvokeUnlessDisposed に
        /// 相当する処理を直接記述しています。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        private void InvokeTransfer(FileTransfer src, out IEnumerable<string> paths)
        {
            paths = !Disposed ? src.Invoke() : Enumerable.Empty<string>();
            foreach (var f in paths) this.LogDebug($"Save:{f}");
        }

        /* ----------------------------------------------------------------- */
        ///
        /// InvokePostProcess
        ///
        /// <summary>
        /// Invokes the post process.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void InvokePostProcess(IEnumerable<string> dest) =>
            InvokeUnlessDisposed(() => new ProcessLauncher(Settings).Invoke(dest));

        #endregion

        #endregion
    }
}
