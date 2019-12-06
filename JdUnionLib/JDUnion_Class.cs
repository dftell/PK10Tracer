using System.Collections.Generic;
using WolfInv.Com.JsLib;
namespace WolfInv.com.JdUnionLib
{
    public class JDYSCM_SaleOrder_Update_Class: JDYSCM_SaleOrder_Add_Class
    {

    }

    public class JDYSCM_SaleOrder_Delete_Class : JDYSCM_Bussiness_Class
    {

    }

    public class JDYSCM_SaleOrder_Lock_Class : JDYSCM_SaleOrder_Delete_Class
    {

    }

    public class JDYSCM_SaleOrder_List_Class : JDYSCM_Bussiness_List_Class
    {

    }

    public class JDYSCM_SaleDelivery_List_Class : JDYSCM_Bussiness_List_Class
    {

    }

    public class JDYSCM_SaleDelivery_Add_Class : JDYSCM_Bussiness_List_Class
    {

    }

    public class JDYSCM_SaleDelivery_Update_Class : JDYSCM_Bussiness_List_Class
    {

    }

    public class JDYSCM_SaleDelivery_Delete_Class : JDYSCM_Bussiness_List_Class
    {

    }

    public class JDYSCM_SaleOrder_Add_Class : JDYSCM_Bussiness_List_Class
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
