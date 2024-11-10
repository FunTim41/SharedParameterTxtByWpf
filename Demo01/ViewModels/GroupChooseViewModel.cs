using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Demo01.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo01.ViewModels
{
    public partial class GroupChooseViewModel:ObservableRecipient,IRecipient<ValueChangedMessage<ObservableCollection<ParaGroup>>>
    {
        public GroupChooseViewModel()
        {
                IsActive = true;
            WeakReferenceMessenger.Default.Register(this, "现有组");
        }
        [ObservableProperty]
        ObservableCollection<ParaGroup> groupList = new();
        [ObservableProperty]
        ParaGroup selectedGroup;

        public void Receive(ValueChangedMessage<ObservableCollection<ParaGroup>> message)
        {
            GroupList = message.Value;
            SelectedGroup = GroupList[0];
        }
        [RelayCommand]
        void SendGroup()
        {
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<ParaGroup>(SelectedGroup),"指定组");
        }
    }
}
