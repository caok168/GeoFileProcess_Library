using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoFileProcess.Model
{
    public class GEO_FILE_HEADER
    {
        public short file_version { get; set; }
        public short dir_flag { get; set; }
        public int data_record_length { get; set; }
        public float sample_interval { get; set; }
        public float post_units { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string area { get; set; }
        public string division { get; set; }
        public string region { get; set; }
    }
}
