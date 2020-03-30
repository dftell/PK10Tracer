using System;
using System.Collections.Generic;
using System.Data;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.PK10CorePress
{
    public class CommCollection_KLXxY : PK10CorePress.CommCollection
    {
        DataTable _dt;
        public int AllNums { get; set; }
        public int SelNums { get; set; }

        public string strAllTypeOdds { get; set; }
        public string strCombinTypeOdds { get; set; }
        public string strPermutTypeOdds { get; set; }
        public override DataTable Table
        {
            get
            {
                if(_dt == null)
                {
                    _dt = new DataTable();
                    if (isByNo)
                    {
                        for (int i = 0; i < SelNums; i++)
                        {
                            _dt.Columns.Add(string.Format("{0}", (i + 1) % (SelNums+1)).PadLeft(2,'0'));
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= this.AllNums; i++)
                        {
                            _dt.Columns.Add(i.ToString().PadLeft(2,'0'));
                        }
                        //_dt.Columns.Add("0");
                    }

                    for (int i = 0; i < Data[0].Count; i++)
                    {
                        DataRow dr = _dt.NewRow();
                        if (isByNo)
                        {
                            for (int j = 0; j < SelNums; j++)
                            {
                                dr[(j + 1).ToString().PadLeft(2,'0')] = Data[j][i];
                            }
                        }
                        else
                        {
                            for (int j = 0; j < AllNums; j++)
                            {
                                dr[(j+1).ToString().PadLeft(2,'0')] = Data[j][i];
                            }
                        }
                        _dt.Rows.Add(dr);
                    }
                }
                return _dt;
            }
        }

        public override DataTableEx CarDistributionTable => throw new NotImplementedException();

        public override DataTableEx CarTable {

            get
            {
                if (_CarTable != null) return _CarTable;
                _CarTable = new DataTableEx();
                _CarTable.Columns.Add("Id", typeof(int));
                //_CarTable.Columns.Add("Expect", typeof(string));
                if (isByNo)
                {
                    for (int i = 0; i < SelNums; i++)
                    {
                        _CarTable.Columns.Add(string.Format("{0}", (i + 1) % 10).PadLeft(2,'0'));
                    }
                }
                else
                {
                    for (int i = 1; i < AllNums; i++)
                    {
                        _CarTable.Columns.Add(i.ToString());
                    }

                }

                for (int i = 0; i < Data[0].Count; i++)
                {
                    DataRow dr = _CarTable.NewRow();
                    if (isByNo)
                    {
                        for (int j = 0; j < SelNums; j++)
                        {
                            dr[j] = (this.orgData[i]).ValueList[j];
                        }
                    }
                    else
                    {
                        for (int j = 0; j < AllNums; j++)
                        {
                            for (int c = 0; c < this.orgData[i].ValueList.Length; c++)
                            {
                                if(int.Parse(this.orgData[i].ValueList[c])-1 == j)
                                {
                                    dr[this.orgData[i].ValueList[c].PadLeft(2,'0')] = 1;
                                }
                                else
                                {
                                    dr[this.orgData[i].ValueList[c].PadLeft(2, '0')] = 0;
                                }
                            }
                        }
                    }
                    dr["Id"] = Data[0].Count - i;
                    _CarTable.Rows.Add(dr);

                }
                return _CarTable;


            }
        }

        public override DataTableEx SerialDistributionTable => throw new NotImplementedException();

        public override bool isByNo { get; set; }

        public override int FindLastDataExistCount(int StartPos, int lng, string StrKey, string val)
        {
            throw new NotImplementedException();
        }

        public override int FindLastDataExistCount(int lng, string StrPos, string key)
        {
            throw new NotImplementedException();
        }

        public override string FindSpecColumnValue(int id, string strKey)
        {
            throw new NotImplementedException();
        }

        public override List<double> getAllDistrStdDev(int n, int c)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, double> getAllShiftCnt(int ReviewCnt, int TrainCnt)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, Matrix> getC_K_NStep(int reviewCnt, int StepCnt)
        {
            throw new NotImplementedException();
        }

        public override List<double> getEntropyList(int reviewCnt)
        {
            throw new NotImplementedException();
        }

        public override DataTableEx getSubTable(int cnt, int n)
        {
            throw new NotImplementedException();
        }
    }

}
