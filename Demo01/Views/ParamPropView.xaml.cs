using CommunityToolkit.Mvvm.Messaging;
using Demo01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Demo01.Views
{
   
    /// <summary>
    /// ParamPropView.xaml 的交互逻辑
    /// </summary>
    public partial class ParamPropView : Window
    {
       
        public ParamPropView()
        {
            
            InitializeComponent();
            this.DataContext = new PropViewModel();
            WeakReferenceMessenger.Default.Register<CloseWindowMessage,string>(this, "关闭属性窗口", (r, m) =>
            {
                this.Close();
            });
        }

       
    }
}
