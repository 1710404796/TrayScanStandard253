using LinxUniverse.Utils;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TrayScanStandard.Mediator.Commands;

namespace TrayScanStandard.Mediator.Handlers
{
    class SaveDataFrameCommandHandler : IRequestHandler<SaveDataFrameCommand>
    {
        public Task Handle(SaveDataFrameCommand request, CancellationToken cancellationToken)
        {
            var folderPath = FilenameHelper.FileName + "_DataFrame";
            var fullPath = Path.Combine(FilenameHelper.AppPath + "DataFrame", folderPath);

            EnsureDirectoryExists(fullPath);

            var updatedDataFrame = ProcessImagesAndUpdatePaths(request.DataFrame, fullPath);

            SaveDataFrameInfo(fullPath, updatedDataFrame);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 确保目录存在
        /// </summary>
        /// <param name="path"></param>
        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 处理图像并更新路径
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        private static DataFrame ProcessImagesAndUpdatePaths(DataFrame dataFrame, string outputPath)
        {
            var updatedImages = dataFrame.Images
                .Select((img, index) => ProcessSingleImage(img, index + 1, outputPath))
                .ToArray();

            return dataFrame with { Images = updatedImages };
        }

        /// <summary>
        /// 处理单个图像并更新路径
        /// </summary>
        /// <param name="image"></param>
        /// <param name="cameraIndex"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        private static CamImages ProcessSingleImage(CamImages image, int cameraIndex, string outputPath)
        {
            var updatedImagesPaths = image.ImagesPath
                .Select(info => CopyImageAndCreateNewPath(info, image.Serial, cameraIndex, outputPath))
                .ToArray();

            return image with { ImagesPath = updatedImagesPaths };
        }

        /// <summary>
        /// 复制图像并创建新路径
        /// </summary>
        /// <param name="originalInfo"></param>
        /// <param name="serial"></param>
        /// <param name="cameraIndex"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        private static ImageInfo CopyImageAndCreateNewPath(ImageInfo originalInfo, string serial, int cameraIndex, string outputPath)
        {
            var extension = Path.GetExtension(originalInfo.Path);
            if (string.IsNullOrWhiteSpace(extension)) extension = ".png";
            var newFileName = $"cam{cameraIndex:0000}-{serial}-exp{originalInfo.Exposure}{extension}";
            var newFilePath = Path.Combine(outputPath, newFileName);

            File.Copy(originalInfo.Path, newFilePath, true);

            return originalInfo with { Path = newFileName };
        }

        /// <summary>
        /// 保存DataFrame信息
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="dataFrame"></param>
        private static void SaveDataFrameInfo(string outputPath, DataFrame dataFrame)
        {
            var jsonContent = JsonSerializer.Serialize(dataFrame);
            File.WriteAllText(Path.Combine(outputPath, "info.json"), jsonContent);
        }
    }
}
