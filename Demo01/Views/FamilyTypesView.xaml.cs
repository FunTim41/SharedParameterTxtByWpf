using CommunityToolkit.Mvvm.Messaging;
using Demo01.Models;
using Demo01.ViewModels;
using Demo01.XmlHelp;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// FamilyTypesView.xaml 的交互逻辑
    /// </summary>
    public partial class FamilyTypesView : Window
    {
       
        public FamilyTypesView()
        {
            
            InitializeComponent();
            this.DataContext = new FamTypeViewModel();
            WeakReferenceMessenger.Default.Register<CloseWindowMessage, string>(this, "关闭族类型窗口", (r, m) =>
            {
                this.Close();
            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

    }
}
