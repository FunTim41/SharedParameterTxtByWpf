using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Demo01.Models;
using Demo01.Views;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit.Primitives;

namespace Demo01.ViewModels
{
    public partial class MainViewModel : ObservableObject, IRecipient<ValueChangedMessage<Param>>
    {
        [ObservableProperty]
        Param newParam;

        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register(this, "新参数");
        }

        /// <summary>
        /// 组相关按钮可用性
        /// </summary>
        [ObservableProperty]
        bool isGroEn = false;

        /// <summary>
        /// 参数相关按钮可用性
        /// </summary>
        [ObservableProperty]
        bool isEn = false;

        /// <summary>
        /// 新属性组名称
        /// </summary>
        [ObservableProperty]
        string newGroupName;

        /// <summary>
        /// 共享参数文件地址
        /// </summary>
        [ObservableProperty]
        string filePath;

        /// <summary>
        /// 当前选中的group
        /// </summary>
        [ObservableProperty]
        ParaGroup selectedGroup;

        [ObservableProperty]
        ObservableCollection<ParaGroup> groupList = new();

        /// <summary>
        /// 共享参数合集
        /// </summary>
        //[ObservableProperty]
        //ObservableCollection<Param> paramList = new();

        #region 命令
        /// <summary>
        /// 创建新组
        /// </summary>
        [RelayCommand]
        void OpenNewGroupView()
        {
            NewGroupView newGroupView = new NewGroupView();
            //加入新的组名到ComboBox
            if (newGroupView.ShowDialog() == true)
            {
                bool isexist = false;
                ParaGroup group = new ParaGroup();
                group.Name = newGroupView.InputBox.Text;
                foreach (var item in GroupList)
                {
                    if (item.Name.Equals(group.Name))
                    {
                        isexist = true;
                        MessageBox.Show(
                            "已存在该组",
                            "提示",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                    }
                }
                group.Id = GroupList.Count + 1;
                if (!isexist)
                {
                    GroupList.Add(group);
                    SelectedGroup = group;
                }
            }
            isListCountIsZero();
        }

        /// <summary>
        /// 创建共享参数属性
        /// </summary>
        [RelayCommand]
        void OpenParamPropView()
        {
            ParamPropView PropView = new ParamPropView();
            PropView.ShowDialog();
        }

        /// <summary>
        /// 载入现有共享参数文件
        /// </summary>
        [RelayCommand]
        void LoadSharedFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                FilePath = openFile.FileName;
            }
        }

        /// <summary>
        ///创建新共享参数文件
        /// </summary>
        [RelayCommand]
        void CreateNewSharedFile()
        {
            SaveFileDialog saveFile = new SaveFileDialog
            {
                Title = "创建共享参数文件",
                Filter = "文本文件 (*.txt)|*.txt",
                DefaultExt = ".txt",
                AddExtension = true,
            };
            if (saveFile.ShowDialog() == true)
            {
                FileStream file = File.Create(saveFile.FileName);
                file.Close();

                FilePath = saveFile.FileName;
                WriteIntoFile();
            }
        }

        [RelayCommand]
        void CloseWindow()
        {
            Application.Current.Shutdown();
        }
        /// <summary>
        /// 重命名组
        /// </summary>
        [RelayCommand]
        void ReNameGroup()
        {
            NewGroupView newGroupView = new NewGroupView();
            newGroupView.Title = "重命名组";
            if (newGroupView.ShowDialog() == true)
            {
                ParaGroup group = new ParaGroup();
                group = SelectedGroup;
                GroupList.Remove(SelectedGroup);
                group.Name = newGroupView.InputBox.Text;
                GroupList.Add(group);

                SelectedGroup=group;

            }

        }
        /// <summary>
        /// 删除组
        /// </summary>
        [RelayCommand]
        void DeleteGroup()
        {
            GroupList.Remove(SelectedGroup);
            if (GroupList.Count!=0)
            {
                SelectedGroup = GroupList[0];
            }
            
        }

        /// <summary>
        /// 判断list是否为零,零则改变按钮可用性
        /// </summary>
        void isListCountIsZero()
        {
            if (GroupList.Count != 0)
            {
                IsGroEn = true;
            }
            else
            {
                IsGroEn = false;
                return;
            }
            if (SelectedGroup!=null)
            {
                if (SelectedGroup.ParamList.Count != 0)
                {
                    IsEn = true;
                }
                else
                {
                    IsEn = false;
                }
            }
            
        }

        #endregion
        /// <summary>
        /// 文件初始化
        /// </summary>
        private void WriteIntoFile()
        {
            // 使用 StreamWriter 写入文件
            using (StreamWriter writer = new StreamWriter(FilePath, append: false)) // append: false 表示覆盖文件
            {
                writer.WriteLine("# This is a Revit shared parameter file.");
                writer.WriteLine("# Do not edit manually.");
                writer.WriteLine("*META\tVERSION\tMINVERSION");
                writer.WriteLine("META\t2\t1");
                writer.WriteLine("*GROUP\tID\tNAME");
                writer.WriteLine(
                    "*PARAM\tGUID\tNAME\tDATATYPE\tDATACATEGORY\tGROUP\tVISIBLE\tDESCRIPTION\tUSERMODIFIABLE"
                );
            }
        }

        /// <summary>
        /// 接收的新参数
        /// </summary>
        public void Receive(ValueChangedMessage<Param> message)
        {
            NewParam = message.Value;
            NewParam.GroupId = SelectedGroup.Id;
            bool isExist = false;
            foreach (var item in GroupList)
            {
                foreach (var param in item.ParamList)
                {
                    if (NewParam.Name == param.Name)
                    {
                        isExist = true;
                        break;
                    }
                }
            }

            if (!isExist)
            {
                SelectedGroup.ParamList.Add(NewParam);
            }
            else
                MessageBox.Show(
                    "已存在该参数",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            isListCountIsZero();
        }
    }
}
