﻿// <copyright file="SymmetricDenseMatrixTests.cs" company="Math.NET">
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

namespace MathNet.Numerics.UnitTests.LinearAlgebraTests.Single
{
    using System;
    using System.Collections.Generic;

    using MathNet.Numerics.LinearAlgebra.Single;

    using NUnit.Framework;

    /// <summary>
    /// Symmetric Dense matrix tests.
    /// </summary>
    public class SymmetricDenseMatrixTests : SymmetricMatrixTests
    {
        /// <summary>
        /// Creates a matrix for the given number of rows and columns.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        /// <returns>
        /// A matrix with the given dimensions.
        /// </returns>
        protected override Matrix CreateMatrix(int rows, int columns)
        {
            return new DenseMatrix(rows, columns);
        }

        /// <summary>
        /// Creates a matrix from a 2D array.
        /// </summary>
        /// <param name="data">
        /// The 2D array to create this matrix from.
        /// </param>
        /// <returns>
        /// A matrix with the given values.
        /// </returns>
        protected override Matrix CreateMatrix(float[,] data)
        {
            if (SymmetricMatrix.CheckIfSymmetric(data))
            {
                return new SymmetricDenseMatrix(data);
            }

            return new DenseMatrix(data);
        }

        /// <summary>
        /// Creates a vector of the given size.
        /// </summary>
        /// <param name="size">
        /// The size of the vector to create.
        /// </param>
        /// <returns>
        /// The new vector. 
        /// </returns>
        protected override Vector CreateVector(int size)
        {
            return new DenseVector(size);
        }

        /// <summary>
        /// Creates a vector from an array.
        /// </summary>
        /// <param name="data">
        /// The array to create this vector from.
        /// </param>
        /// <returns>
        /// The new vector. 
        /// </returns>
        protected override Vector CreateVector(float[] data)
        {
            return new DenseVector(data);
        }

        /// <summary>
        /// Can create a matrix form array.
        /// </summary>
        [Test]
        public void CanCreateMatrixFrom1DArray()
        {
            var testData = new Dictionary<string, Matrix>
                           {
                             { "Singular3x3", new SymmetricDenseMatrix(3, new[] { 1.0f, 2.0f, 0.0f, 3.0f, 0.0f, 0.0f }) }, 
                             { "Square3x3", new SymmetricDenseMatrix(3, new[] { -1.1f, 2.0f, 1.1f, 3.0f, 0.0f, 6.6f }) }, 
                             { "Square4x4", new SymmetricDenseMatrix(4, new[] { 1.1f, 2.0f, 5.0f, -3.0f, -6.0f, 8.0f, 4.4f, 7.0f, 9.0f, 10.0f }) }, 
                             { "Singular4x4", new SymmetricDenseMatrix(4, new[] { 1.0f, 2.0f, 5.0f, 0.0f, 0.0f, 0.0f, 4.0f, 7.0f, 0.0f, 10.0f }) }, 
                             { "Symmetric3x3", new SymmetricDenseMatrix(3, new[] { 1.0f, 2.0f, 2.0f, 3.0f, 0.0f, 3.0f }) },
                             { "IndexTester4x4", new SymmetricDenseMatrix(4, new [] { 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f }) }
                           };

            foreach (var name in testData.Keys)
            {
                Assert.AreEqual(TestMatrices[name], testData[name]);
            }
        }

        /// <summary>
        /// Matrix from array is a reference.
        /// </summary>
        [Test]
        public void MatrixFrom1DArrayIsReference()
        {
            var data = new float[] { 1, 1, 1, 1, 1, 1 };
            var matrix = new SymmetricDenseMatrix(3, data);
            matrix[0, 0] = 10.0f;
            Assert.AreEqual(10.0f, data[0]);
        }

        /// <summary>
        /// Can create a matrix form array.
        /// </summary>
        [Test]
        public void CanCreateMatrixFrom2DArray()
        {
            var testData = new Dictionary<string, Matrix>
                          {
                             { "Singular3x3", new SymmetricDenseMatrix(new[,] { { 1.0f, 2.0f, 3.0f }, { 2.0f, 0.0f, 0.0f }, { 3.0f, 0.0f, 0.0f } }) }, 
                             { "Square3x3", new SymmetricDenseMatrix(new[,] { { -1.1f, 2.0f, 3.0f }, { 2.0f, 1.1f, 0.0f }, { 3.0f, 0.0f, 6.6f } }) }, 
                             { "Square4x4", new SymmetricDenseMatrix(new[,] { { 1.1f, 2.0f, -3.0f, 4.4f }, { 2.0f, 5.0f, -6.0f, 7.0f }, { -3.0f, -6.0f, 8.0f, 9.0f }, { 4.4f, 7.0f, 9.0f, 10.0f } }) }, 
                             { "Singular4x4", new SymmetricDenseMatrix(new[,] { { 1.0f, 2.0f, 0.0f, 4.0f }, { 2.0f, 5.0f, 0.0f, 7.0f }, { 0.0f, 0.0f, 0.0f, 0.0f }, { 4.0f, 7.0f, 0.0f, 10.0f } }) }, 
                             { "Symmetric3x3", new SymmetricDenseMatrix(new[,] { { 1.0f, 2.0f, 3.0f }, { 2.0f, 2.0f, 0.0f }, { 3.0f, 0.0f, 3.0f } }) },
                             { "IndexTester4x4", new SymmetricDenseMatrix(new [,] { { 0f, 1f, 3f, 6f }, { 1f, 2f, 4f, 7f }, { 3f, 4f, 5f, 8f }, { 6f, 7f, 8f, 9f } }) }
                         };

            foreach (var name in testData.Keys)
            {
                Assert.AreEqual(TestMatrices[name], testData[name]);
            }
        }

        /// <summary>
        /// Matrix from two-dimensional array is a copy.
        /// </summary>
        [Test]
        public void MatrixFrom2DArrayIsCopy()
        {
            var matrix = new DenseMatrix(TestData2D["Singular3x3"]);
            matrix[0, 0] = 10.0f;
            Assert.AreEqual(1.0f, TestData2D["Singular3x3"][0, 0]);
        }

        /// <summary>
        /// Can create a matrix with uniform values.
        /// </summary>
        [Test]
        public void CanCreateMatrixWithUniformValues()
        {
            var matrix = new SymmetricDenseMatrix(10, 10.0f);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(matrix[i, j], 10.0f);
                }
            }
        }

        /// <summary>
        /// Can create an identity matrix.
        /// </summary>
        [Test]
        public void CanCreateIdentity()
        {
            var matrix = SymmetricDenseMatrix.Identity(5);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(i == j ? 1.0f : 0.0f, matrix[i, j]);
                }
            }
        }

        /// <summary>
        /// Identity with wrong order throws <c>ArgumentOutOfRangeException</c>.
        /// </summary>
        /// <param name="order">The size of the square matrix</param>
        [TestCase(0)]
        [TestCase(-1)]
        public void IdentityWithWrongOrderThrowsArgumentOutOfRangeException(int order)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SymmetricDenseMatrix.Identity(order));
        }
    }
}