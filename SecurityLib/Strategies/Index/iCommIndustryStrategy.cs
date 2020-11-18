using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
   


    public interface iCommIndustryStrategy<T>:iCommReverseMethod<T>, iCommBreachMethod<T>, iCommBalanceMethod<T> where T:TimeSerialData
    {
        
    }
}
