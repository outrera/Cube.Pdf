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
using Cube.Mixin.Pdf;
using Cube.Pdf.Ghostscript;
using Cube.Pdf.Itext;
using System;

namespace Cube.Pdf.Converter
{
    /* --------------------------------------------------------------------- */
    ///
    /// FileDecorator
    ///
    /// <summary>
    /// Provides functionality to invoke additional operations to files
    /// that are generated by Ghostscript API.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    internal sealed class FileDecorator
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// FileDecorator
        ///
        /// <summary>
        /// Initializes a new instance of the FileDecorator class with the
        /// specified settings.
        /// </summary>
        ///
        /// <param name="src">User settings.</param>
        ///
        /* ----------------------------------------------------------------- */
        public FileDecorator(SettingFolder src) { Setting = src; }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Setting
        ///
        /// <summary>
        /// Gets the instance of the SettingFolder class.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public SettingFolder Setting { get; }

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Invoke
        ///
        /// <summary>
        /// Invokes operations on the specified file.
        /// </summary>
        ///
        /// <param name="src">Path of the source file.</param>
        ///
        /* ----------------------------------------------------------------- */
        public void Invoke(string src)
        {
            if (Setting.Value.Format != Format.Pdf) return;

            InvokeItext(src);
            InvokeLinearization(src);
        }

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// InvokeItext
        ///
        /// <summary>
        /// Invokes iTextSharp operations.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void InvokeItext(string src)
        {
            var io    = Setting.IO;
            var value = Setting.Value;
            var tmp   = io.Combine(io.Get(src).DirectoryName, Guid.NewGuid().ToString("D"));

            using (var writer = new DocumentWriter(io))
            {
                value.Encryption.Method = GetEncryptionMethod(value.Metadata.Version);
                writer.Set(value.Metadata);
                writer.Set(value.Encryption);
                Add(writer, value.Destination, SaveOption.MergeTail);
                writer.Add(new DocumentReader(src, string.Empty, false, io));
                Add(writer, value.Destination, SaveOption.MergeHead);
                writer.Save(tmp);
            }

            io.Move(tmp, src, true);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// InvokeLinearization
        ///
        /// <summary>
        /// Invokes the linearization on the specified PDF file.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void InvokeLinearization(string src)
        {
            var io    = Setting.IO;
            var value = Setting.Value;

            if (!value.Linearization || value.Encryption.Enabled) return;

            if (GhostscriptFactory.Create(Setting) is PdfConverter gs)
            {
                var tmp = io.Combine(io.Get(src).DirectoryName, Guid.NewGuid().ToString("D"));
                gs.Linearization = value.Linearization;
                gs.Invoke(src, tmp);
                io.Move(tmp, src, true);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Add
        ///
        /// <summary>
        /// Adds the collection of Pages to the specified writer.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void Add(DocumentWriter src, string path, SaveOption so)
        {
            var io    = Setting.IO;
            var value = Setting.Value;

            if (value.SaveOption != so || !io.Exists(path)) return;

            var password = value.Encryption.Enabled ?
                           value.Encryption.OwnerPassword :
                           string.Empty;

            src.Add(new DocumentReader(path, password, true, io));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetEncryptionMethod
        ///
        /// <summary>
        /// Gets an EncryptionMethod value from the specified version.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private EncryptionMethod GetEncryptionMethod(PdfVersion src) =>
            src.Minor >= 7 ? EncryptionMethod.Aes256 :
            src.Minor >= 6 ? EncryptionMethod.Aes128 :
            src.Minor >= 4 ? EncryptionMethod.Standard128 :
                             EncryptionMethod.Standard40;

        #endregion
    }
}
