using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CDBReader
{
    public class CDBReader
    {
        public string Data { get; set; }

        public int DataLength { get; set; }

        public int ColumnNB { get; set; }

        public int[] ColumnSize { get; set; }

        public string[] ColumnName { get; set; }

        /// <summary>

        /// C for Char

        /// N for numeric

        /// F for float

        /// </summary>

        public char[] ColumnType { get; set; }



        public CDBReader(string filePath)

        {



            Data = File.ReadAllText(filePath, Encoding.UTF8);



            DataLength = Data.Length;

            //find the nombre of column

            for (int i = 32; i < DataLength / 2; i = i + 32)

            {

                if (String.Format("{0:X2}", Convert.ToInt32(Data[i])).Equals("0D"))

                {

                    ColumnNB = i / 32 - 1;

                    break;

                }

            }

            if (ColumnNB == 0)

                throw new Exception("cdb format incorrect!");

            ColumnName = new string[ColumnNB];

            ColumnSize = new int[ColumnNB];

            ColumnType = new char[ColumnNB];



            for (int i = 32, j = 0; i < (ColumnNB + 1) * 32; i = i + 32, j++)

            {

                //set column name

                ColumnName[j] = Data.Substring(i, 10).Replace("\0", " ").Trim();

                //set column type

                ColumnType[j] = Data[i + 11];

                //set column size

                ColumnSize[j] = Convert.ToInt32(String.Format("{0:X2}", Convert.ToInt32(Data[i + 16])), 16);

            }

        }



        public CDBReader(byte[] bytes)

        {

            Data = Encoding.UTF8.GetString(bytes);

            //Data = File.ReadAllText(filePath);



            DataLength = Data.Length;

            //find the nombre of column

            for (int i = 32; i < DataLength / 2; i = i + 32)

            {

                if (String.Format("{0:X2}", Convert.ToInt32(Data[i])).Equals("0D"))

                {

                    ColumnNB = i / 32 - 1;

                    break;

                }

            }

            if (ColumnNB == 0)

                throw new Exception("cdb format incorrect!");

            ColumnName = new string[ColumnNB];

            ColumnSize = new int[ColumnNB];

            ColumnType = new char[ColumnNB];



            for (int i = 32, j = 0; i < (ColumnNB + 1) * 32; i = i + 32, j++)

            {

                //set column name

                ColumnName[j] = Data.Substring(i, 10).Replace("\0", " ").Trim();

                //set column type

                ColumnType[j] = Data[i + 11];

                //set column size

                ColumnSize[j] = Convert.ToInt32(String.Format("{0:X2}", Convert.ToInt32(Data[i + 16])), 16);

            }

        }



        public string[,] GetData()
        {

            double eps = 1e-10;

            double lineNB = (DataLength - 2 - (ColumnNB + 1) * 32) * 1.0 / (ColumnSize.Sum() + 1);

            if (lineNB - Math.Floor(lineNB) > eps)

                throw new Exception("cdb format incorrect!");

            string[,] datavalue = new string[(int)lineNB, ColumnNB];

            int index = (ColumnNB + 1) * 32 + 1;

            for (int i = 0; i < (int)lineNB; i++)

            {

                index++;

                for (int j = 0; j < ColumnNB; j++)

                {

                    datavalue[i, j] = Data.Substring(index, ColumnSize[j]).Replace("\0", " ").Trim();

                    index += ColumnSize[j];

                }

            }

            return datavalue;



        }




    }
}
