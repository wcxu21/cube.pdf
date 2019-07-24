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
using Cube.Xui;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Cube.Pdf.Editor
{
    /* --------------------------------------------------------------------- */
    ///
    /// MetadataViewModel
    ///
    /// <summary>
    /// Represents the ViewModel for a MetadataWindow instance.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public sealed class MetadataViewModel : DialogViewModel<MetadataFacade>
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// MetadataViewModel
        ///
        /// <summary>
        /// Initializes a new instance of the MetadataViewModel
        /// with the specified arguments.
        /// </summary>
        ///
        /// <param name="callback">Callback method when applied.</param>
        /// <param name="src">Metadata object.</param>
        /// <param name="file">File information.</param>
        /// <param name="context">Synchronization context.</param>
        ///
        /* ----------------------------------------------------------------- */
        public MetadataViewModel(Action<Metadata> callback,
            Metadata src,
            Entity file,
            SynchronizationContext context
        ) : base(new MetadataFacade(src, file),
            new Aggregator(),
            context
        ) {
            OK.Command = new DelegateCommand(() => { Send<CloseMessage>(); callback(src); });
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Versions
        ///
        /// <summary>
        /// Gets the collection of PDF version numbers.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IEnumerable<PdfVersion> Versions => MetadataFacade.Versions;

        /* ----------------------------------------------------------------- */
        ///
        /// Options
        ///
        /// <summary>
        /// Gets the collection of viewer options.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IEnumerable<ViewerOption> Options => MetadataFacade.ViewerOptions;

        /* ----------------------------------------------------------------- */
        ///
        /// Filename
        ///
        /// <summary>
        /// Gets the menu that represents the filename.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> Filename => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuFilename,
            () => Facade.File.Name,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Document
        ///
        /// <summary>
        /// Gets the menu that represents the title of the specified
        /// PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> Document => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuTitle,
            () => Facade.Value.Title,
            e  => Facade.Value.Title = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Author
        ///
        /// <summary>
        /// Gets the menu that represents the author of the specified
        /// PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> Author => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuAuthor,
            () => Facade.Value.Author,
            e  => Facade.Value.Author = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Subject
        ///
        /// <summary>
        /// Gets the menu that represents the subject of the specified
        /// PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> Subject => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuSubject,
            () => Facade.Value.Subject,
            e  => Facade.Value.Subject = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Keywords
        ///
        /// <summary>
        /// Gets the menu that represents keywords of the specified
        /// PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> Keywords => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuKeywords,
            () => Facade.Value.Keywords,
            e  => Facade.Value.Keywords = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Creator
        ///
        /// <summary>
        /// Gets the menu that represents the creation program of the
        /// specified PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> Creator => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuCreator,
            () => Facade.Value.Creator,
            e  => Facade.Value.Creator = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Producer
        ///
        /// <summary>
        /// Gets the menu that represents the generating program of the
        /// specified PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> Producer => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuProducer,
            () => Facade.Value.Producer,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Version
        ///
        /// <summary>
        /// Gets the menu that represents the PDF version of the specified
        /// document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<PdfVersion> Version => Get(() => new BindableElement<PdfVersion>(
            () => Properties.Resources.MenuVersion,
            () => Facade.Value.Version,
            e  => Facade.Value.Version = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Option
        ///
        /// <summary>
        /// Gets the menu that represents the viewer options of the
        /// PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<ViewerOption> Option => Get(() => new BindableElement<ViewerOption>(
            () => Properties.Resources.MenuLayout,
            () => Facade.Value.Options,
            e  => Facade.Value.Options = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Length
        ///
        /// <summary>
        /// Gets the menu that represents the length of the specified file.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<long> Length => Get(() => new BindableElement<long>(
            () => Properties.Resources.MenuFilesize,
            () => Facade.File.Length,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// CreationTime
        ///
        /// <summary>
        /// Gets the menu that represents the creation time of the
        /// specified file.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<DateTime> CreationTime => Get(() => new BindableElement<DateTime>(
            () => Properties.Resources.MenuCreationTime,
            () => Facade.File.CreationTime,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// LastWriteTime
        ///
        /// <summary>
        /// Gets the menu that represents the last updated time of the
        /// specified file.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<DateTime> LastWriteTime => Get(() => new BindableElement<DateTime>(
            () => Properties.Resources.MenuLastWriteTime,
            () => Facade.File.LastWriteTime,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Summary
        ///
        /// <summary>
        /// Gets the menu that represents the summary group.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement Summary => Get(() => new BindableElement(
            () => Properties.Resources.MenuSummary,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Details
        ///
        /// <summary>
        /// Gets the menu that represents the details group.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement Details => Get(() => new BindableElement(
            () => Properties.Resources.MenuDetails,
            GetInvoker(false)
        ));

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// GetTitle
        ///
        /// <summary>
        /// Gets the title of the dialog.
        /// </summary>
        ///
        /// <returns>String value.</returns>
        ///
        /* ----------------------------------------------------------------- */
        protected override string GetTitle() => Properties.Resources.TitleMetadata;

        #endregion
    }
}
