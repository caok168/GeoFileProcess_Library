using GeoFileProcess.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeoFileProcess
{
    public class GeoFileHelper
    {
        const int GEO_FILE_VERSION = 0x3031;
        const int GEO_FILE_HEADER_SIZE = 0x2800;
        const int GEOMETRY_RECORD_ID = 0x5a5a;

        int iChannelLength = 0;
        int iChannelCount = 0;

        public GEO_FILE_HEADER GetDataInfoHead(string sFile)
        {
            try
            {
                using (FileStream fs = new FileStream(sFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BinaryReader br = new BinaryReader(fs, Encoding.Default))
                    {
                        br.BaseStream.Position = 0;
                        GEO_FILE_HEADER gfh = GetDataInfoHead(br.ReadBytes(GEO_FILE_HEADER_SIZE));
                        br.Close();
                        fs.Close();
                        return gfh;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sFile"></param>
        /// <returns></returns>
        public List<DataChannelInfo> GetDataChannel(string sFile)
        {
            using (FileStream fs = new FileStream(sFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.Default))
                {
                    br.BaseStream.Position = 0;
                    GEO_FILE_HEADER gfh = GetDataInfoHead(br.ReadBytes(GEO_FILE_HEADER_SIZE));
                    //添加通道信息
                    iChannelLength = BitConverter.ToInt16(br.ReadBytes(2), 0);
                    iChannelCount = BitConverter.ToInt16(br.ReadBytes(2), 0);

                    StringBuilder sbName = new StringBuilder();
                    List<DataChannelInfo> dciL = new List<DataChannelInfo>();
                    byte[] bChannelData = br.ReadBytes(iChannelLength - 2);

                    for (int j = 0; j < iChannelLength - 2; )
                    {
                        DataChannelInfo dci = GetChannelInfo(bChannelData, ref j);
                        dciL.Add(dci);
                        if (dciL.Count > iChannelCount)
                        {
                            break;
                        }
                        //if (iType == 10)
                        //{
                        //    if (dci.sNameEn.ToLower().Contains("null"))
                        //    {
                        //        break;
                        //    }
                        //}
                    }

                    return dciL;
                }
            }
        }

        /// <summary>
        /// 获取原始通道数据 KM 和sample
        /// </summary>
        /// <param name="sFile"></param>
        /// <returns></returns>
        public List<int[]> GetMileChannelData(string sFile)
        {
            using (FileStream fs = new FileStream(sFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.Default))
                {
                    br.BaseStream.Position = 0;
                    GEO_FILE_HEADER gfh = GetDataInfoHead(br.ReadBytes(GEO_FILE_HEADER_SIZE));
                    //添加通道信息
                    iChannelLength = BitConverter.ToInt16(br.ReadBytes(2), 0);
                    iChannelCount = BitConverter.ToInt16(br.ReadBytes(2), 0);

                    br.BaseStream.Position = 0;
                    br.ReadBytes(GEO_FILE_HEADER_SIZE);
                    br.ReadBytes(4);
                    br.ReadBytes(iChannelLength - 2);

                    int iChannelNumberSize = (gfh.data_record_length);
                    byte[] b = new byte[iChannelNumberSize];
                    //if (iType == 10)
                    //{
                    //    iChannelCount -= 1;
                    //}
                    //else
                    //{

                    //}

                    List<int> kmList = new List<int>();
                    List<int> mList = new List<int>();

                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {

                        b = br.ReadBytes(iChannelNumberSize);
                        int iGeometryRecordId = BitConverter.ToInt16(b, 0);
                        if (iGeometryRecordId != GEOMETRY_RECORD_ID)
                        {
                            continue;
                        }
                        short sKM = BitConverter.ToInt16(b, 4 * 2);
                        short sM = BitConverter.ToInt16(b, 5 * 2);

                        kmList.Add(sKM);
                        mList.Add(sM);
                    }
                    br.Close();
                    fs.Close();

                    List<int[]> listData = new List<int[]>();
                    listData.Add(kmList.ToArray());
                    listData.Add(mList.ToArray());

                    return listData;
                }
            }
        }

        /// <summary>
        /// 获取里程数据 KM 和 Meter
        /// </summary>
        /// <param name="sFile"></param>
        /// <returns></returns>
        public List<double[]> GetMileData(string sFile)
        {
            using (FileStream fs = new FileStream(sFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.Default))
                {
                    br.BaseStream.Position = 0;
                    GEO_FILE_HEADER gfh = GetDataInfoHead(br.ReadBytes(GEO_FILE_HEADER_SIZE));
                    //添加通道信息
                    iChannelLength = BitConverter.ToInt16(br.ReadBytes(2), 0);
                    iChannelCount = BitConverter.ToInt16(br.ReadBytes(2), 0);

                    br.BaseStream.Position = 0;
                    br.ReadBytes(GEO_FILE_HEADER_SIZE);
                    br.ReadBytes(4);
                    br.ReadBytes(iChannelLength - 2);

                    int iChannelNumberSize = (gfh.data_record_length);
                    byte[] b = new byte[iChannelNumberSize];

                    List<double> kmList = new List<double>();
                    List<double> mList = new List<double>();

                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {

                        b = br.ReadBytes(iChannelNumberSize);
                        int iGeometryRecordId = BitConverter.ToInt16(b, 0);
                        if (iGeometryRecordId != GEOMETRY_RECORD_ID)
                        {
                            continue;
                        }
                        short sKM = BitConverter.ToInt16(b, 4 * 2);
                        short sM = BitConverter.ToInt16(b, 5 * 2);

                        kmList.Add(sKM);
                        mList.Add(sM * 0.25);
                    }
                    br.Close();
                    fs.Close();

                    List<double[]> listData = new List<double[]>();
                    listData.Add(kmList.ToArray());
                    listData.Add(mList.ToArray());

                    return listData;
                }
            }
        }

        /// <summary>
        /// 获取里程标数据 KM加Meter  单位是公里
        /// </summary>
        /// <param name="sFile"></param>
        /// <returns></returns>
        public double[] GetMileStone(string sFile)
        {
            using (FileStream fs = new FileStream(sFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.Default))
                {
                    br.BaseStream.Position = 0;
                    GEO_FILE_HEADER gfh = GetDataInfoHead(br.ReadBytes(GEO_FILE_HEADER_SIZE));
                    //添加通道信息
                    iChannelLength = BitConverter.ToInt16(br.ReadBytes(2), 0);
                    iChannelCount = BitConverter.ToInt16(br.ReadBytes(2), 0);

                    br.BaseStream.Position = 0;
                    br.ReadBytes(GEO_FILE_HEADER_SIZE);
                    br.ReadBytes(4);
                    br.ReadBytes(iChannelLength - 2);

                    int iChannelNumberSize = (gfh.data_record_length);
                    byte[] b = new byte[iChannelNumberSize];
                    //if (iType == 10)
                    //{
                    //    iChannelCount -= 1;
                    //}
                    //else
                    //{

                    //}

                    List<double> kmList = new List<double>();

                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {

                        b = br.ReadBytes(iChannelNumberSize);
                        int iGeometryRecordId = BitConverter.ToInt16(b, 0);
                        if (iGeometryRecordId != GEOMETRY_RECORD_ID)
                        {
                            continue;
                        }
                        short sKM = BitConverter.ToInt16(b, 4 * 2);
                        short sM = BitConverter.ToInt16(b, 5 * 2);

                        kmList.Add(sKM + sM * 0.25 / 1000);
                    }
                    br.Close();
                    fs.Close();

                    return kmList.ToArray();
                }
            }
        }


        /// <summary>
        /// 按照结构体读取各个数据
        /// </summary>
        /// <param name="bDataInfo"></param>
        /// <returns></returns>
        private GEO_FILE_HEADER GetDataInfoHead(byte[] bDataInfo)
        {
            GEO_FILE_HEADER gfh = new GEO_FILE_HEADER();

            gfh.file_version = BitConverter.ToInt16(bDataInfo, 0);
            gfh.dir_flag = BitConverter.ToInt16(bDataInfo, 2);
            gfh.data_record_length = BitConverter.ToInt32(bDataInfo, 4);
            gfh.sample_interval = BitConverter.ToSingle(bDataInfo, 8);
            gfh.post_units = BitConverter.ToSingle(bDataInfo, 12);

            //获取GEO Date
            StringBuilder sbDate = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                if (bDataInfo[42 + i] == 0)
                {
                    break;
                }
                sbDate.Append(UnicodeEncoding.Default.GetString(bDataInfo, 42 + i, 1));
            }
            gfh.date = sbDate.ToString();

            //获取GEO Time
            StringBuilder sbTime = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                if (bDataInfo[54 + i] == 0)
                {
                    break;
                }
                sbTime.Append(UnicodeEncoding.Default.GetString(bDataInfo, 54 + i, 1));
            }
            gfh.time = sbTime.ToString();

            //获取GEO Area
            StringBuilder sbArea = new StringBuilder();
            for (int i = 0; i < 60; i++)
            {
                if (bDataInfo[66 + i] == 0)
                {
                    break;
                }
                sbArea.Append(UnicodeEncoding.Default.GetString(bDataInfo, 66 + i, 1));
            }
            gfh.area = sbArea.ToString();

            //获取GEO Division
            StringBuilder sbDivision = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                if (bDataInfo[130 + i] == 0)
                {
                    break;
                }
                sbDivision.Append(UnicodeEncoding.Default.GetString(bDataInfo, 130 + i, 1));
            }
            gfh.division = sbDivision.ToString();

            //获取GEO Region
            StringBuilder sbRegion = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                if (bDataInfo[150 + i] == 0)
                {
                    break;
                }
                sbRegion.Append(UnicodeEncoding.Default.GetString(bDataInfo, 150 + i, 1));
            }
            gfh.region = sbRegion.ToString();

            return gfh;
        }

        /// <summary>
        /// 获取通道信息
        /// </summary>
        /// <param name="bDataInfo"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        private DataChannelInfo GetChannelInfo(byte[] bDataInfo, ref int start)
        {
            DataChannelInfo dci = new DataChannelInfo();
            StringBuilder sNameEn = new StringBuilder();
            StringBuilder sUnit = new StringBuilder();
            int iNameEnLen = (int)bDataInfo[start];
            for (int i = 1; i <= iNameEnLen; i++)
            {
                sNameEn.Append(UnicodeEncoding.Default.GetString(bDataInfo, i + start, 1));
            }
            start += (1 + iNameEnLen);
            int iUnitLen = (int)bDataInfo[start];
            for (int i = 1; i <= iUnitLen; i++)
            {
                sUnit.Append(UnicodeEncoding.Default.GetString(bDataInfo, i + start, 1));
            }
            start += (1 + iUnitLen);

            dci.sNameEn = sNameEn.ToString();
            dci.sUnit = sUnit.ToString();
            dci.fScale = BitConverter.ToInt32(bDataInfo, start);
            start += 4;
            while (start < bDataInfo.Length && (bDataInfo[start] == (byte)0))
            {
                start++;
            }

            return dci;

        }
    }
}
