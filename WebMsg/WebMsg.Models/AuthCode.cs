using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMsg.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AuthCode
    {
        public AuthCode()
        {
            this.PK_ACID = Tools.BillNumber.NextBillNumberForLocal();
            this.UpdateTime = DateTime.Now;
            this.CreateTime = DateTime.Now;
            this.IsRemove = 0;
            this.Sort = 0;
        }
        /// <summary>
        /// 验证码主键
        /// </summary>
        public string PK_ACID { get; set; }
        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 验证码类型
        /// </summary>
        public int? CType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? IsRemove { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime { get; set; }

    }
}
