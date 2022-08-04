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
namespace Cube.Pdf.Tests.Ghostscript;

using System.Collections.Generic;
using Cube.Pdf.Ghostscript;
using NUnit.Framework;

/* ------------------------------------------------------------------------- */
///
/// PngTestCase
///
/// <summary>
/// Represents test cases for PostScript to PNG conversion.
/// These test cases are invoked via ConverterTest class.
/// </summary>
///
/* ------------------------------------------------------------------------- */
static class PngTestCase
{
    #region TestCases

    /* --------------------------------------------------------------------- */
    ///
    /// Get
    ///
    /// <summary>
    /// Gets the collection of test cases.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public static IEnumerable<TestCaseData> Get()
    {
        yield return Make("_Default",    new(Format.Png));
        yield return Make("32bppArgb",   new(Format.Png32bppArgb));
        yield return Make("24bppRgb",    new(Format.Png24bppRgb));
        yield return Make("8bppIndex",   new(Format.Png8bppIndexed));
        yield return Make("4bppIndex",   new(Format.Png4bppIndexed));
        yield return Make("8bppGray",    new(Format.Png8bppGrayscale));
        yield return Make("1bppMono",    new(Format.Png1bppMonochrome));
        yield return Make("NoAntiAlias", new(Format.Png) { AntiAlias = false });
    }

    #endregion

    #region Others

    /* --------------------------------------------------------------------- */
    ///
    /// Make
    ///
    /// <summary>
    /// Creates a new TestCaseData object.
    /// </summary>
    ///
    /// <param name="name">
    /// Test name, which is used for a part of the destination path.
    /// </param>
    ///
    /// <param name="converter">Converter object.</param>
    ///
    /* --------------------------------------------------------------------- */
    private static TestCaseData Make(string name, ImageConverter converter)
    {
        converter.Resolution = 96; // Reduce test time.
        return new("Png", name, "SampleMix.ps", converter);
    }

    #endregion
}