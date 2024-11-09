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
        PropViewModel propViewModel;
       public  Param Parameter=new();
        public ParamPropView()
        {
           propViewModel=  new PropViewModel();
            InitializeComponent();
            this.DataContext = propViewModel;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ParamToolTipView paramToolTipView = new ParamToolTipView();
            if (paramToolTipView.ShowDialog() == true)
            {
                this.tipBlock.Text = paramToolTipView.tip.Text;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }



        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            okBtn.IsEnabled = false;
            if (propNameBox.Text != null && propNameBox.Text != "")
            {
                okBtn.IsEnabled = true;
            }
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Parameter = propViewModel.Parameter;
            this.Close();
        }

       
    }
}
