namespace WolfInv.com.BaseObjectsLib
{
    /// <summary>
    /// 一次性机会，其整体开始停止时间由发现机会的类控制，机会本身无法指定下次金额，无getChipAmount方法
    /// </summary>
    public class OnceChance<T> : ChanceClass<T> where T : TimeSerialData
    {
        public OnceChance()
        {
            AllowMaxHoldTimeCnt = 1;
        }
    }


    
}
