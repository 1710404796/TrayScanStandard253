using TrayScanStandard;
using TrayScanStandard.ViewModel;

namespace TrayScanStandard.Data.Models
{
    public class OKNGCnt
    {
        // 想办法通知一下
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 扫码OK
        /// </summary>
        public int OKCnt { get; set; }
        /// <summary>
        /// 扫码NG
        /// </summary>
        public int NG1Cnt { get; set; }

        /// <summary>
        /// 校验NG
        /// </summary>
        public int NG2Cnt { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// Ok新增并通知
        /// </summary>
        /// <param name="value"></param>
        public void AddOk(int value)
        {
            OKCnt += value;
            App.GetService<DataDisplayViewModel>().Update();
        }

        /// <summary>
        /// NG1新增并通知
        /// </summary>
        /// <param name="value"></param>

        public void AddNG1(int value)
        {
            NG1Cnt += value;
            App.GetService<DataDisplayViewModel>().Update();
        }
        /// <summary>
        /// NG2新增并通知
        /// </summary>
        /// <param name="value"></param>

        public void AddNG2(int value)
        {
            NG2Cnt += value;
            App.GetService<DataDisplayViewModel>().Update();
        }
    }
}