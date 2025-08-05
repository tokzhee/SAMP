using App.Core.Utilities;
using App.Database.Access;
using App.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Services
{
    public static class TransactionReportService
    {
        public static List<TransactionReport> GetTransactionReport(string startDate, string endDate, string fintechFinacleTermId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<TransactionReport> transactionReports = null;

            try
            {
                var query = new StringBuilder();
                query.Append("select datetime_req, tran_nr, message_type, pan, from_account_id, to_account_id, tran_amount_req, tran_amount_rsp, terminal_id, retrieval_reference_nr, system_trace_audit_nr, rsp_code_rsp, source_node_name, sink_node_name from post_tran pt with (nolock) join post_tran_cust ptc with (nolock) on pt.post_tran_cust_id = ptc.post_tran_cust_id and tran_postilion_originated = '1' and sink_node_name = 'REMITAFINsnk'");

                //if (!string.IsNullOrEmpty(fintechFinacleTermId))
                //{
                //    query.Append(" and (source_node_name = @TermId1 or source_node_name = @TermId2)");
                //}

                if (!string.IsNullOrEmpty(fintechFinacleTermId))
                {
                    query.Append($" and terminal_id like '%' + @TermId1 + '%'");
                }

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    query.Append(" and cast (datetime_req as date)  BETWEEN cast ( @StartDate as date)  and  cast ( @EndDate as date)");
                }

                query.Append("order by datetime_req asc");

                using (var db = new Data(ConfigurationManager.ConnectionStrings["PostillionOfficeConnection"].ToString(), null))
                {
                    //transactionReports = (List<TransactionReport>)db.QueryList<TransactionReport>(query.ToString(),new { TermId1  = $"{fintechFinacleTermId.ToUpper()}scr", TermId2 = $"{fintechFinacleTermId.ToUpper()}src", StartDate = Convert.ToDateTime(startDate).ToString("yyyy-MM-dd"), EndDate = Convert.ToDateTime(endDate).ToString("yyyy-MM-dd") });
                    transactionReports = (List<TransactionReport>)db.QueryList<TransactionReport>(query.ToString(), new { TermId1 = fintechFinacleTermId, StartDate = Convert.ToDateTime(startDate).ToString("yyyy-MM-dd"), EndDate = Convert.ToDateTime(endDate).ToString("yyyy-MM-dd") });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return transactionReports;
        }
    }
}
