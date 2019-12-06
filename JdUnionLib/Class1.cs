﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolfInv.com.JdUnionLib
{
    public class JdUnionClass
    {
        public string appkey { get; set; }
        public string secretkey { get; set; }
        
    }

    public class JdGoodsClass:JdUnionClass
    {
        /*频道id：1-好券商品,
        2-超级大卖场,
        10-9.9专区,
        22-热销爆品,
        24-数码家电,
        25-超市,
        26-母婴玩具,
        27-家具日用,
        28-美妆穿搭,
        29-医药保健,
        30-图书文具,
        31-今日必推,
        32-王牌好货,
        33-秒杀商品,
        34-拼购商品
        */
        public string eliteId { get; set; }
    }

   
}