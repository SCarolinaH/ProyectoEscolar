using System.Collections.Generic;

namespace SchoolAppV2.Models
{
    public class FileUpdate
    {
        public int StudentId { get; set; }

        public List<FileServer> FilesServers { get; set; }
    }
}
