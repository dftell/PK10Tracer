using System;
using System.Collections.Generic;
using System.Linq;

namespace WolfInv.com.BaseObjectsLib
{
    public class GroupBuilder
    {
        /// <summary>
        /// 分组T对象,前N-1填满，剩余的放最后一个数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orgArr"></param>
        /// <param name="grpCnt"></param>
        /// <returns></returns>
        public static List<T[]> ToGroup<T>(T[] orgArr, int grpCnt = 0)
        {
            List<T[]> ret = new List<T[]>();
            int realcnt = grpCnt;
            if (grpCnt <= 0)
                realcnt = orgArr.Length/10;
            int grpInnerCnt = (int)Math.Floor((double)orgArr.Length / realcnt);//每组个数
            int lastInnerCnt = orgArr.Length - grpInnerCnt * (realcnt - 1);
            for (int i = 0; i < realcnt; i++)
            {
                int copycnt = grpInnerCnt;
                if (i == realcnt - 1)
                {
                    copycnt = lastInnerCnt;
                }
                T[] grpcodes = new T[copycnt];
                orgArr.ToList().CopyTo(i * grpInnerCnt, grpcodes, 0, copycnt);//复制到数组中去
                ret.Add(grpcodes);
            }
            return ret;
        }


    }

    

}
