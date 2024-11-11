using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Demo01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Demo01.ViewModels
{
    public partial class ToolTipViewModel : ObservableObject
    {

        [ObservableProperty]
        string toolTip;
        /// <summary>
        /// 发送消息并关闭窗口
        /// </summary>
        [RelayCommand]
        void SendToolTip()
        {
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(ToolTip), "工具提示消息");
            WeakReferenceMessenger.Default.Send(new CloseWindowMessage(), "关闭属性描述窗口");
        }
    }
}
