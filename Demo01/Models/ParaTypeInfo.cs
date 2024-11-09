using Demo01.XmlHelp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Demo01.Models
{
	class Rule
	{
        public string Name { get; set; }
        public ObservableCollection<MyType>  TypeList { get; set; }=new();
    }
}
