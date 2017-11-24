using GeoFileProcess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"H:\工作文件汇总\铁科院\程序\工具\里程准确性判定小工具\data\GNHS-HANGZHOU-NANJING-14052016-175302-1.geo";
            path = @"F:\个人文件\铁路\工程代码\文件工具\里程小工具\data\CJJS-TIANJIN-BEIJING-18052012-235618_给曹凯测试用.geo";

            GeoFileProcess.GeoFileHelper geoHelper = new GeoFileProcess.GeoFileHelper();
            //var header = geoHelper.GetDataInfoHead(path);

            //var channelList = geoHelper.GetDataChannel(path);

            //StringBuilder sb = new StringBuilder();

            //for (int i = 0; i < channelList.Count; i++)
            //{
            //    sb.AppendLine(String.Format("sNameEn:{0},fScale:{1},sUnit:{2}", channelList[i].sNameEn, channelList[i].fScale, channelList[i].sUnit));
            //}

            //string str = sb.ToString();

            var data = geoHelper.GetMileStone(path);

            //List<int[]> listData = geoHelper.GetMileData(path);

            //List<double[]> listDataNew = new List<double[]>();
            //double[] km = new double[listData[0].Length];
            //double[] m = new double[listData[0].Length];
            //for (int i = 0; i < listData[0].Length; i++)
            //{
            //    km[i] = listData[0][i];
            //    m[i] = listData[1][i] / 4;
            //}

            //listDataNew.Add(km);
            //listDataNew.Add(m);

            //string pathNew = @"F:\个人文件\铁路\工程代码\文件工具\里程小工具\data\data.txt";
            //using (StreamWriter sw = new StreamWriter(pathNew))
            //{
            //    for (int i = 0; i < km.Length; i++)
            //    {
            //        sw.WriteLine(km[i] + "," + m[i]);
            //    }
            //}
        }
    }
}
