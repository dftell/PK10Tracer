using WebMsg.IRepository.Repositorys;
using WebMsg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebMsg.Repository.Repositorys
{
    public class AuthCodeRep : BaseRepository<AuthCode>, IAuthCodeRep
    {
        //相关的配置
        public AuthCodeRep()
        {
            this.IsLogicDelete = true;
            this.RemoveField = "IsRemove";
            this.RemoveFalseValue = 0;
            this.RemoveTrueValue = 1;
        }
    }
}
