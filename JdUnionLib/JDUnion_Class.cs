using System.Collections.Generic;
using WolfInv.Com.JsLib;
namespace WolfInv.com.JdUnionLib
{

    public class JdUnion_Service_List_Class : JdUnion_Class
    {
        public new List<JdUnion_Service_List_Item_Class> items { get; set; }
        public class JdUnion_Service_List_Item_Class : JdUnion_Class.JdUnion_Item_Class
        {
            public string dbId { get; set; }
            public bool isFree { get; set; }
            public string name { get; set; }
            public string endDate { get; set; }
            public string beginDate { get; set; }
            //// "dbId": 7950938951,
            ////"isFree": false,
            ////"name": "在线进销存3.0（标准版）",
            ////"endDate": "2022-02-24",
            ////"beginDate": "2017-02-24"

        }

        public bool InitRequestJson()
        {
            base.InitRequestJson();
            ReqJson = string.Format("access_token={0}", JdUnion_GlbObject.Access_token);

            return true;

        }
    }

    public class JdUnion_Goods_List_Class : JdUnion_Bussiness_List_Class
    {
        
    }

    /// <summary>
    /// 示例
    /// </summary>
    public class JDYSCM_SaleOrder_Add_Class : JdUnion_Bussiness_List_Class
    {
        public List<JDYSCM_SaleOrder_List_Item_Class> items { get; set; }
        public class JDYSCM_SaleOrder_List_Item_Class:JsonableClass<JDYSCM_SaleOrder_List_Item_Class>
        {
            public string number { get; set; }
            public string date { get; set; }
            public string customerNumber { get; set; }
            public string employeeNumber { get; set; }
            public string discRate { get; set; }
            public string discAmt { get; set; }
            public string deliveryDate { get; set; }
            public string remark { get; set; }
            public List<SaleOrderProduct> entries { get; set; }

        }

        public class SaleOrderProduct
        {
            public string productNumber { get; set; }
            public string skuCode { get; set; }
            public string skuId { get; set; }
            public string unit { get; set; }
            public string location { get; set; }
            public int qty { get; set; }
            public string price { get; set; }
            public int discRate { get; set; }
            public int discAmt { get; set; }
            public string remark { get; set; }

        }

        public class SaleOrderReturnResult
        {

        }
    }
}
