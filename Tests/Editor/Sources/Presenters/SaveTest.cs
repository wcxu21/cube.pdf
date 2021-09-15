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
using System.Linq;
using Cube.FileSystem;
using Cube.Tests;
using NUnit.Framework;

namespace Cube.Pdf.Editor.Tests.Presenters
{
    /* --------------------------------------------------------------------- */
    ///
    /// SaveTest
    ///
    /// <summary>
    /// Tests the Save commands.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    class SaveTest : VmFixture
    {
        #region Tests

        /* ----------------------------------------------------------------- */
        ///
        /// SaveAs
        ///
        /// <summary>
        /// Executes the test for saving the PDF document as a new file.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("Sample.pdf",         ""        )]
        [TestCase("SampleAes128.pdf",   "password")]
        [TestCase("SampleRc40.pdf",     "password")]
        [TestCase("SampleRc40Open.pdf", "password")]
        public void SaveAs(string file, string password)
        {
            var vp = new VmParam
            {
                Source   = GetSource(file),
                Save     = Get(Args(Io.Get(file).BaseName)),
                Password = password,
            };

            using var vm = NewVM();
            using var z0 = vm.Boot(vp);

            vp.Password = string.Empty;
            Assert.That(Io.Exists(vp.Save), Is.False);
            vm.Test(vm.Ribbon.SaveAs);
            Assert.That(Wait.For(() => Io.Exists(vp.Save)), vp.Save);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Overwrite
        ///
        /// <summary>
        /// Executes the test for overwriting the PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("Sample.pdf", "")]
        public void Overwrite(string file, string password)
        {
            var vp = new VmParam
            {
                Source   = Get(Args(Io.Get(GetSource(file)).BaseName)),
                Password = password,
            };

            Io.Copy(GetSource(file), vp.Source, true);

            using var vm = NewVM();
            using var z0 = vm.Boot(vp);

            vm.Value.Images.First().Selected = true;
            vm.Test(vm.Ribbon.RotateLeft);
            vm.Test(vm.Ribbon.Save);
        }

        #endregion
    }
}
