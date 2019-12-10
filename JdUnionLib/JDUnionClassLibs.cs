using System.Threading.Tasks;
using XmlProcess;
namespace WolfInv.com.JdUnionLib
{

    public abstract class JdUnion_Class: JdUnion_RequestClass
    {
        public JdUnion_Class()
        {

        }
        public string dbId { get; set; } //账套Id
        
        public int page { get; set; }   //当前页码
        public int totalsize { get; set; }  //当前返回总记录数
        public int records { get; set; }    //总记录数
        public int totalPages { get; set; }	//总页数
        //public List<JDYSCM_Item_Class> items { get; set; }

        public class JdUnion_Item_Class:JdUnion_JsonClass
        {

        }

        
    }

    
    
    


}
