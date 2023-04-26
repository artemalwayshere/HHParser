using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HHParser.Business.Models
{
    public class JobMessage
    {
        public string Title { get; set; }
        public string JobUrl { get; set; }
        public DateTime PublishDate { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is JobMessage message) return JobUrl == message.JobUrl;
            return false;
        }
        public override int GetHashCode() => JobUrl.GetHashCode();
    }
}
