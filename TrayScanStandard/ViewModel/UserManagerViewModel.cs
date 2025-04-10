using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using LinxUniverse.Auth;
using TrayScanStandard.Attritubes;
using TrayScanStandard.Data.Models;
using TrayScanStandard.View.User;

namespace TrayScanStandard.ViewModel
{
    public partial class UserManagerViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private ObservableCollection<AdvUser> _users = new();
    }


    public class AdvUser
    {
        public LinxUser LinxUser { get; set; }
        public string RoleName => Utils.Utils.GetRoleName(Role);
        public RoleEnum Role { get; set; }
        public string PassStar => LinxUser.Password.Truncate(5, "****", Truncator.FixedLength);

    }
}