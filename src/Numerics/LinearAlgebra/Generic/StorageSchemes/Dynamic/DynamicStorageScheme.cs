﻿// <copyright file="DynamicStorageScheme.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
// Copyright (c) 2009-2010 Math.NET
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
namespace MathNet.Numerics.LinearAlgebra.Generic.StorageSchemes.Dynamic
{
    using System;

    /// <summary>
    ///   Classes that completely manage dynamic storage scheme.
    /// </summary>
    /// <remarks>
    ///   A dynamic storage scheme is dependent on the actual matrix and therefore different for
    ///   every matrix. This category of storage scheme needs to manage the data array.
    /// </remarks>
    /// <typeparam name = "T">Supported data types are double, single, <see cref = "Complex" />, and <see cref = "Complex32" />.</typeparam>
    public abstract class DynamicStorageScheme<T> : StorageScheme
        where T : struct, IEquatable<T>, IFormattable
    {
        /// <summary>
        ///    Gets the array containing the stored elements.
        ///  </summary>
        public abstract T[] Data
        {
            get;
        }
    }
}