﻿/* ------------------------------------------------------------------------- */
///
/// DocumentReaderTest.cs
/// 
/// Copyright (c) 2010 CubeSoft, Inc.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///  http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
///
/* ------------------------------------------------------------------------- */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;

namespace Cube.Pdf.Tests.Drawing
{
    /* --------------------------------------------------------------------- */
    ///
    /// DocumentReaderTest
    /// 
    /// <summary>
    /// DocumentReader のテストを行うクラスです。
    /// </summary>
    /// 
    /* --------------------------------------------------------------------- */
    [Parallelizable]
    [TestFixture]
    class DocumentReaderTest : FileResource
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Open
        ///
        /// <summary>
        /// PDF ファイルを開くテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        #region Open

        /* ----------------------------------------------------------------- */
        ///
        /// Open
        ///
        /// <summary>
        /// PDF ファイルを開くテストを実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("rotation.pdf",        "",         true)]
        [TestCase("password.pdf",        "password", true)]
        [TestCase("password.pdf",        "view",     true /* false */)]
        [TestCase("password-aes256.pdf", "password", true)]
        public void Open(string filename, string password, bool fullAccess)
        {
            var src = Example(filename);
            using (var reader = new Cube.Pdf.Drawing.DocumentReader())
            {
                reader.Open(src, password);
                Assert.That(reader.IsOpen, Is.True);
                Assert.That(((PdfFile)reader.File).FullAccess, Is.EqualTo(fullAccess));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Open_PasswordRequired
        ///
        /// <summary>
        /// 間違ったパスワードを入力して PDF ファイルを開こうとするテストを
        /// 実行します。
        /// </summary>
        /// 
        /// <remarks>
        /// 現在、パスワードの入力エラーとそれ以外のエラーの判別が
        /// できないため、未実装となっています。要修正。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        //[Test]
        public void Open_EncryptionException() => Assert.That(() =>
        {
            using (var reader = new Cube.Pdf.Drawing.DocumentReader())
            {
                reader.Open(Example("password.pdf"), "bad-password-string");
            }
        }, Throws.TypeOf<EncryptionException>());

        /* ----------------------------------------------------------------- */
        ///
        /// Open_PasswordRequired
        ///
        /// <summary>
        /// 間違ったパスワードを入力して PDF ファイルを開こうとするテストを
        /// 実行します。
        /// </summary>
        /// 
        /// <remarks>
        /// 現在、パスワードの入力エラーとそれ以外のエラーの判別が
        /// できないため、未実装となっています。要修正。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        //[Test]
        public void Open_PasswordRequired() => Assert.DoesNotThrow(() =>
        {
            var raised = false;

            using (var reader = new Cube.Pdf.Drawing.DocumentReader())
            {
                reader.PasswordRequired += (s, e) =>
                {
                    raised = true;
                    e.Cancel = true;
                };
                reader.Open(Example("password.pdf"), "bad-password-string");
            }

            Assert.That(raised, Is.True);
        });

        #endregion

        #region File

        /* ----------------------------------------------------------------- */
        ///
        /// File
        ///
        /// <summary>
        /// File オブジェクトの内容を確認します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCaseSource(nameof(FileTestCases))]
        public void File(string filename, string password, PdfFile expected)
        {
            using (var reader = new Cube.Pdf.Drawing.DocumentReader())
            {
                reader.Open(Example(filename), password);
                var actual = reader.File as PdfFile;

                Assert.That(actual.PageCount,  Is.EqualTo(expected.PageCount));
                Assert.That(actual.Password,   Is.EqualTo(expected.Password));
                Assert.That(actual.Resolution, Is.EqualTo(expected.Resolution));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// FileTestCases
        ///
        /// <summary>
        /// File のテストに必要なテストケースを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static IEnumerable<TestCaseData> FileTestCases
        {
            get
            {
                yield return new TestCaseData("rotation.pdf", "",
                    new PdfFile("rotation.pdf", "")
                    {
                        PageCount  = 9,
                        FullAccess = true,
                    });

                yield return new TestCaseData("password.pdf", "password",
                    new PdfFile("password.pdf", "password")
                    {
                        PageCount  = 2,
                        FullAccess = true,
                    });

                yield return new TestCaseData("password-aes256.pdf", "password",
                    new PdfFile("password-aes256.pdf", "password")
                    {
                        PageCount  = 9,
                        FullAccess = true,
                    });
            }
        }

        #endregion

        #region Metadata

        /* ----------------------------------------------------------------- */
        ///
        /// Metadata
        ///
        /// <summary>
        /// Metadata オブジェクトの内容を確認します。
        /// </summary>
        /// 
        /// <remarks>
        /// 要実装
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        //[TestCaseSource(nameof(MetadataTestCases))]
        public void Metadata(string filename, string password, Metadata expected)
        {
            using (var reader = new Cube.Pdf.Drawing.DocumentReader())
            {
                reader.Open(Example(filename), password);
                var actual = reader.Metadata;

                Assert.That(actual.Version,    Is.EqualTo(expected.Version));
                Assert.That(actual.ViewLayout, Is.EqualTo(expected.ViewLayout));
                Assert.That(actual.ViewMode,   Is.EqualTo(expected.ViewMode));
                Assert.That(actual.Title,      Is.EqualTo(expected.Title));
                Assert.That(actual.Author,     Is.EqualTo(expected.Author));
                Assert.That(actual.Subtitle,   Is.EqualTo(expected.Subtitle));
                Assert.That(actual.Keywords,   Is.EqualTo(expected.Keywords));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// MetadataTestCases
        ///
        /// <summary>
        /// Metadata のテストに必要なテストケースを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static IEnumerable<TestCaseData> MetadataTestCases
        {
            get
            {
                yield return new TestCaseData("rotation.pdf", "",
                    new Metadata
                    {
                        Version    = new Version(1, 7, 0, 0),
                        ViewLayout = ViewLayout.TwoPageLeft,
                        ViewMode   = ViewMode.None,
                        Title      = "テスト用文書",
                        Author     = "株式会社キューブ・ソフト",
                        Subtitle   = "Cube.Pdf.Tests",
                        Keywords   = "CubeSoft,PDF,Test",
                    });
            }
        }

        #endregion

        #region Encryption

        /* ----------------------------------------------------------------- */
        ///
        /// Encryption
        ///
        /// <summary>
        /// Encryption オブジェクトの内容を確認します。
        /// </summary>
        /// 
        /// <remarks>
        /// 要実装
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        //[TestCaseSource(nameof(EncryptionTestCases))]
        public void Encryption(string filename, string password, Encryption expected)
        {
            using (var reader = new Cube.Pdf.Drawing.DocumentReader())
            {
                reader.Open(Example(filename), password);
                var actual = reader.Encryption;

                Assert.That(actual.IsEnabled,             Is.EqualTo(expected.IsEnabled));
                Assert.That(actual.IsUserPasswordEnabled, Is.EqualTo(expected.IsUserPasswordEnabled));
                Assert.That(actual.Method,                Is.EqualTo(expected.Method));
                Assert.That(actual.UserPassword,          Is.EqualTo(expected.UserPassword));

                // Permission
                Assert.That(actual.Permission.Accessibility,     Is.EqualTo(expected.Permission.Accessibility));
                Assert.That(actual.Permission.Assemble,          Is.EqualTo(expected.Permission.Assemble));
                Assert.That(actual.Permission.CopyContents,      Is.EqualTo(expected.Permission.CopyContents));
                Assert.That(actual.Permission.FillInFormFields,  Is.EqualTo(expected.Permission.FillInFormFields));
                Assert.That(actual.Permission.ModifyAnnotations, Is.EqualTo(expected.Permission.ModifyAnnotations));
                Assert.That(actual.Permission.ModifyContents,    Is.EqualTo(expected.Permission.ModifyContents));
                Assert.That(actual.Permission.Print,             Is.EqualTo(expected.Permission.Print));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// EncryptionTestCases
        ///
        /// <summary>
        /// Encryption のテストに必要なテストケースを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static IEnumerable<TestCaseData> EncryptionTestCases
        {
            get
            {
                yield return new TestCaseData("password.pdf", "password",
                    new Encryption
                    {
                        Method                = EncryptionMethod.Standard128,
                        IsEnabled             = true,
                        IsUserPasswordEnabled = true,
                        UserPassword          = "view",
                        Permission            = new Permission
                        {
                            Accessibility     = PermissionMethod.Deny,
                            Assemble          = PermissionMethod.Allow,
                            CopyContents      = PermissionMethod.Deny,
                            FillInFormFields  = PermissionMethod.Deny,
                            ModifyAnnotations = PermissionMethod.Deny,
                            ModifyContents    = PermissionMethod.Deny,
                            Print             = PermissionMethod.Allow,
                        }
                    });

                yield return new TestCaseData("password-aes256.pdf", "password",
                    new Encryption
                    {
                        Method                = EncryptionMethod.Aes256,
                        IsEnabled             = true,
                        IsUserPasswordEnabled = false, // true
                        UserPassword          = "", // "view"
                        Permission            = new Permission
                        {
                            Accessibility     = PermissionMethod.Allow,
                            Assemble          = PermissionMethod.Allow,
                            CopyContents      = PermissionMethod.Allow,
                            FillInFormFields  = PermissionMethod.Allow,
                            ModifyAnnotations = PermissionMethod.Allow,
                            ModifyContents    = PermissionMethod.Allow,
                            Print             = PermissionMethod.Allow,
                        }
                    });
            }
        }

        #endregion

        #region Pages

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        ///
        /// <summary>
        /// Pages オブジェクトの内容を確認します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("rotation.pdf",        "",         ExpectedResult = 9)]
        [TestCase("password.pdf",        "password", ExpectedResult = 2)]
        [TestCase("password.pdf",        "view",     ExpectedResult = 2)]
        [TestCase("password-aes256.pdf", "password", ExpectedResult = 9)]
        public int Pages(string filename, string password)
        {
            using (var reader = new Cube.Pdf.Drawing.DocumentReader())
            {
                reader.Open(Example(filename), password);
                return reader.Pages.Count();
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetPage_Size
        ///
        /// <summary>
        /// 各ページのサイズ情報を確認します。
        /// </summary>
        /// 
        /// <remarks>
        /// ページを回転している場合、回転後のサイズが返されている。
        /// Editing.DocumentReader() では元のサイズとなっているが、
        /// この辺りの整合性をどうするか要検討。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("rotation.pdf", 1, 595, 842)]
        [TestCase("rotation.pdf", 2, 842, 595)]
        public void GetPage_Size(string filename, int n, int width, int height)
        {
            using (var reader = new Cube.Pdf.Drawing.DocumentReader())
            {
                reader.Open(Example(filename));
                var actual = reader.GetPage(n);
                Assert.That(actual.Size, Is.EqualTo(new Size(width, height)));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetPage_Resolution
        ///
        /// <summary>
        /// 各ページの解像度情報を確認します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("rotation.pdf", 1, ExpectedResult = 72)]
        public int GetPage_Resolution(string filename, int n)
        {
            using (var reader = new Cube.Pdf.Drawing.DocumentReader())
            {
                reader.Open(Example(filename));
                return reader.GetPage(n).Resolution.X;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetPage_Rotation
        ///
        /// <summary>
        /// 各ページの回転角情報を確認します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("rotation.pdf", 1, ExpectedResult =   0)]
        [TestCase("rotation.pdf", 2, ExpectedResult =  90)]
        [TestCase("rotation.pdf", 3, ExpectedResult = 180)]
        [TestCase("rotation.pdf", 4, ExpectedResult = 270)]
        [TestCase("rotation.pdf", 5, ExpectedResult =   0)]
        public int GetPage_Rotation(string filename, int n)
        {
            using (var reader = new Cube.Pdf.Drawing.DocumentReader())
            {
                reader.Open(Example(filename));
                return reader.GetPage(n).Rotation;
            }
        }

        #endregion

        #region CreateImage

        /* ----------------------------------------------------------------- */
        ///
        /// CreateImage
        ///
        /// <summary>
        /// Image オブジェクトを生成するテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("rotation.pdf", "", 1)]
        [TestCase("rotation.pdf", "", 2)]
        [TestCase("rotation.pdf", "", 3)]
        [TestCase("rotation.pdf", "", 4)]
        public void CreateImage(string filename, string password, int pagenum)
        {
            using (var reader = new Cube.Pdf.Drawing.DocumentReader())
            {
                reader.Open(Example(filename), password);
                var page = reader.GetPage(pagenum);

                using (var image = reader.CreateImage(pagenum, 1.0))
                {
                    var dest = Result($"CreateImage-{pagenum}.png");
                    image.Save(dest);
                    Assert.That(System.IO.File.Exists(dest), Is.True);
                }
            }
        }

        #endregion
    }
}
