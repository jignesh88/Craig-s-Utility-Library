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
    /// File system base class
    /// </summary>
    public abstract class FileSystemBase : SafeDisposableBaseClass, IFileSystem
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        protected FileSystemBase()
            : base()
        {
            HandleRegex = new Regex(HandleRegexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Regex string used to determine if the file system can handle the path
        /// </summary>
        protected abstract string HandleRegexString { get; }

        /// <summary>
        /// Regex used to determine if the file system can handle the path
        /// </summary>
        protected Regex HandleRegex { get; private set; }

        /// <summary>
        /// Name of the file system
        /// </summary>
        public abstract string Name { get; }

        #endregion

        #region Functions

        /// <summary>
        /// Gets the class representation for the file
        /// </summary>
        /// <param name="Path">Path to the file</param>
        /// <returns>The file object</returns>
        public abstract IFile File(string Path);

        /// <summary>
        /// Gets the directory representation for the directory
        /// </summary>
        /// <param name="Path">Path to the directory</param>
        /// <returns>The directory object</returns>
        public abstract IDirectory Directory(string Path);

        /// <summary>
        /// Gets the absolute path of the variable passed in
        /// </summary>
        /// <param name="Path">Path to convert to absolute</param>
        /// <returns>The absolute path of the path passed in</returns>
        protected abstract string AbsolutePath(string Path);

        /// <summary>
        /// Returns true if it can handle the path, false otherwise
        /// </summary>
        /// <param name="Path">The path to check against</param>
        /// <returns>True if it can handle the path, false otherwise</returns>
        public bool CanHandle(string Path)
        {
            return HandleRegex.IsMatch(Path);
        }

        #endregion
    }
}