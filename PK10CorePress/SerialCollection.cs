namespace WolfInv.com.PK10CorePress
{
    /// <summary>
    /// 按车号集合，和按名次集合命名搞反了
    /// </summary>
    public class SerialCollection : PK10CorePress.CommCollection
    {
        public SerialCollection()
        {
            isByNo = true;
        }
    }

}
