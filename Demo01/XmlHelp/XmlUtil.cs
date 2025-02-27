﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Demo01.XmlHelp
{
    class XmlUtil
    {/// <summary>
     /// XML序列化某一类型到指定的文件
     /// </summary>
     /// <param name="filePath"></param>
     /// <param name="obj"></param>
     /// <param name="type"></param>
        public static void SerializeToXml<T>(string filePath, T obj)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    xs.Serialize(writer, obj);
                }
            }
            catch (Exception ex )
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 从某一XML文件反序列化到某一类型
        /// </summary>
        /// <param name="filePath">待反序列化的XML文件名称</param>
        /// <param name="type">反序列化出的</param>
        /// <returns></returns>
        public static T DeserializeFromXml<T>(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                    throw new ArgumentNullException(filePath + " not Exists");

                using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    T ret = (T)xs.Deserialize(reader);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return default(T);
            }
        }
    }

    [XmlType(TypeName = "List")]
    public class TypeList
    {
        [XmlArray("types")]
        public List<MyType> Types { get; set; } = new ();
    }
    [XmlType("MyType")]
    public class MyType
    {
        [XmlAttribute("RuleName")]
        public string RuleName { get; set; }
        /// <summary>
        /// 导出的名字
        /// </summary>
        [XmlAttribute("ParaTypeName")]
        public string ParaType { get; set; }
        /// <summary>
        /// 展示的名字
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }
}
