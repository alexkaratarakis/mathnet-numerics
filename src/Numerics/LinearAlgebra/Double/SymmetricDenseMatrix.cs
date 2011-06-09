﻿// <copyright file="SymmetricDenseMatrix.cs" company="Math.NET">
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

namespace MathNet.Numerics.LinearAlgebra.Double
{
    using System;
    using Distributions;
    using Generic;
    using Generic.StorageSchemes.Static;
    using Properties;
    using Threading;

    /// <summary>
    /// A Symmetric Matrix class with dense storage. 
    /// </summary>
    /// <remarks> The underlying storage is a one dimensional array in column-major order.
    /// The Upper Triangle is stored(it is equal to the Lower Triangle) </remarks>
    [Serializable]
    public class SymmetricDenseMatrix : SymmetricMatrix
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricDenseMatrix"/> class. This matrix is square with a given size.
        /// </summary>
        /// <param name="order">The size of the square matrix.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="order"/> is less than one.
        /// </exception>
        public SymmetricDenseMatrix(int order)
            : base(order)
        {
            Indexer = new PackedStorageSchemeUpper(order);
            Data = new double[Indexer.DataLength];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricDenseMatrix"/> class.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        /// /// <exception cref="ArgumentException">
        /// If <paramref name="rows"/> not equal to <paramref name="columns"/>.
        /// </exception>
        public SymmetricDenseMatrix(int rows, int columns)
            : base(rows, columns)
        {
            Indexer = new PackedStorageSchemeUpper(rows);
            Data = new double[Indexer.DataLength];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricDenseMatrix"/> class with all entries set to a particular value.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        /// <param name="value">The value which we assign to each element of the matrix.</param>
        /// <remarks> Forcing user to input (int, int, double) because only asking (int, double) would 
        /// create a signature easily confused with (int, int) which is already used </remarks>
        public SymmetricDenseMatrix(int rows, int columns, double value)
            : this(rows, columns)
        {
            for (var i = 0; i < Data.Length; i++)
            {
                Data[i] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricDenseMatrix"/> class from a one dimensional array. This constructor
        /// will reference the one dimensional array and not copy it.
        /// </summary>
        /// <param name="order">The size of the square matrix.</param>
        /// <param name="array">
        /// The one dimensional array to create this matrix from. Column-major and row-major order is identical on a symmetric matrix: http://en.wikipedia.org/wiki/Row-major_order 
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="array"/> does not represent a packed array.
        /// </exception>
        public SymmetricDenseMatrix(int order, double[] array)
            : this(order)
        {
            Data = array;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricDenseMatrix"/> class from a 2D array. This constructor
        /// will allocate a completely new memory block for storing the symmetric dense matrix.
        /// </summary>
        /// <param name="array">The 2D array to create this matrix from.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="array"/> is not a square array.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="array"/> is not a symmetric array.
        /// </exception>
        public SymmetricDenseMatrix(double[,] array)
            : this(array.GetLength(0), array.GetLength(1))
        {
            if (!CheckIfSymmetric(array))
            {
                throw new ArgumentException(Resources.ArgumentMatrixSymmetric);
            }

            Indexer = new PackedStorageSchemeUpper(Order);
            for (var row = 0; row < Order; row++)
            {
                for (var column = row; column < Order; column++)
                {
                    Data[Indexer.IndexOf(row, column)] = array[row, column];
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricDenseMatrix"/> class, copying
        /// the values from the given matrix. Matrix must be Symmetric.
        /// </summary>
        /// <param name="matrix">The matrix to copy.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="matrix"/> is not a square matrix.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="matrix"/> is not a symmetric matrix.
        /// </exception>
        public SymmetricDenseMatrix(Matrix<double> matrix)
            : base(matrix.RowCount, matrix.ColumnCount)
        {
            var symmetricMatrix = matrix as SymmetricDenseMatrix;

            if (!matrix.IsSymmetric)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSymmetric);
            }

            if (symmetricMatrix == null)
            {
                for (var row = 0; row < Order; row++)
                {
                    for (var column = row; column < Order; column++)
                    {
                        Data[Indexer.IndexOf(row, column)] = matrix[row, column];
                    }
                }
            }
            else
            {
                matrix.CopyTo(this);
            }
        }

        /// <summary>
        /// Gets the matrix's data in indexed format.
        /// </summary>
        /// <value>The matrix's indexed data.</value>
        public PackedStorageSchemeUpper Indexer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the matrix's data in array format. 
        /// </summary>
        /// <value>The matrix's raw data.</value>
        public double[] Data
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a <c>SymmetricDenseMatrix</c> for the given number of rows and columns. 
        /// If rows and columns are not equal, returns a <c>DenseMatrix</c> instead. 
        /// </summary>
        /// <param name="numberOfRows">
        /// The number of rows.
        /// </param>
        /// <param name="numberOfColumns">
        /// The number of columns.
        /// </param>
        /// <returns>
        /// A <c>DenseMatrix</c> or <c>SymmetricDenseMatrix</c> with the given dimensions.
        /// </returns>
        /// /// <exception cref="ArgumentException">
        /// If <paramref name="numberOfRows"/> is not equal to <paramref name="numberOfColumns"/>. 
        /// Symmetric arrays are always square
        /// </exception>
        public override Matrix<double> CreateMatrix(int numberOfRows, int numberOfColumns)
        {
            if (numberOfRows != numberOfColumns)
            {
                return new DenseMatrix(numberOfRows, numberOfColumns);
            }

            return new SymmetricDenseMatrix(numberOfRows, numberOfColumns);
        }

        /// <summary>
        /// Creates a <see cref="Vector{T}"/> with a the given dimension.
        /// </summary>
        /// <param name="size">The size of the vector.</param>
        /// <returns>
        /// A <see cref="Vector{T}"/> with the given dimension.
        /// </returns>
        public override Vector<double> CreateVector(int size)
        {
            return new DenseVector(size);
        }

        #region IExtraAccessors<T> Members
        /// <summary>
        /// Retrieves the requested element without range checking. 
        /// </summary>
        /// <param name="row">
        /// The row of the element.
        /// </param>
        /// <param name="column">
        /// The column of the element.
        /// </param>
        /// <returns>
        /// The requested element.
        /// </returns>
        public override double At(int row, int column)
        {
            var r = Math.Min(row, column);
            var c = Math.Max(row, column);
            return Data[Indexer.IndexOf(r, c)];
        }

        /// <summary>
        /// Sets the value of the given element.
        /// </summary>
        /// <param name="row">
        /// The row of the element.
        /// </param>
        /// <param name="column">
        /// The column of the element.
        /// </param>
        /// <param name="value">
        /// The value to set the element to.
        /// </param>
        public override void At(int row, int column, double value)
        {
            var r = Math.Min(row, column);
            var c = Math.Max(row, column);
            Data[Indexer.IndexOf(r, c)] = value;
        }

        /// <summary>
        /// Retrieves the requested element without range checking. 
        /// CAUTION:
        /// This method assumes that you request an element from the upper triangle (row less than or equal to column).  
        /// If not, the result is completely wrong.  
        /// </summary>
        /// <param name="row">
        /// The row of the element. Must be less than or equal to column. 
        /// </param>
        /// <param name="column">
        /// The column of the element. Must be more than or equal to row. 
        /// </param>
        /// <returns>
        /// The requested element from the upper triangle.
        /// </returns>
        public override double AtUpper(int row, int column)
        {
            return Data[Indexer.IndexOf(row, column)];
        }

        /// <summary>
        /// Sets the value of the given element.
        /// CAUTION:
        /// This method assumes that you set an element from the upper triangle (row less than or equal to column).
        /// If not, the result is completely wrong. 
        /// </summary>
        /// <param name="row">
        /// The row of the element. Must be less than or equal to column.
        /// </param>
        /// <param name="column">
        /// The column of the element. Must be more than or equal to row. 
        /// </param>
        /// <param name="value">
        /// The value on the upper triangle to set the element to.
        /// </param>
        public override void AtUpper(int row, int column, double value)
        {
            Data[Indexer.IndexOf(row, column)] = value;
        }

        /// <summary>
        /// Retrieves the requested element without range checking. 
        /// CAUTION:
        /// This method assumes that you request an element from the lower triangle (row greater than or equal to column).  
        /// </summary>
        /// <param name="row">
        /// The row of the element. Must be more than or equal to column. 
        /// </param>
        /// <param name="column">
        /// The column of the element. Must be less than or equal to row. 
        /// </param>
        /// <returns>
        /// The requested element from the lower triangle.
        /// </returns>
        public override double AtLower(int row, int column)
        {
            return Data[Indexer.IndexOf(column, row)];
        }

        /// <summary>
        /// Sets the value of the given element.
        /// CAUTION:
        /// This method assumes that you set an element from the lower triangle (row greater than or equal to column).
        /// If not, the result is completely wrong. 
        /// </summary>
        /// <param name="row">
        /// The row of the element. Must be more than or equal to column
        /// </param>
        /// <param name="column">
        /// The column of the element. Must be less than or equal to row. 
        /// </param>
        /// <param name="value">
        /// The value on the lower triangle to set the element to.
        /// </param>
        public override void AtLower(int row, int column, double value)
        {
            Data[Indexer.IndexOf(column, row)] = value;
        }

        /// <summary>
        /// Retrieves the requested element without range checking. 
        /// </summary>
        /// <param name="row">
        /// The row=column of the diagonal element.
        /// </param>
        /// <returns>
        /// The requested element.
        /// </returns>
        public override double AtDiagonal(int row)
        {
            return Data[Indexer.IndexOfDiagonal(row)];
        }

        /// <summary>
        /// Sets the value of the given element.
        /// </summary>
        /// <param name="row">
        /// The row=column of the diagonal element.
        /// </param>
        /// <param name="value">
        /// The value to set the element to.
        /// </param>
        public override void AtDiagonal(int row, double value)
        {
            Data[Indexer.IndexOfDiagonal(row)] = value;
        }
        #endregion

        /// <summary>
        /// Sets all values to zero.
        /// </summary>
        public override void Clear()
        {
            Array.Clear(Data, 0, Data.Length);
        }
     
        #region Static constructors for special matrices.

        /// <summary>
        /// Initializes a square <see cref="SymmetricDenseMatrix"/> with all zero's except for ones on the diagonal.
        /// </summary>
        /// <param name="order">the size of the square matrix.</param>
        /// <returns>A symmetric dense identity matrix.</returns>
        /// <exception cref="ArgumentException">
        /// If <paramref name="order"/> is less than one.
        /// </exception>
        public static SymmetricDenseMatrix Identity(int order)
        {
            var m = new SymmetricDenseMatrix(order);
            for (var i = 0; i < order; i++)
            {
                m.AtDiagonal(i, 1.0);
            }

            return m;
        }

        #endregion

        /// <summary>
        /// Adds another matrix to this matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this matrix.</param>
        /// <param name="result">The matrix to store the result of add</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoAdd(Matrix<double> other, Matrix<double> result)
        {
            var denseOther = other as SymmetricDenseMatrix;
            var denseResult = result as SymmetricDenseMatrix;
            if (denseOther == null || denseResult == null)
            {
                base.DoAdd(other, result);
            }
            else
            {
                Control.LinearAlgebraProvider.AddArrays(Data, denseOther.Data, denseResult.Data);
            }
        }

        /// <summary>
        /// Subtracts another matrix from this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract.</param>
        /// <param name="result">The matrix to store the result of the subtraction.</param>
        protected override void DoSubtract(Matrix<double> other, Matrix<double> result)
        {
            var denseOther = other as SymmetricDenseMatrix;
            var denseResult = result as SymmetricDenseMatrix;
            if (denseOther == null || denseResult == null)
            {
                base.DoSubtract(other, result);
            }
            else
            {
                Control.LinearAlgebraProvider.SubtractArrays(Data, denseOther.Data, denseResult.Data);
            }
        }

        /// <summary>
        /// Multiplies each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="scalar">The scalar to multiply the matrix with.</param>
        /// <param name="result">The matrix to store the result of the multiplication.</param>
        protected override void DoMultiply(double scalar, Matrix<double> result)
        {
            var denseResult = result as SymmetricDenseMatrix;
            if (denseResult == null)
            {
                base.DoMultiply(scalar, result);
            }
            else
            {
                Control.LinearAlgebraProvider.ScaleArray(scalar, Data, denseResult.Data);
            }
        }

        /// <summary>
        /// Multiplies this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Vector<double> rightSide, Vector<double> result)
        {
            var denseRight = rightSide as DenseVector;
            var denseResult = result as DenseVector;

            if (denseRight == null || denseResult == null)
            {
                base.DoMultiply(rightSide, result);
            }
            else
            {
                // TODO: Change this when symmetric methods are implemented in the Linear Algebra Providers. 
                base.DoMultiply(rightSide, result);
            }
        }

        /// <summary>
        /// Left multiply a matrix with a vector ( = vector * matrix ) and place the result in the result vector.
        /// </summary>
        /// <param name="leftSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoLeftMultiply(Vector<double> leftSide, Vector<double> result)
        {
            var denseLeft = leftSide as DenseVector;
            var denseResult = result as DenseVector;

            if (denseLeft == null || denseResult == null)
            {
                base.DoLeftMultiply(leftSide, result);
            }
            else
            {
                // TODO: Change this when symmetric methods are implemented in the Linear Algebra Providers.
                base.DoLeftMultiply(leftSide, result);
            }
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Matrix<double> other, Matrix<double> result)
        {
            var denseOther = other as SymmetricDenseMatrix;
            var denseResult = result as SymmetricDenseMatrix;

            if (denseOther == null || denseResult == null)
            {
                base.DoMultiply(other, result);
            }
            else
            {
                // TODO: Change this when symmetric methods are implemented in the Linear Algebra Providers.
                base.DoMultiply(other, result);
            }
        }

        /// <summary>
        /// Multiplies this matrix with transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeAndMultiply(Matrix<double> other, Matrix<double> result)
        {
            var denseOther = other as SymmetricDenseMatrix;
            var denseResult = result as SymmetricDenseMatrix;

            if (denseOther == null || denseResult == null)
            {
                base.DoTransposeAndMultiply(other, result);
            }
            else
            {
                // TODO: Change this when symmetric methods are implemented in the Linear Algebra Providers.
                base.DoTransposeAndMultiply(other, result);
            }
        }

        /// <summary>
        /// Negate each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the negation.</param>
        protected override void DoNegate(Matrix<double> result)
        {
            var denseResult = result as SymmetricDenseMatrix;

            if (denseResult == null)
            {
                base.DoNegate(result);
            }
            else
            {
                Control.LinearAlgebraProvider.ScaleArray(-1, Data, denseResult.Data);
            }
        }

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(Matrix<double> other, Matrix<double> result)
        {
            var denseOther = other as SymmetricDenseMatrix;
            var denseResult = result as SymmetricDenseMatrix;

            if (denseOther == null || denseResult == null)
            {
                base.DoPointwiseMultiply(other, result);
            }
            else
            {
                Control.LinearAlgebraProvider.PointWiseMultiplyArrays(Data, denseOther.Data, denseResult.Data);
            }
        }

        /// <summary>
        /// Pointwise divide this matrix by another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise divide this one by.</param>
        /// <param name="result">The matrix to store the result of the pointwise division.</param>
        protected override void DoPointwiseDivide(Matrix<double> other, Matrix<double> result)
        {
            var denseOther = other as SymmetricDenseMatrix;
            var denseResult = result as SymmetricDenseMatrix;

            if (denseOther == null || denseResult == null)
            {
                base.DoPointwiseDivide(other, result);
            }
            else
            {
                Control.LinearAlgebraProvider.PointWiseDivideArrays(Data, denseOther.Data, denseResult.Data);
            }
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>  
        public override Matrix<double> LowerTriangle()
        {
            var ret = new DenseMatrix(Order);
            for (var row = 0; row < Order; row++)
            {
                for (var column = 0; column <= row; column++)
                {
                    ret[row, column] = AtLower(row, column);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix. The new matrix
        /// does not contain the diagonal elements of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>
        public override Matrix<double> StrictlyLowerTriangle()
        {
            var ret = new DenseMatrix(Order);
            for (var row = 0; row < Order; row++)
            {
                for (var column = 0; column < row; column++)
                {
                    ret[row, column] = AtLower(row, column);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>   
        public override Matrix<double> UpperTriangle()
        {
            var ret = new DenseMatrix(Order);
            for (var row = 0; row < Order; row++)
            {
                for (var column = row; column < Order; column++)
                {
                    ret[row, column] = AtUpper(row, column);
                }
            }

            return ret;
        }
        
        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix. The new matrix
        /// does not contain the diagonal elements of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>
        public override Matrix<double> StrictlyUpperTriangle()
        {
            var ret = new DenseMatrix(Order);
            for (var row = 0; row < Order; row++)
            {
                for (var column = row + 1; column < Order; column++)
                {
                    ret[row, column] = AtUpper(row, column);
                }
            }

            return ret;
        }

        /// <summary>
        /// Computes the modulus for each element of the matrix.
        /// </summary>
        /// <param name="divisor">The divisor to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected override void DoModulus(double divisor, Matrix<double> result)
        {
            var denseResult = result as SymmetricDenseMatrix;

            if (denseResult == null)
            {
                base.DoModulus(divisor, result);
            }
            else
            {
                if (!ReferenceEquals(this, result))
                {
                    CopyTo(result);
                }

                CommonParallel.For(
                    0,
                    Data.Length,
                    index => denseResult.Data[index] %= divisor);
            }
        }

        /// <summary>
        /// Returns the conjugate transpose of this matrix.
        /// </summary>        
        /// <returns>The conjugate transpose of this matrix.</returns>
        public override Matrix<double> ConjugateTranspose()
        {
            return Transpose();
        }

        /// <summary>
        /// Computes the trace of this matrix.
        /// </summary>
        /// <returns>The trace of this matrix</returns>
        /// <exception cref="ArgumentException">If the matrix is not square</exception>
        public override double Trace()
        {
            if (RowCount != ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSquare);
            }

            var sum = 0.0;
            for (var i = 0; i < RowCount; i++)
            {
                sum += AtDiagonal(i);
            }

            return sum;
        }

        /// <summary>
        /// Populates a symmetric matrix with random elements.
        /// </summary>
        /// <param name="matrix">The symmetric matrix to populate.</param>
        /// <param name="distribution">Continuous Random Distribution to generate elements from.</param>
        protected override void DoRandom(Matrix<double> matrix, IContinuousDistribution distribution)
        {
            var denseMatrix = matrix as SymmetricDenseMatrix;

            if (denseMatrix == null)
            {
                base.DoRandom(matrix, distribution);
            }
            else
            {
                for (var i = 0; i < denseMatrix.Data.Length; i++)
                {
                    denseMatrix.Data[i] = distribution.Sample();
                }
            }
        }

        /// <summary>
        /// Populates a symmetric matrix with random elements.
        /// </summary>
        /// <param name="matrix">The symmetric matrix to populate.</param>
        /// <param name="distribution">Continuous Random Distribution to generate elements from.</param>
        protected override void DoRandom(Matrix<double> matrix, IDiscreteDistribution distribution)
        {
            var denseMatrix = matrix as SymmetricDenseMatrix;

            if (denseMatrix == null)
            {
                base.DoRandom(matrix, distribution);
            }
            else
            {
                for (var i = 0; i < denseMatrix.Data.Length; i++)
                {
                    denseMatrix.Data[i] = distribution.Sample();
                }
            }
        }

        /// <summary>
        /// Adds two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to add.</param>
        /// <param name="rightSide">The right matrix to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SymmetricDenseMatrix operator +(SymmetricDenseMatrix leftSide, SymmetricDenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide.RowCount != rightSide.RowCount)
            {
                throw new ArgumentOutOfRangeException(Resources.ArgumentMatrixDimensions);
            }

            return (SymmetricDenseMatrix)leftSide.Add(rightSide);
        }

        /// <summary>
        /// Returns a <strong>Matrix</strong> containing the same values of <paramref name="rightSide"/>. 
        /// </summary>
        /// <param name="rightSide">The matrix to get the values from.</param>
        /// <returns>A matrix containing a the same values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SymmetricDenseMatrix operator +(SymmetricDenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (SymmetricDenseMatrix)rightSide.Clone();
        }

        /// <summary>
        /// Subtracts two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to subtract.</param>
        /// <param name="rightSide">The right matrix to subtract.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SymmetricDenseMatrix operator -(SymmetricDenseMatrix leftSide, SymmetricDenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide.RowCount != rightSide.RowCount)
            {
                throw new ArgumentOutOfRangeException(Resources.ArgumentMatrixDimensions);
            }

            return (SymmetricDenseMatrix)leftSide.Subtract(rightSide);
        }

        /// <summary>
        /// Negates each element of the matrix.
        /// </summary>
        /// <param name="rightSide">The matrix to negate.</param>
        /// <returns>A matrix containing the negated values.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SymmetricDenseMatrix operator -(SymmetricDenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (SymmetricDenseMatrix)rightSide.Negate();
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static SymmetricDenseMatrix operator *(SymmetricDenseMatrix leftSide, double rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return (SymmetricDenseMatrix)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SymmetricDenseMatrix operator *(double leftSide, SymmetricDenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (SymmetricDenseMatrix)rightSide.Multiply(leftSide);
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to multiply.</param>
        /// <param name="rightSide">The right matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the dimensions of <paramref name="leftSide"/> or <paramref name="rightSide"/> don't conform.</exception>
        public static SymmetricDenseMatrix operator *(SymmetricDenseMatrix leftSide, SymmetricDenseMatrix rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide.ColumnCount != rightSide.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            return (SymmetricDenseMatrix)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> and a Vector.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The vector to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator *(SymmetricDenseMatrix leftSide, DenseVector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return (DenseVector)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a Vector and a <strong>Matrix</strong>.
        /// </summary>
        /// <param name="leftSide">The vector to multiply.</param>
        /// <param name="rightSide">The matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator *(DenseVector leftSide, SymmetricDenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (DenseVector)rightSide.LeftMultiply(leftSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static SymmetricDenseMatrix operator %(SymmetricDenseMatrix leftSide, double rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return (SymmetricDenseMatrix)leftSide.Modulus(rightSide);
        }
    }
}