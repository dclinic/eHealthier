using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class LogEntry
    {
        public static EPROMDBEntities db = null;

        public static EPROMDBEntities GetDataContext()
        {
            return new EPROMDBEntities();
        }

        public static void AddLog(string Message, string InnerMessage, string StackTrace)
        {
            LogManagement log = new LogManagement();

            var db = GetDataContext();

            log.Message = Message;
            log.InnerMessage = InnerMessage;
            log.StackTrace = StackTrace;
            log.CreatedDate = DateTime.Now;

            db.LogManagements.Add(log);
            db.SaveChanges();
        }
    }
}