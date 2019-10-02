using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    #region Company

   public class TermEntity
    {
        public string Descr { get; set; }
    }
   
    public class AccountEntity
    {
        public int AcctNo { get; set; }
    }

    #endregion


    #region FirmDefaultScope

    public class RecordTypeEntity
    {
        public byte Code { get; set; }
        public string Descr { get; set; }
        public int? IsLocationPageView { get; set; }
    }

    #endregion

    public class FileTypeEntity
    {
        public int FileTypeId { get; set; }
        public string FileType { get; set; }
    }

    public class InternalStatusesEntity
    {
        public int InternalStatusId { get; set; }
        public string InternalStatus { get; set; }
    }

    public class CanvasRequestMasterEntity
    {
        public int ID { get; set; }
        public string Description { get; set; }
    }
}
