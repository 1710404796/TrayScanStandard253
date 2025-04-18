using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camera.Fs.Common;
using LinxUniverse.CST;
using MugenCamera;
namespace TrayScanStandard.Utils
{
    public static class DetectUtil
    {

        // 还需要一个光源的中间件

        public static Either<string, TResult> UseLight<TResult>
            ( Func<Either<string, TResult>> func)
        {
            var lightInfos = MainStorage.Saves.LightInfos.Zip(MainStorage.CST);
            lightInfos.Iter(s =>
            {
                s.Item1.Values.Iter((i, v) =>
                {
                    s.Item2.SetLight(i, v);
                });
            });
            //lightValue.Iter((i, s) => lightCST.SetLight(i, s));

            var result = func();
            // Ensure the light is turned off after the action
            MainStorage.CST.Iter((i, s) => s.SetLight(i, 0));
            return result;

        }

        public static Either<string, TResult> UseCamera<TResult>(
            MugenCamera.MugenCamera mugenCamera,
            Func<MugenCamera.MugenCamera, Either<string, TResult>> action)
        {
            mugenCamera.StopGrab();
            var camSession = mugenCamera.StartGrab();
            // Execute the action only if StartGrab was successful
            var result = camSession.Bind(action);
            // Ensure StopGrab is called if StartGrab succeeded, regardless of action result
            camSession.Bind(s => s.StopGrab());
            return result;
        }


        public static Either<string, ImageData> CaptureOne(this MugenCamera.MugenCamera mugenCamera)
        {
            // Define the specific action: Software Trigger then Capture

            return UseLight(
                () => UseCamera(mugenCamera, 
                    cam =>
                    cam.SoftwareTrigger() // Assuming SoftwareTrigger returns Either<string, MugenCamera>
                       .Bind(s => s.Capture(TimeSpan.FromSeconds(3)))));
            // Use the helper method to execute the action within a session
            //return ;
        }

        //public static Either<string, ImageData> CaptureOne(MugenCamera.MugenCamera mugenCamera)
        //{
        //    var cam = mugenCamera.StartGrab();
        //    var img = cam.Bind(MugenCameraExtensions.SoftwareTrigger)
        //        .Bind(s => s.Capture(TimeSpan.FromSeconds(3)));
        //    cam = cam.Bind(s => s.StopGrab());
        //    return cam.Bind(s => img);
        //}
        public static Either<string, IEnumerable<ImageData>> Capture(MugenCamera.MugenCamera[] mugenCameras)
        {
            return mugenCameras
                .Map(CaptureOne)
                .Traverse(s => s);
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