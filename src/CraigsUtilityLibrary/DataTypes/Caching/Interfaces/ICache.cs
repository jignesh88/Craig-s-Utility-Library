﻿/*
Copyright (c) 2014 <a href="http://www.gutgames.com">James Craig</a>

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

using System;
using System.Collections.Generic;

namespace Utilities.DataTypes.Caching.Interfaces
{
    /// <summary>
    /// Cache interface
    /// </summary>
    public interface ICache : IDictionary<string, object>, IDisposable
    {
        /// <summary>
        /// Cache name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The tags used thus far
        /// </summary>
        IEnumerable<string> Tags { get; }

        /// <summary>
        /// Adds a value/key combination and assigns tags to it
        /// </summary>
        /// <param name="key">Key to add</param>
        /// <param name="tags">Tags to associate with the key/value pair</param>
        /// <param name="value">Value to add</param>
        void Add(string key, object value, IEnumerable<string> tags);

        /// <summary>
        /// Gets the objects associated with a specific tag
        /// </summary>
        /// <param name="tag">Tag to use</param>
        /// <returns>The objects associated with the tag</returns>
        IEnumerable<object> GetByTag(string tag);

        /// <summary>
        /// Removes all items associated with the tag specified
        /// </summary>
        /// <param name="tag">Tag to remove</param>
        void RemoveByTag(string tag);
    }
}