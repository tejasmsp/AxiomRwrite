using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Axiom.Data.Repository;
using Axiom.Entity;

namespace Axiom.Web
{
    public static class CommonFunction
    {
        private static readonly GenericRepository<OrderPartEntity> _repository = new GenericRepository<OrderPartEntity>();

        public static CompanyDetailForEmailEntity CompanyDetailForEmail(int CompanyNo)
        {
            CompanyDetailForEmailEntity result = new CompanyDetailForEmailEntity();
            try
            {
                SqlParameter[] param = { new SqlParameter("CompanyNo", (object)CompanyNo ?? (object)DBNull.Value) };

                result = _repository.ExecuteSQL<CompanyDetailForEmailEntity>("CompanyDetailForEmail", param).FirstOrDefault();

                if (result == null)
                {
                    result = new CompanyDetailForEmailEntity();
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

    }
}