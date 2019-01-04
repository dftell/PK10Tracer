namespace DataRecSvr
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        ///////// <summary> 
        ///////// 清理所有正在使用的资源。
        ///////// </summary>
        ///////// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        //////protected override void Dispose(bool disposing)
        //////{
        //////    if (disposing && (components != null))
        //////    {
        //////        components.Dispose();
        //////    }
        //////    base.Dispose(disposing);
        //////}




        #region 组件设计器生成的代码

        ///////// <summary>
        ///////// 设计器支持所需的方法 - 不要
        ///////// 使用代码编辑器修改此方法的内容。
        ///////// </summary>
        //////private void InitializeComponent()
        //////{
        //////    components = new System.ComponentModel.Container();
        //////}

        #endregion
   
    
    
    
    
        /// <summary>
        /// 
        /// </summary>
        private System.ServiceProcess.ServiceProcessInstaller WorkProcessInstaller;

        /// <summary>
        /// 
        /// </summary>
        private System.ServiceProcess.ServiceInstaller WorkServiceInstaller;
    
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        //private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.WorkProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.WorkServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // WorkProcessInstaller
            // 
            this.WorkProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.WorkProcessInstaller.Password = null;
            this.WorkProcessInstaller.Username = null;
            // 
            // WorkServiceInstaller
            // 
            this.WorkServiceInstaller.Description = "武府菠菜服务";
            this.WorkServiceInstaller.DisplayName = "武府菠菜综合服务程序";
            this.WorkServiceInstaller.ServiceName = "武府菠菜";
            this.WorkServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.WorkProcessInstaller,
            this.WorkServiceInstaller});

        }

        #endregion

    
    }
}