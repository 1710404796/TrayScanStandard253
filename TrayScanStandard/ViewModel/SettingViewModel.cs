using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TrayScanStandard.Models;

namespace TrayScanStandard.ViewModel
{
    public partial class SettingViewModel(MainViewModel mainViewModel) : ObservableRecipient
    {
        public WcsSaves Saves => MainStorage.Saves;
        [ObservableProperty]
        bool _canEditing = true;

        public string WcsIpPort
        {
            get
            {
                var ip = Saves.IP?.Trim() ?? string.Empty;
                var port = Saves.Port?.Trim() ?? string.Empty;
                return string.IsNullOrWhiteSpace(port) ? ip : $"{ip}:{port}";
            }
            set
            {
                var text = (value ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(text))
                {
                    Saves.IP = string.Empty;
                    Saves.Port = string.Empty;
                    return;
                }

                // 兼容中英文冒号
                var normalized = text.Replace('：', ':');
                var idx = normalized.LastIndexOf(':');
                if (idx > 0 && idx < normalized.Length - 1)
                {
                    Saves.IP = normalized[..idx].Trim();
                    Saves.Port = normalized[(idx + 1)..].Trim();
                }
                else
                {
                    // 未输入端口时保留原端口，仅更新IP
                    Saves.IP = normalized.Trim();
                }
            }
        }

        public bool IsBackGround
        {
            get => Saves.BackGroundEnable == System.Windows.Visibility.Visible;
            set
            {
                Saves.BackGroundEnable = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                mainViewModel.HasBackGround = Saves.BackGroundEnable;
            }
        }

    }
}
