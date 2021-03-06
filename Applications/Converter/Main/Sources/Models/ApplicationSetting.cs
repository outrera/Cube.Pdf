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

namespace Cube.Pdf.Converter
{
    /* --------------------------------------------------------------------- */
    ///
    /// ApplicationSetting
    ///
    /// <summary>
    /// Represents the global settings of the application.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public sealed class ApplicationSetting
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// ApplicationSetting
        ///
        /// <summary>
        /// Initializes a new instance of the ApplicationSetting class.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private ApplicationSetting()
        {
            Locale.Subscribe(e => Properties.Resources.Culture = e.ToCultureInfo());
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Uri
        ///
        /// <summary>
        /// Gets the URL of the Web page.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static Uri Uri { get; } = new Uri("https://www.cube-soft.jp/cubepdf/");

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Configure
        ///
        /// <summary>
        /// Configures global settings.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static void Configure() => _core.Invoke();

        #endregion

        #region Fields
        private static readonly OnceAction _core = new OnceAction(() => new ApplicationSetting());
        #endregion
    }
}
