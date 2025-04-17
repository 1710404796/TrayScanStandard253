using HKCamera.Fs.NET.Controls;
using MediatR;
using MugenCamera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MvCamCtrl.NET.MyCamera;
namespace TrayScanStandard.Service
{
    public class ScanCameraService(IMediator mediator)
    {
        public MugenCamera.MugenCamera[] MugenCameras { get; set; }
        public void Init()
        {
            var cameras = 
                MainStorage.Saves.ConnectAddresses
                .Take(MainStorage.Saves.CameraCnt)
                .Map(s => 
                    s.CameraAddresses.Create()
                    .Bind(MugenCameraExtensions.Connect)
                    .Bind(s => s.SetControl(new AcquisitionControl
                    {
                        TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON,
                        TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE
                    }))
                )
                // Traverse to ensure all cameras are connected
                ;

            //MugenCameras = cameras.Match
        }

    }
}
