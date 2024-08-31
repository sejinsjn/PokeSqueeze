using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarusuProofOrganizer.Models
{
    public class VideoProof
    {
        public string OldFilePath { get; set; }
        public string MuteFilePath { get; set; }
        public string NewFilePath { get; set; }
        public string TradeHistory { get; set; }
        public bool Compress { get; set; }

        public VideoProof() { }
        public VideoProof(string OldFilePath, string MuteFilePath, string NewFilePath, string TradeHistory, bool Compress) 
        {
            this.OldFilePath = OldFilePath;
            this.MuteFilePath = MuteFilePath;
            this.NewFilePath = NewFilePath;
            this.TradeHistory = TradeHistory;
            this.Compress = Compress;
        }
    }
}
