using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TrayScanStandard.Models; // Added for LightInfo and WcsSaves

namespace TrayScanStandard.ViewModel
{
    public partial class LightManagerViewModel : ObservableRecipient
    {
        private  WcsSaves _wcsSaves => MainStorage.Saves; // Assuming WcsSaves is injected or accessible

        [ObservableProperty]
        private ObservableCollection<LightInfoViewModel> _lightInfos;        
        [ObservableProperty]
        private string _newComPort = string.Empty;

        [ObservableProperty]
        private string _newValuesString = string.Empty; // Input for comma-separated values

        [ObservableProperty]
        private LightInfoViewModel? _selectedLightInfo;
        
        [ObservableProperty]
        private string _editComPort = string.Empty;
        
        [ObservableProperty]
        private string _editValuesString = string.Empty;
        
        [ObservableProperty]
        private string _errorMessage = string.Empty;
        
        [ObservableProperty]
        private bool _isEditMode = false;

        // Constructor - Assuming WcsSaves is injected via DI or retrieved statically
        // Adjust this based on your actual dependency injection setup
        public LightManagerViewModel()
        {
            LightInfos = new ObservableCollection<LightInfoViewModel>(
                _wcsSaves.LightInfos.Select(li => new LightInfoViewModel(li))
            );
            // Listen for changes in the collection to update WcsSaves
            //_lightInfos.CollectionChanged += (s, e) => SaveChanges();
        }        [RelayCommand]
        private void AddLight()
        {
            if (string.IsNullOrWhiteSpace(NewComPort) || string.IsNullOrWhiteSpace(NewValuesString))
            {
                ErrorMessage = "COM端口和值必须填写";
                return;
            }

            try
            {
                // Parse the comma-separated string into an int array
                int[] values = NewValuesString.Split(',')
                                              .Select(s => int.Parse(s.Trim()))
                                              .ToArray();
                
                if (values.Length == 0)
                {
                    ErrorMessage = "至少需要一个光源值";
                    return;
                }

                // Check if COM port already exists
                if (LightInfos.Any(li => li.Com.Equals(NewComPort, StringComparison.OrdinalIgnoreCase)))
                {
                    ErrorMessage = $"COM端口 '{NewComPort}' 已经存在";
                    return;
                }

                var newLightInfo = new LightInfo(NewComPort, values);
                LightInfos.Add(new LightInfoViewModel(newLightInfo));

                // Clear input fields
                NewComPort = string.Empty;
                NewValuesString = string.Empty;
                ErrorMessage = string.Empty;
                SaveChanges();
            }
            catch (FormatException)
            {
                ErrorMessage = "光源值必须是以逗号分隔的整数";
            }
            catch (Exception)
            {
                ErrorMessage = "添加光源时发生错误";
            }
        }        [RelayCommand(CanExecute = nameof(CanDeleteLight))]
        private void DeleteLight()
        {
            if (SelectedLightInfo != null)
            {
                LightInfos.Remove(SelectedLightInfo);
                ErrorMessage = string.Empty;
                // SaveChanges(); // Called by CollectionChanged handler
            }
        }
        
        [RelayCommand(CanExecute = nameof(CanEditLight))]
        private void EditLight()
        {
            if (SelectedLightInfo == null || string.IsNullOrWhiteSpace(EditComPort) 
                || string.IsNullOrWhiteSpace(EditValuesString))
            {
                ErrorMessage = "请选择要编辑的光源并填写有效的值";
                return;
            }

            try
            {
                // Parse the comma-separated string into an int array
                int[] values = EditValuesString.Split(',')
                                               .Select(s => int.Parse(s.Trim()))
                                               .ToArray();
                
                if (values.Length == 0)
                {
                    ErrorMessage = "至少需要一个光源值";
                    return;
                }
                
                // Check if the COM port already exists (other than the currently selected item)
                if (LightInfos.Any(li => li != SelectedLightInfo && 
                                         li.Com.Equals(EditComPort, StringComparison.OrdinalIgnoreCase)))
                {
                    ErrorMessage = $"COM端口 '{EditComPort}' 已被其他光源使用";
                    return;
                }
                
                // Update the selected item
                SelectedLightInfo.Com = EditComPort;
                SelectedLightInfo.Values = values;
                var a = SelectedLightInfo;
                // Force UI refresh for the updated item
                var index = LightInfos.IndexOf(SelectedLightInfo);

                LightInfos.RemoveAt(index);
                LightInfos.Insert(index, a);
                
                IsEditMode = false;
                ErrorMessage = string.Empty;

                SaveChanges();

            }
            catch (FormatException)
            {
                ErrorMessage = "光源值必须是以逗号分隔的整数";
            }
            catch (Exception)
            {
                ErrorMessage = "编辑光源时发生错误";
            }
        }
        
        private bool CanEditLight() => SelectedLightInfo != null;private bool CanDeleteLight() => SelectedLightInfo != null;

        partial void OnSelectedLightInfoChanged(LightInfoViewModel? value)
        {
            // Update CanExecute state when selection changes
            DeleteLightCommand.NotifyCanExecuteChanged();
            EditLightCommand.NotifyCanExecuteChanged();
            
            if (value != null)
            {
                // Load the selected light's values to edit fields
                EditComPort = value.Com;
                EditValuesString = value.ValuesString;
            }
        }
        
        private void SaveChanges()
        {
            // Update the WcsSaves instance with the current list
            _wcsSaves.LightInfos = LightInfos.Select(vm => vm.ToModel()).ToArray();
            // Save changes to persist them
            MainStorage.SaveManager.Save();
        }
    }

    // Simple ViewModel wrapper for LightInfo to handle potential future UI-specific logic
    // And potentially INotifyPropertyChanged if inline editing is needed later
    public partial class LightInfoViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _com;

        [ObservableProperty]
        private int[] _values;

        // Display string for the values array
        public string ValuesString => string.Join(", ", Values);

        public LightInfoViewModel(LightInfo model)
        {
            _com = model.Com;
            _values = model.Values;
        }

        public LightInfo ToModel() => new LightInfo(Com, Values);

        // Override ToString for better display in lists if needed, though DataGrid columns are better
        public override string ToString() => $"COM: {Com}, Values: {ValuesString}";
    }
}