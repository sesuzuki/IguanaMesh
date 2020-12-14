/*
 * <Iguana>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;

namespace Iguana.IguanaMesh.ITypes
{
	public struct IMatrix
	{
		private double[,] matrix;

		public int RowsCount { get => matrix.GetLength(0); }
		public int ColumnsCount { get => matrix.GetLength(1); }

		public IMatrix(int rows, int columns)
		{
			matrix = new double[rows,columns];
		}

		public IMatrix(double[,] matrix)
		{
			this.matrix = matrix;
		}

		public IMatrix(double[][] data)
		{
			int rows = data.Length;
			int columns = data[0].Length;
			matrix = new double[rows, columns];
			for(int i=0; i<rows; i++)
            {
				for(int j=0; j<columns; j++)
                {
					matrix[i, j] = data[i][j];
                }
            }
		}

		public IMatrix(IMatrix m)
		{
			matrix = new double[m.RowsCount, m.ColumnsCount];
			for (int i = 0; i < m.RowsCount; i++)
			{
				for (int j = 0; j < m.ColumnsCount; j++)
				{
					matrix[i,j] = m.GetData(i, j);
				}
			}
		}

		public void SetData(int row, int column, double data)
		{
			matrix[row,column] = data;
		}

		public double GetData(int row, int column)
		{
			return matrix[row,column];
		}

		public double[] GetRow(int index)
		{
			double[] c = new double[ColumnsCount];
			for (int i = 0; i < ColumnsCount; i++)
			{
				c[i] = matrix[index, i];
			}
			return c;
		}

		public double[] GetColumn(int index)
		{
			double[] c = new double[RowsCount];
			for (int i = 0; i < RowsCount; i++)
			{
				c[i] = matrix[i,index];
			}
			return c;
		}

		public double[,] GetMatrix()
		{
			return matrix;
		}

		public override string ToString()
		{
			String result = "";
			try
			{
				for (int i = 0; i < matrix.GetLength(0); i++)
				{
					for (int j = 0; j < matrix.GetLength(1); j++)
					{
						result += " " + matrix[i,j];
					}
					result += "\n";
				}
				return result;
			}
			catch (Exception)
			{
				result = "Null Matrix";
				return result;
			}

		}

		public static IMatrix Mult(IMatrix matrix1, IMatrix matrix2)
		{
			try
			{
				IMatrix matrix = new IMatrix(matrix1.RowsCount, matrix2.ColumnsCount);
				for (int i = 0; i < matrix.RowsCount; i++)
				{
					for (int j = 0; j < matrix.ColumnsCount; j++)
					{
						double newData = 0;
						for (int k = 0; k < matrix1.ColumnsCount; k++)
						{
							newData += matrix1.GetData(i, k) * matrix2.GetData(k, j);
						}
						matrix.SetData(i, j, newData);
					}
				}
				return matrix;
			}
			catch (Exception)
			{
				Console.WriteLine("Matrix with unequal column/row lengths");
				return new IMatrix();
			}
		}

		public void Mult(IMatrix matrix1)
		{
			try
			{
				IMatrix newMatrix = new IMatrix(RowsCount, matrix1.ColumnsCount);
				for (int i = 0; i < newMatrix.RowsCount; i++)
				{
					for (int j = 0; j < newMatrix.ColumnsCount; j++)
					{
						double newData = 0;
						for (int k = 0; k < matrix1.ColumnsCount; k++)
						{
							newData += matrix[i,k] * matrix1.GetData(k, j);
						}
						newMatrix.SetData(i, j, newData);
					}
				}
				matrix = newMatrix.GetMatrix();
			}
			catch (Exception)
			{
				Console.WriteLine("Matrix with unequal column/row lengths");
			}
		}

		public static IMatrix Transpose(IMatrix matrix)
		{
			try
			{
				IMatrix newMatrix = new IMatrix(matrix.ColumnsCount, matrix.RowsCount);
				for (int i = 0; i < matrix.RowsCount; i++)
				{
					for (int j = 0; j < matrix.ColumnsCount; j++)
					{
						newMatrix.SetData(j, i, matrix.matrix[i,j]);
					}
				}
				return newMatrix;
			}
			catch (Exception)
			{
				Console.WriteLine("Matrix with unequal column/row lengths");
				return new IMatrix();
			}
		}

		public void Transpose()
		{
			try
			{
				IMatrix newMatrix = new IMatrix(ColumnsCount, RowsCount);
				for (int i = 0; i < RowsCount; i++)
				{
					for (int j = 0; j < ColumnsCount; j++)
					{
						newMatrix.SetData(j, i, matrix[i,j]);
					}
				}
				this.matrix = newMatrix.GetMatrix();
			}
			catch (Exception)
			{
				Console.WriteLine("Matrix with unequal column/row lengths");
			}
		}

		public static IMatrix Add(IMatrix matrix1, IMatrix matrix2)
		{
			try
			{
				IMatrix newMatrix = new IMatrix(matrix1.RowsCount, matrix1.ColumnsCount);
				for (int i = 0; i < matrix1.RowsCount; i++)
				{
					for (int j = 0; j < matrix1.ColumnsCount; j++)
					{
						double data = matrix1.GetData(i, j) + matrix2.GetData(i, j);
						newMatrix.SetData(i, j, data);
					}
				}
				return newMatrix;
			}
			catch (Exception)
			{
				Console.WriteLine("Matrices with unequal dimensions");
				return new IMatrix();
			}
		}

		public void Add(IMatrix matrix1)
		{
			try
			{
				for (int i = 0; i < RowsCount; i++)
				{
					for (int j = 0; j < ColumnsCount; j++)
					{
						matrix[i,j] += matrix1.GetData(i, j);
					}
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Matrices with unequal dimensions");
			}
		}


		public static double[] vectorMult(IMatrix matrix, double[] vector)
		{
			if (matrix.ColumnsCount == vector.Length)
			{
				double[] F = new double[matrix.RowsCount];
				for (int i = 0; i < matrix.RowsCount; i++)
				{
					for (int j = 0; j < matrix.ColumnsCount; j++)
					{
						F[i] += matrix.GetData(i, j) * vector[j];
					}
				}
				return F;
			}
			else return null;
		}

		public static IVector3D VectorMult(IMatrix matrix, IVector3D vector)
		{
			double[] F = new double[3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					F[i] += matrix.GetData(i, j) * vector.GetVectorComponent(j);
				}
			}
			IVector3D vec = new IVector3D(F[0], F[1], F[2]);
			return vec;
		}

		public double Determinant()
		{
			try
			{
				IMatrix temp;
				double result = 0;
				if (RowsCount == 1) result = matrix[0,0];
				if (RowsCount == 2) result = ((matrix[0,0] * matrix[1,1]) - (matrix[0,1] * matrix[1,0]));

				else
				{
					for (int i = 0; i < ColumnsCount; i++)
					{
						temp = new IMatrix(RowsCount - 1, ColumnsCount - 1);
						for (int j = 1; j < RowsCount; j++)
						{
							for (int k = 0; k < ColumnsCount; k++)
							{
								if (k < i)
								{
									temp.matrix[j - 1,k] = matrix[j,k];
								}
								else if (k > i)
								{
									temp.matrix[j - 1,k - 1] = matrix[j,k];
								}
							}
						}
						result += matrix[0,i] * Math.Pow(-1, i) * Determinant(temp);
					}
				}
				return result;
			}
			catch (Exception)
			{
				Console.WriteLine("Matrices with unequal dimensions");
				return 0;
			}
		}

		public static double Determinant(IMatrix matrix)
		{
			try
			{
				IMatrix temp;
				double result = 0;
				if (matrix.RowsCount == 1) result = matrix.GetData(0, 0);
				if (matrix.RowsCount == 2) result = ((matrix.GetData(0, 0) * matrix.GetData(1, 1)) - (matrix.GetData(0, 1) * matrix.GetData(1, 0)));

				else
				{
					for (int i = 0; i < matrix.ColumnsCount; i++)
					{
						temp = new IMatrix(matrix.RowsCount - 1, matrix.ColumnsCount - 1);
						for (int j = 1; j < matrix.RowsCount; j++)
						{
							for (int k = 0; k < matrix.ColumnsCount; k++)
							{
								if (k < i)
								{
									temp.SetData(j - 1, k, matrix.GetData(j, k));
								}
								else if (k > i)
								{
									temp.SetData(j - 1, k - 1, matrix.GetData(j, k));
								}
							}
						}
						result += matrix.GetData(0, i) * Math.Pow(-1, i) * Determinant(temp);
					}
				}
				return result;
			}
			catch (Exception)
			{
				Console.WriteLine("Matrices with unequal dimensions");
				return 0;
			}
		}

		public void Invert()
		{
			try
			{
				double d = Determinant();
				if (d != 0)
				{
					int length = RowsCount;
					double[,] newMatrix = new double[length,length];
					double[,] temp = new double[length,length];
					int[] index = new int[length];

					for (int i = 0; i < length; ++i)
					{
						for (int j = 0; j < length; ++j) temp[i,j] = 1;
					}

					ToUpperTriangle(matrix, index);

					for (int i = 0; i < length - 1; ++i)
						for (int j = i + 1; j < length; ++j)
							for (int k = 0; k < length; ++k)
								temp[index[j],k] -= matrix[index[j],i] * temp[index[i],k];

					for (int i = 0; i < length; ++i)
					{
						newMatrix[length - 1,i] = temp[index[length - 1],i] / matrix[index[length - 1],length - 1];
						for (int j = length - 2; j >= 0; --j)
						{
							newMatrix[j,i] = temp[index[j],i];
							for (int k = j + 1; k < length; ++k)
							{
								newMatrix[j,i] -= matrix[index[j],k] * newMatrix[k,i];
							}
							newMatrix[j,i] /= matrix[index[j],j];
						}
					}
					matrix = newMatrix;
				}
				else
				{
					Console.WriteLine("Determinant is zero; Inverse matrix doesn´t exist");
					matrix = new double[0,0];
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Matrices with unequal dimensions");
			}
		}

		public static IMatrix Invert(IMatrix matrix)
		{
			try
			{
				double d = Determinant(matrix);
				if (d != 0)
				{
					IMatrix mCopy = new IMatrix(matrix);
					int length = mCopy.RowsCount;
					IMatrix newMatrix = new IMatrix(length, length);
					double[,] temp = new double[length,length];
					int[] index = new int[length];

					for (int i = 0; i < length; ++i)
					{
						for (int j = 0; j < length; ++j) temp[i,j] = 1;
					}

					ToUpperTriangle(mCopy.GetMatrix(), index);

					for (int i = 0; i < length - 1; ++i)
						for (int j = i + 1; j < length; ++j)
							for (int k = 0; k < length; ++k)
								temp[index[j],k] -= mCopy.matrix[index[j],i] * temp[index[i],k];

					for (int i = 0; i < length; ++i)
					{
						newMatrix.matrix[length - 1,i] = temp[index[length - 1],i] / mCopy.matrix[index[length - 1],length - 1];
						for (int j = length - 2; j >= 0; --j)
						{
							newMatrix.matrix[j,i] = temp[index[j],i];
							for (int k = j + 1; k < length; ++k)
							{
								newMatrix.matrix[j,i] -= mCopy.matrix[index[j],k] * newMatrix.matrix[k,i];
							}
							newMatrix.matrix[j,i] /= mCopy.matrix[index[j],j];
						}
					}
					return newMatrix;
				}
				else
				{
					Console.WriteLine("Determinant is zero; Inverse matrix doesn´t exist");
					return new IMatrix();
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Matrices with unequal dimensions");
				return new IMatrix();
			}
		}

		private static void ToUpperTriangle(double[,] a, int[] index)
		{
			int n = index.Length;
			double[] c = new double[n];

			for (int i = 0; i < n; ++i)
			{
				index[i] = i;
			}

			for (int i = 0; i < n; ++i)
			{
				double c1 = 0;
				for (int j = 0; j < n; ++j)
				{
					double c0 = Math.Abs(a[i,j]);
					if (c0 > c1) c1 = c0;
				}
				c[i] = c1;
			}

			int k = 0;
			for (int j = 0; j < n - 1; ++j)
			{
				double pi1 = 0;
				for (int i = j; i < n; ++i)
				{
					double pi0 = Math.Abs(a[index[i],j]);
					pi0 /= c[index[i]];
					if (pi0 > pi1)
					{
						pi1 = pi0;
						k = i;
					}
				}

				int itmp = index[j];
				index[j] = index[k];
				index[k] = itmp;
				for (int i = j + 1; i < n; ++i)
				{
					double pj = a[index[i],j] / a[index[j],j];
					a[index[i],j] = pj;

					for (int l = j + 1; l < n; ++l)
						a[index[i],l] -= pj * a[index[j],l];
				}
			}
		}

		public static IMatrix Scale(IMatrix matrix, double value)
		{
			try
			{
				IMatrix m = new IMatrix(matrix.RowsCount, matrix.ColumnsCount);
				for (int i = 0; i < matrix.RowsCount; i++)
				{
					for (int j = 0; j < matrix.ColumnsCount; j++)
					{
						m.matrix[i,j] = matrix.matrix[i,j] * value;
					}
				}
				return m;
			}
			catch (Exception)
			{
				Console.WriteLine("Matrices with unequal dimensions");
				return new IMatrix();
			}
		}

		public IMatrix TensorProduct(IVector3D vec1, IVector3D vec2)
		{
			double[][] data = new double[3][];
			data[0] = new double[3] { vec1.X * vec2.X, vec1.X * vec2.Y, vec1.X * vec2.Z };
			data[1] = new double[3] { vec1.Y * vec2.X, vec1.Y * vec2.Y, vec1.Y * vec2.Z };
			data[2] = new double[3] { vec1.Z * vec2.X, vec1.Z * vec2.Y, vec1.Z * vec2.Z };
			return new IMatrix(data);
		}

		public void Scale(double value)
		{
			try
			{
				for (int i = 0; i < RowsCount; i++)
				{
					for (int j = 0; j < ColumnsCount; j++)
					{
						this.matrix[i,j] *= value;
					}
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Matrices with unequal dimensions");
			}
		}

		public static IMatrix Skew(IVector3D vector)
		{
			IMatrix m = new IMatrix(3, 3);
			m.matrix[0,0] = 0;
			m.matrix[0,1] = -vector.Z;
			m.matrix[0,2] = vector.Y;
			m.matrix[1,0] = vector.Z;
			m.matrix[1,1] = 0;
			m.matrix[1,2] = -vector.X;
			m.matrix[2,0] = -vector.Y;
			m.matrix[2,1] = vector.X;
			m.matrix[2,2] = 0;
			return m;

		}

		public static IMatrix ExpSkew(IVector3D vec)
		{
			double r = vec.Mag();

			double c = Math.Cos(r);
			double s = Math.Sin(r);
			IVector3D e = IVector3D.Norm(vec);

			// Rodriguez formula
			double[][] m = new double[3][];
			m[0] = new double[] { c + (e.X * e.X) * (1 - c), e.X * e.Y * (1 - c) - e.Z * s, e.Z * e.X * (1 - c) + e.Y * s };
			m[1] = new double[] { e.X * e.Y * (1 - c) + e.Z * s, c + (e.Y * e.Y) * (1 - c), e.Y * e.Z * (1 - c) - e.X * s };
			m[2] = new double[] { e.Z * e.X * (1 - c) - e.Y * s, e.Y * e.Z * (1 - c) + e.X * s, c + (e.Z * e.Z) * (1 - c) };

			IMatrix R = new IMatrix(m);

			return new IMatrix(R);
		}

		public static IMatrix ExpSkewNeg(IVector3D vec)
		{
			double r = vec.Mag();

			double c = Math.Cos(r);
			double s = Math.Sin(r);
			IVector3D e = IVector3D.Norm(vec);

			// Rodriguez formula
			double[][] m = new double[3][];
			m[0] = new double[] { c + (e.X * e.X) * (1 - c), e.X * e.Y * (1 - c) - e.Z * s, e.Z * e.X * (1 - c) + e.Y * s };
			m[1] = new double[] { e.X * e.Y * (1 - c) + e.Z * s, c + (e.Y * e.Y) * (1 - c), e.Y * e.Z * (1 - c) - e.X * s };
			m[2] = new double[] { e.Z * e.X * (1 - c) - e.Y * s, e.Y * e.Z * (1 - c) + e.X * s, c + (e.Z * e.Z) * (1 - c) };

			IMatrix R = new IMatrix(m);
			R.Invert();

			return new IMatrix(R);
		}

		public IMatrix clone()
		{
			IMatrix newMatrix = new IMatrix(this);
			return newMatrix;
		}

		public double GetMaxValue()
		{
			double max = this.matrix[0,0];
			for (int i = 0; i < this.RowsCount; i++)
			{
				for (int j = 0; j < this.ColumnsCount; j++)
				{
					double temp = this.matrix[i,j];
					if (temp > max) max = temp;
				}
			}
			return max;
		}

		public double GetMinValue()
		{
			double min = this.matrix[0,0];
			for (int i = 0; i < this.RowsCount; i++)
			{
				for (int j = 0; j < this.ColumnsCount; j++)
				{
					double temp = this.matrix[i,j];
					if (temp < min) min = temp;
				}
			}
			return min;
		}

		public static IMatrix Identity4x4Matrix
		{
			get
			{
				double[][] data = new double[4][];
				data[0] = new double[] { 1, 0, 0, 0 };
				data[1] = new double[] { 0, 1, 0, 0 };
				data[2] = new double[] { 0, 0, 1, 0 };
				data[3] = new double[] { 0, 0, 0, 1 };
				return new IMatrix(data);
			}
		}

		public static IMatrix Identity3x3Matrix
		{
			get
			{
				double[][] data = new double[3][];
				data[0] = new double[] { 1, 0, 0 };
				data[1] = new double[] { 0, 1, 0 };
				data[2] = new double[] { 0, 0, 1 };

				return new IMatrix(data);
			}
		}

		public static IMatrix InvertIdentity3x3Matrix
		{
			get
			{
				IMatrix m = IMatrix.Identity3x3Matrix;
				m.Invert();
				return m;
			}
		}

		public static IMatrix InvertIdentity4x4Matrix
		{
			get
			{
				IMatrix m = IMatrix.Identity4x4Matrix;
				m.Invert();
				return m;
			}
		}
	}
}
