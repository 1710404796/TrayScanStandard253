using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MugenCamera;
namespace TrayScanStandard.Utils
{
    public static class DetectUtil
    {

        public static void Detect(MugenCamera.MugenCamera[] mugenCamera)
        {
            var aa = mugenCamera
                .Map(s =>
                {
                    return s.StartGrab();
                })
;
            // 让所有相机拍个照

        }
        public static void CamCapture()
        {
            // 让所有相机拍个照

        }

        // 光源中间件‘

        public static void LightMiddle<T>(Func<T> f)
        {
            // 让所有光源中间件都亮
        }
    }
}