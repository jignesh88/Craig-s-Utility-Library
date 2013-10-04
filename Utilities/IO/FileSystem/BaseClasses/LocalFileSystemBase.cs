﻿/*
Copyright (c) 2013 <a href="http://www.gutgames.com">James Craig</a>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

#region Usings
using System;
using System.Text.RegularExpressions;
using Utilities.DataTypes.Patterns.BaseClasses;
using Utilities.IO.FileSystem.Default;
using Utilities.IO.FileSystem.Interfaces;

#endregion

namespace Utilities.IO.FileSystem.BaseClasses
{
    /// <summary>
    /// Local file system base class
    /// </summary>
    public abstract class LocalFileSystemBase : FileSystemBase, IFileSystem
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        protected LocalFileSystemBase()
            : base()
        {
        }

        #endregion

        #region Functions

        /// <summary>
        /// Gets the class representation for the file
        /// </summary>
        /// <param name="Path">Path to the file</param>
        /// <returns>The file object</returns>
        public override IFile File(string Path)
        {
            Path = AbsolutePath(Path);
            return new LocalFile(Path);
        }

        /// <summary>
        /// Gets the directory representation for the directory
        /// </summary>
        /// <param name="Path">Path to the directory</param>
        /// <returns>The directory object</returns>
        public override IDirectory Directory(string Path)
        {
            Path = AbsolutePath(Path);
            return new LocalDirectory(Path.RemoveIllegalDirectoryNameCharacters());
        }

        /// <summary>
        /// Function to override in order to dispose objects
        /// </summary>
        /// <param name="Managed">If true, managed and unmanaged objects should be disposed. Otherwise unmanaged objects only.</param>
        protected override void Dispose(bool Managed)
        {

        }

        #endregion
    }
}