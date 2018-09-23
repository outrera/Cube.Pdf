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
using Cube.FileSystem.TestService;
using Cube.Pdf.App.Editor;
using Cube.Xui.Mixin;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cube.Pdf.Tests.Editor.ViewModels
{
    /* --------------------------------------------------------------------- */
    ///
    /// ViewTest
    ///
    /// <summary>
    /// Tests for viewing operations of the MainViewModel class.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    class ViewTest : ViewModelFixture
    {
        #region Tests

        /* ----------------------------------------------------------------- */
        ///
        /// Preview
        ///
        /// <summary>
        /// Executes the test to preview an item.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public Task Preview() => CreateAsync("Sample.pdf", "", 2, async (vm) =>
        {
            var cts = new CancellationTokenSource();
            var dp  = vm.Register<PreviewViewModel>(this, e =>
            {
                Assert.That(e.Title.Text,        Is.Not.Null.And.Not.Empty);
                Assert.That(e.Data.File.Value,   Is.Not.Null);
                Assert.That(e.Data.Width.Value,  Is.GreaterThan(0));
                Assert.That(e.Data.Height.Value, Is.GreaterThan(0));

                Assert.That(Wait.For(() => !e.Data.Busy.Value), "Timeout (PreviewImage)");
                Assert.That(e.Data.Image.Value,  Is.Not.Null);

                e.Cancel.Command.Execute();
                cts.Cancel(); // done
            });

            await ExecuteAsync(vm, vm.Ribbon.Select);
            Assert.That(vm.Ribbon.Preview.Command.CanExecute(), Is.True);
            vm.Ribbon.Preview.Command.Execute();
            await Wait.ForAsync(cts.Token);
            dp.Dispose();
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Select
        ///
        /// <summary>
        /// Executes the test to select some items.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void Select() => Create("SampleRotation.pdf", "", 9, vm =>
        {
            var dest = vm.Data.Selection;
            Assert.That(dest.Count,   Is.EqualTo(0));
            Assert.That(dest.Items,   Is.Not.Null);
            Assert.That(dest.Indices, Is.Not.Null);
            Assert.That(dest.Last,    Is.EqualTo(-1));

            vm.Data.Images.First().IsSelected = true;
            Assert.That(Wait.For(() => !vm.Data.Busy.Value));
            Assert.That(dest.Count, Is.EqualTo(1), nameof(dest.Count));
            Assert.That(dest.Last,  Is.EqualTo(0), nameof(dest.Last));

            Execute(vm, vm.Ribbon.SelectFlip);
            Assert.That(dest.Count, Is.EqualTo(8), nameof(dest.Count));
            Assert.That(dest.Last,  Is.EqualTo(8), nameof(dest.Last));

            Execute(vm, vm.Ribbon.Select); // SelectAll
            Assert.That(dest.Count, Is.EqualTo(9), nameof(dest.Count));
            Assert.That(dest.Last,  Is.EqualTo(8), nameof(dest.Last));

            Execute(vm, vm.Ribbon.Select); // SelectClear
            Assert.That(dest.Count, Is.EqualTo(0));
        });

        #endregion
    }
}