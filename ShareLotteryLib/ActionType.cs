namespace WolfInv.com.ShareLotteryLib
{
    public enum ActionType
    {
        QueryBussiness,
        ShowSystemInfo,
        CreateUserInfo,
        BindUserInfo,
        AddBussiness,//加业务
        ManualInstructs,//手工补充下注指令,属于彩票平台指令系统
        JdUnion,//京东联盟系统
        LotteryRunningTimeInfoQuery,//彩票运行时信息查询,属于彩票平台查询系统
        ApplyCreate,
        SubmitNewInfo,
        CancelCurr,
        SubcribeShares,
        ModifyPlan,
        AppendShares,
        ShowPlan,
        EndTheSubscribe,
        DeclareResult,
        DeclareProfit,
        ClosePlan,
        Charge,//充值指令
        ResetSystem,
        ValidateInfo,
        Undefined
    }


}
