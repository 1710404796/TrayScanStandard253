namespace TrayScanStandard.Attritubes
{
    /// <summary>
    /// 权限枚举
    /// </summary>
    public enum PowerEnum
    {
        电芯条码显示,
        相机管理界面,
        光源控制界面,
        扫码日志,
        电芯种类管理,
        程序参数设定,
        用户管理界面,
        关闭软件,
        权限设置
        // 电芯计数,
        // 拆盘日志,
        // 组盘日志,
        // 相机列表,
    }
    /// <summary>
    /// 角色枚举
    /// </summary>
    public enum RoleEnum
    {
        操作员 = 1,
        技师 = 2,
        me技术员 = 3,
        ME工程师 = 4,
        系统管理员 = 5,

        SuperAdmin = 999
    }
}