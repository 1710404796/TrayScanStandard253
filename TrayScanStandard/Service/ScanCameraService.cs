using HKCamera.Fs.NET.Controls;
using MediatR;
using MugenCamera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayScanStandard.ViewModel;
using static MvCamCtrl.NET.MyCamera;
namespace TrayScanStandard.Service
{
    public class ScanCameraService(IMediator mediator)
    {
        public Option<MugenCamera.MugenCamera>[] MugenCameras { get; set; } = [];
        public BcrBorderViewModel[] BcrBorderViewModels = [];
        public Image2DViewModel[] Image2DViewModels = [];
        public void Init()
        {
            var settings = MainStorage.Saves.ConnectAddresses
                .Take(MainStorage.Saves.CameraCnt);
            var cameras =
                settings
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
            MugenCameras = [.. cameras.Map(
                c => c.ToOption()
                )];

            Image2DViewModels = [.. settings.Map((i, s) => new Image2DViewModel() { BcrInfo = s, CameraIdx = i + 1})];

            BcrBorderViewModels =
                [.. Image2DViewModels.Map(s => new BcrBorderViewModel() { Image2DViewModel = s })];


            //MugenCameras = cameras.Match
        }

    }
}
