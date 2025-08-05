using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Utilities;
using App.Database.Access;
using App.DataModels.Models;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

namespace App.Core.Services
{
    public static class FinacleServices
    {
        /*
            SELECT * from (SELECT rownum RN, A.* from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling A) where (MOD(RN, 1)+ 1) = 1 and (is_valid_crms is null or is_valid_crc is null or is_valid_bvn is null) order by INSERTED_DATE desc;
         */
        public static Sol GetFinacleSol1(string staffid, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            Sol sol = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("UseDummySol").Equals("Y"))
                {
                    sol = new Sol
                    {
                        SolId = "230",
                        SolName = "HO",
                        SolAddress = "Marina"
                    };

                    return sol;
                }

                using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("FinacleConnection")))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = oracleConnection.CreateCommand())
                    {
                        var oracleParameters = new OracleParameter[]
                        {
                            new OracleParameter(":StaffID", staffid.ToUpper())
                        };

                        oracleCommand.CommandType = CommandType.Text;
                        oracleCommand.CommandText = "SELECT C.SOL_ID, B.FORACID as TILL, C.sol_desc,C.ADDR_1||C.ADDR_2 as address from tbaadm.GEC A, tbaadm.gam B, tbaadm.Sol C WHERE A.EMP_CASH_ACCT = B.BACID AND A.SOL_ID = B.SOL_ID AND B.SOL_ID = C.SOL_ID AND A.EMP_ID = :StaffID AND A.SOL_ID = (select sol_id from tbaadm.upr where user_id= :StaffID) AND B.ACCT_CRNCY_CODE = 'NGN'";
                        oracleCommand.Parameters.AddRange(oracleParameters);

                        try
                        {
                            using (var oracleDataReader = oracleCommand.ExecuteReader())
                            {
                                if (oracleDataReader.HasRows)
                                {
                                    while (oracleDataReader.Read())
                                    {
                                        sol = new Sol
                                        {
                                            SolId = oracleDataReader["Sol_Id"]?.ToString(),
                                            SolName = oracleDataReader["Sol_Desc"]?.ToString(),
                                            SolAddress = oracleDataReader["Address"]?.ToString()
                                        };
                                        break;
                                    }
                                }
                                else
                                {
                                    sol = GetFinacleSol2(staffid, callerFormName, callerFormMethod, callerIpAddress);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                        }

                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return sol;
        }
        public static Sol GetFinacleSol2(string staffid, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            Sol sol = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("UseDummySol").Equals("Y"))
                {
                    sol = new Sol
                    {
                        SolId = "230",
                        SolName = "HO",
                        SolAddress = "Marina"
                    };

                    return sol;
                }

                using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("FinacleConnection")))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = oracleConnection.CreateCommand())
                    {
                        var oracleParameters = new OracleParameter[]
                        {
                            new OracleParameter(":StaffID", staffid.ToUpper())
                        };

                        oracleCommand.CommandType = CommandType.Text;
                        oracleCommand.CommandText = "SELECT C.SOL_ID, C.SOL_DESC, C.ADDR_1||C.ADDR_2 as Address from tbaadm.Sol C, tbaadm.upr D WHERE D.sol_id = C.SOL_ID AND D.USER_ID = :StaffID";
                        oracleCommand.Parameters.AddRange(oracleParameters);

                        try
                        {
                            using (var oracleDataReader = oracleCommand.ExecuteReader())
                            {
                                if (oracleDataReader.HasRows)
                                {
                                    while (oracleDataReader.Read())
                                    {
                                        sol = new Sol
                                        {
                                            SolId = oracleDataReader["Sol_Id"]?.ToString(),
                                            SolName = oracleDataReader["Sol_Desc"]?.ToString(),
                                            SolAddress = oracleDataReader["Address"]?.ToString()
                                        };

                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                        }

                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return sol;
        }
        public static string GetAccountName(string accountNumber, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var accountName = "";

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("UseDummyAccountName").Equals("Y"))
                {
                    accountName = "JAMIU ISMAIL";
                    return accountName;
                }

                using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("FinacleConnection")))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = oracleConnection.CreateCommand())
                    {
                        var oracleParameters = new OracleParameter[]
                        {
                            new OracleParameter(":Foracid", accountNumber)
                        };

                        oracleCommand.CommandType = CommandType.Text;
                        oracleCommand.CommandText = "SELECT ACCT_NAME from tbaadm.gam where foracid = :Foracid";
                        oracleCommand.Parameters.AddRange(oracleParameters);

                        try
                        {
                            using (var oracleDataReader = oracleCommand.ExecuteReader())
                            {
                                if (oracleDataReader.HasRows)
                                {
                                    while (oracleDataReader.Read())
                                    {
                                        accountName = oracleDataReader["ACCT_NAME"]?.ToString();
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                        }

                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return accountName;
        }
        public static List<SettlementReport> GetTodaysSettlementReport(string fintechFinacleTermId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<SettlementReport> settlementReports = null;

            try
            {
                using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("FinacleConnection")))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = oracleConnection.CreateCommand())
                    {
                        var oracleParameters = new OracleParameter[]
                        {
                            new OracleParameter(":FintechTermId", fintechFinacleTermId.ToUpper())
                        };

                        var query = "SELECT aa.tran_date, TRAN_ID, aa.dth_init_sol_id init_sol_id, bb.foracid account_number, bb.acct_name account_name, aa.tran_amt, DECODE (aa.part_tran_type, 'C', 'CREDIT', 'DEBIT') transaction_type, aa.tran_particular narration, REF_NUM, TRAN_RMKS, TRAN_PARTICULAR_2, VALUE_DATE FROM tbaadm.dtd aa, (SELECT acid, foracid, acct_name FROM tbaadm.gam WHERE foracid LIKE '%' || '48934389035601' AND sol_id IN ('489')) bb, (select term_id from custom.FINTECH) cc WHERE aa.acid = bb.acid AND aa.del_flg = 'N' AND aa.pstd_flg = 'Y' AND cc.term_id = substr(TRAN_PARTICULAR_2,1, (instr(TRAN_PARTICULAR_2,'/')-1))";
                        if (!string.IsNullOrEmpty(fintechFinacleTermId))
                        {
                            query = $"{query} AND cc.term_id = :FintechTermId order by PSTD_DATE";
                            oracleCommand.Parameters.AddRange(oracleParameters);
                        }
                        else
                        {
                            query = $"{query} order by PSTD_DATE";
                        }

                        oracleCommand.CommandText = query;
                        oracleCommand.CommandType = CommandType.Text;

                        try
                        {
                            using (var oracleDataReader = oracleCommand.ExecuteReader())
                            {
                                if (oracleDataReader.HasRows)
                                {
                                    settlementReports = new List<SettlementReport>();

                                    while (oracleDataReader.Read())
                                    {
                                        var settlementReport = new SettlementReport
                                        {
                                            TranDate = oracleDataReader["Tran_Date"]?.ToString(),
                                            TranId = oracleDataReader["Tran_Id"]?.ToString(),
                                            InitSolId = oracleDataReader["Init_Sol_Id"]?.ToString(),
                                            AccountNumber = oracleDataReader["Account_Number"]?.ToString(),
                                            AccountName = oracleDataReader["Account_Name"]?.ToString(),
                                            TranAmt = oracleDataReader["Tran_Amt"]?.ToString(),
                                            TransactionType = oracleDataReader["Transaction_Type"]?.ToString(),
                                            Narration = oracleDataReader["Narration"]?.ToString(),
                                            RefNum = oracleDataReader["Ref_Num"]?.ToString(),
                                            TranRmks = oracleDataReader["Tran_Rmks"]?.ToString(),
                                            TranParticular2 = oracleDataReader["Tran_Particular_2"]?.ToString(),
                                            ValueDate = oracleDataReader["Value_Date"]?.ToString()
                                        };

                                        settlementReports.Add(settlementReport);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                        }

                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return settlementReports;
        }
        public static List<SettlementReport> GetTodaysSettlementReport(string startDate, string endDate, string fintechFinacleTermId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<SettlementReport> settlementReports = null;

            try
            {
                using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("FinacleConnection")))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = oracleConnection.CreateCommand())
                    {

                        var query = new StringBuilder();
                        query.Append("SELECT aa.tran_date, TRAN_ID, aa.dth_init_sol_id init_sol_id, bb.foracid account_number, bb.acct_name account_name, aa.tran_amt, DECODE (aa.part_tran_type, 'C', 'CREDIT', 'DEBIT') transaction_type, aa.tran_particular narration, REF_NUM, TRAN_RMKS, TRAN_PARTICULAR_2, VALUE_DATE FROM tbaadm.dtd aa, (SELECT acid, foracid, acct_name FROM tbaadm.gam WHERE foracid LIKE '%' || '48934389035601' AND sol_id IN ('489')) bb, (select term_id from custom.FINTECH) cc WHERE aa.acid = bb.acid AND aa.del_flg = 'N' AND aa.pstd_flg = 'Y' AND cc.term_id = substr(TRAN_PARTICULAR_2,1, (instr(TRAN_PARTICULAR_2,'/')-1))");
                        if (!string.IsNullOrEmpty(startDate))
                        {
                            query.Append(" AND aa.tran_date >= TO_DATE(:StartDate,'DD-MM-YYYY') ");
                            oracleCommand.Parameters.Add(new OracleParameter(":StartDate", Convert.ToDateTime(startDate).ToString("dd-MM-yyyy")));
                        }

                        if (!string.IsNullOrEmpty(endDate))
                        {
                            query.Append(" AND aa.tran_date <= TO_DATE(:EndDate,'DD-MM-YYYY') ");
                            oracleCommand.Parameters.Add(new OracleParameter(":EndDate", Convert.ToDateTime(endDate).ToString("dd-MM-yyyy")));
                        }

                        if (!string.IsNullOrEmpty(fintechFinacleTermId))
                        {
                            query.Append(" AND cc.term_id = :FintechTermId ");
                            oracleCommand.Parameters.Add(new OracleParameter(":FintechTermId", fintechFinacleTermId.ToUpper()));
                        }

                        query.Append(" order by PSTD_DATE");

                        oracleCommand.CommandText = query.ToString();
                        oracleCommand.CommandType = CommandType.Text;

                        try
                        {
                            using (var oracleDataReader = oracleCommand.ExecuteReader())
                            {
                                if (oracleDataReader.HasRows)
                                {
                                    settlementReports = new List<SettlementReport>();

                                    while (oracleDataReader.Read())
                                    {
                                        var settlementReport = new SettlementReport
                                        {
                                            TranDate = oracleDataReader["Tran_Date"]?.ToString(),
                                            TranId = oracleDataReader["Tran_Id"]?.ToString(),
                                            InitSolId = oracleDataReader["Init_Sol_Id"]?.ToString(),
                                            AccountNumber = oracleDataReader["Account_Number"]?.ToString(),
                                            AccountName = oracleDataReader["Account_Name"]?.ToString(),
                                            TranAmt = oracleDataReader["Tran_Amt"]?.ToString(),
                                            TransactionType = oracleDataReader["Transaction_Type"]?.ToString(),
                                            Narration = oracleDataReader["Narration"]?.ToString(),
                                            RefNum = oracleDataReader["Ref_Num"]?.ToString(),
                                            TranRmks = oracleDataReader["Tran_Rmks"]?.ToString(),
                                            TranParticular2 = oracleDataReader["Tran_Particular_2"]?.ToString(),
                                            ValueDate = oracleDataReader["Value_Date"]?.ToString()
                                        };

                                        settlementReports.Add(settlementReport);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                        }

                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return settlementReports;
        }
        public static List<SettlementReport> GetHistoricSettlementReport(string startDate, string endDate, string fintechFinacleTermId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<SettlementReport> settlementReports = null;

            try
            {
                using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("FinacleConnection")))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = oracleConnection.CreateCommand())
                    {
                        
                        var query = new StringBuilder();
                        query.Append("SELECT aa.tran_date, TRAN_ID, aa.dth_init_sol_id init_sol_id, bb.foracid account_number, bb.acct_name account_name, aa.tran_amt, DECODE (aa.part_tran_type, 'C', 'CREDIT', 'DEBIT') transaction_type, aa.tran_particular narration, REF_NUM, TRAN_RMKS, TRAN_PARTICULAR_2, VALUE_DATE FROM tbaadm.htd aa, (SELECT acid, foracid, acct_name FROM tbaadm.gam WHERE foracid LIKE '%' || '48934389035601' AND sol_id IN ('489')) bb, (select term_id from custom.FINTECH) cc WHERE aa.acid = bb.acid AND aa.del_flg = 'N' AND aa.pstd_flg = 'Y' AND cc.term_id = substr(TRAN_PARTICULAR_2,1, (instr(TRAN_PARTICULAR_2,'/')-1))");
                        if (!string.IsNullOrEmpty(startDate))
                        {
                            query.Append(" AND aa.tran_date >= TO_DATE(:StartDate,'DD-MM-YYYY') ");
                            oracleCommand.Parameters.Add(new OracleParameter(":StartDate", Convert.ToDateTime(startDate).ToString("dd-MM-yyyy")));
                        }

                        if (!string.IsNullOrEmpty(endDate))
                        {
                            query.Append(" AND aa.tran_date <= TO_DATE(:EndDate,'DD-MM-YYYY') ");
                            oracleCommand.Parameters.Add(new OracleParameter(":EndDate", Convert.ToDateTime(endDate).ToString("dd-MM-yyyy")));
                        }

                        if (!string.IsNullOrEmpty(fintechFinacleTermId))
                        {
                            query.Append(" AND cc.term_id = :FintechTermId ");
                            oracleCommand.Parameters.Add(new OracleParameter(":FintechTermId", fintechFinacleTermId.ToUpper()));
                        }

                        query.Append(" order by PSTD_DATE");

                        oracleCommand.CommandText = query.ToString();
                        oracleCommand.CommandType = CommandType.Text;

                        try
                        {
                            using (var oracleDataReader = oracleCommand.ExecuteReader())
                            {
                                if (oracleDataReader.HasRows)
                                {
                                    settlementReports = new List<SettlementReport>();

                                    while (oracleDataReader.Read())
                                    {
                                        var settlementReport = new SettlementReport
                                        {
                                            TranDate = oracleDataReader["Tran_Date"]?.ToString(),
                                            TranId = oracleDataReader["Tran_Id"]?.ToString(),
                                            InitSolId = oracleDataReader["Init_Sol_Id"]?.ToString(),
                                            AccountNumber = oracleDataReader["Account_Number"]?.ToString(),
                                            AccountName = oracleDataReader["Account_Name"]?.ToString(),
                                            TranAmt = oracleDataReader["Tran_Amt"]?.ToString(),
                                            TransactionType = oracleDataReader["Transaction_Type"]?.ToString(),
                                            Narration = oracleDataReader["Narration"]?.ToString(),
                                            RefNum = oracleDataReader["Ref_Num"]?.ToString(),
                                            TranRmks = oracleDataReader["Tran_Rmks"]?.ToString(),
                                            TranParticular2 = oracleDataReader["Tran_Particular_2"]?.ToString(),
                                            ValueDate = oracleDataReader["Value_Date"]?.ToString()
                                        };

                                        settlementReports.Add(settlementReport);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                        }

                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return settlementReports;
        }
        public static Fee GetFintechFees(string termId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            Fee fee = null;

            try
            {
                using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("FinacleConnection")))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = oracleConnection.CreateCommand())
                    {
                        var oracleParameters = new OracleParameter[]
                        {
                            new OracleParameter(":TermId", termId)
                        };

                        oracleCommand.CommandType = CommandType.Text;
                        oracleCommand.CommandText = "SELECT * from custom.FINTECH where TERM_ID = :TermId";
                        oracleCommand.Parameters.AddRange(oracleParameters);

                        try
                        {
                            using (var oracleDataReader = oracleCommand.ExecuteReader())
                            {
                                if (oracleDataReader.HasRows)
                                {
                                    while (oracleDataReader.Read())
                                    {
                                        fee = new Fee
                                        {
                                            TermId = oracleDataReader["TERM_ID"]?.ToString(),
                                            TillAccountNo = oracleDataReader["TILL_ACCT_NO"]?.ToString(),
                                            FeeActNo1 = oracleDataReader["FEE_ACT_NO1"]?.ToString(),
                                            FeeActNo2 = oracleDataReader["FEE_ACT_NO2"]?.ToString(),
                                            AmtUpperBand = oracleDataReader["AMT_UPP_BAND"]?.ToString(),
                                            AmtLowerBand = oracleDataReader["AMT_LWR_BAND"]?.ToString(),
                                            FlatRate = oracleDataReader["FLAT_RATE"]?.ToString(),
                                            CatClassif = oracleDataReader["CAT_CLASSIF"]?.ToString(),
                                            Percent = oracleDataReader["PERCENT"]?.ToString(),
                                            DelFlag = oracleDataReader["DEL_FLG"]?.ToString(),
                                            RcreUserId = oracleDataReader["RCRE_USER_ID"]?.ToString(),
                                            RcreTime = oracleDataReader["RCRE_TIME"]?.ToString(),
                                            LchgUserId = oracleDataReader["LCHG_USER_ID"]?.ToString(),
                                            LchgTime = oracleDataReader["LCHG_TIME"]?.ToString(),
                                            BankId = oracleDataReader["BANK_ID"]?.ToString()
                                        };

                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                        }

                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return fee;
        }
        public static long InsertFintechFees(Fee fee, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("FinacleConnection")))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = new OracleCommand("INSERT into custom.FINTECH" +
                        "(" +
                        "TERM_ID, " +
                        "TILL_ACCT_NO, " +
                        "FEE_ACT_NO1, " +
                        "FEE_ACT_NO2, " +
                        "AMT_UPP_BAND, " +
                        "AMT_LWR_BAND, " +
                        "FLAT_RATE, " +
                        "CAT_CLASSIF, " +
                        "PERCENT, " +
                        "DEL_FLG, " +
                        "RCRE_USER_ID, " +
                        "RCRE_TIME, " +
                        //"LCHG_USER_ID, " +
                        //"LCHG_TIME, " +
                        "BANK_ID" +
                        ") " +
                        "VALUES" +
                        "(" +
                        ":TermId, " +
                        ":TillAccountNo, " +
                        ":FeeActNo1, " +
                        ":FeeActNo2, " +
                        ":AmtUpperBand, " +
                        ":AmtLowerBand, " +
                        ":FlatRate, " +
                        ":CatClassif, " +
                        ":Percent, " +
                        ":DelFlag, " +
                        ":RcreUserId, " +
                        ":RcreTime, " +
                        //":LchgUserId, " +
                        //":LchgTime, " +
                        ":BankId" +
                        ")", oracleConnection))
                    {
                        oracleCommand.Parameters.Add(new OracleParameter("TermId", OracleDbType.Varchar2)).Value = fee.TermId;
                        oracleCommand.Parameters.Add(new OracleParameter("TillAccountNo", OracleDbType.Varchar2)).Value = fee.TillAccountNo;
                        oracleCommand.Parameters.Add(new OracleParameter("FeeActNo1", OracleDbType.Varchar2)).Value = fee.FeeActNo1;
                        oracleCommand.Parameters.Add(new OracleParameter("FeeActNo2", OracleDbType.Varchar2)).Value = fee.FeeActNo2;
                        oracleCommand.Parameters.Add(new OracleParameter("AmtUpperBand", OracleDbType.Decimal)).Value = fee.AmtUpperBand;
                        oracleCommand.Parameters.Add(new OracleParameter("AmtLowerBand", OracleDbType.Decimal)).Value = fee.AmtLowerBand;
                        oracleCommand.Parameters.Add(new OracleParameter("FlatRate", OracleDbType.Varchar2)).Value = fee.FlatRate;
                        oracleCommand.Parameters.Add(new OracleParameter("CatClassif", OracleDbType.Char)).Value = fee.CatClassif;
                        oracleCommand.Parameters.Add(new OracleParameter("Percent", OracleDbType.Decimal)).Value = fee.Percent;
                        oracleCommand.Parameters.Add(new OracleParameter("DelFlag", OracleDbType.Char)).Value = fee.DelFlag;
                        oracleCommand.Parameters.Add(new OracleParameter("RcreUserId", OracleDbType.Varchar2)).Value = fee.RcreUserId;
                        oracleCommand.Parameters.Add(new OracleParameter("RcreTime", OracleDbType.Date)).Value = fee.RcreTime;
                        //oracleCommand.Parameters.Add(new OracleParameter("LchgUserId", OracleDbType.Varchar2)).Value = fee.LchgUserId;
                        //oracleCommand.Parameters.Add(new OracleParameter("LchgTime", OracleDbType.Varchar2)).Value = fee.LchgTime;
                        oracleCommand.Parameters.Add(new OracleParameter("BankId", OracleDbType.Varchar2)).Value = fee.BankId;
                        
                        result = oracleCommand.ExecuteNonQuery();
                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long UpdateFintechFees(Fee fee, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("FinacleConnection")))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = new OracleCommand("UPDATE custom.FINTECH " +
                        "SET " +
                        "TILL_ACCT_NO = :TillAccountNo, " +
                        "AMT_UPP_BAND = :AmtUpperBand, " +
                        "FLAT_RATE = :FlatRate, " +
                        "PERCENT = :Percent, " +
                        "LCHG_USER_ID = :LchgUserId, " +
                        "LCHG_TIME = :LchgTime " +
                        "WHERE " +
                        "TERM_ID = :TermId", oracleConnection))
                    {
                        oracleCommand.Parameters.Add(new OracleParameter("TillAccountNo", OracleDbType.Varchar2)).Value = fee.TillAccountNo;
                        oracleCommand.Parameters.Add(new OracleParameter("AmtUpperBand", OracleDbType.Decimal)).Value = fee.AmtUpperBand;
                        oracleCommand.Parameters.Add(new OracleParameter("FlatRate", OracleDbType.Varchar2)).Value = fee.FlatRate;
                        oracleCommand.Parameters.Add(new OracleParameter("Percent", OracleDbType.Decimal)).Value = fee.Percent;
                        oracleCommand.Parameters.Add(new OracleParameter("LchgUserId", OracleDbType.Varchar2)).Value = fee.LchgUserId;
                        oracleCommand.Parameters.Add(new OracleParameter("LchgTime", OracleDbType.Date)).Value = fee.LchgTime;
                        oracleCommand.Parameters.Add(new OracleParameter("TermId", OracleDbType.Varchar2)).Value = fee.TermId;

                        result = oracleCommand.ExecuteNonQuery();
                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        
        //
        public static List<SalProfiling> GetAllFromSalProfiling(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<SalProfiling> salProfilings = null;

            try
            {
                //using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection")))
                //{
                //    if (oracleConnection.State != ConnectionState.Open)
                //    {
                //        oracleConnection.Open();
                //    }

                //    using (var oracleCommand = oracleConnection.CreateCommand())
                //    {
                //        //var oracleParameters = new OracleParameter[]
                //        //{
                //        //    new OracleParameter(":FintechTermId", fintechFinacleTermId.ToUpper())
                //        //};

                //        var query = $"SELECT * from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling order by INSERTED_DATE desc";
                //        //if (!string.IsNullOrEmpty(fintechFinacleTermId))
                //        //{
                //        //    query = $"{query} AND cc.term_id = :FintechTermId order by PSTD_DATE";
                //        //    oracleCommand.Parameters.AddRange(oracleParameters);
                //        //}
                //        //else
                //        //{
                //        //    query = $"{query} order by PSTD_DATE";
                //        //}

                //        oracleCommand.CommandText = query;
                //        oracleCommand.CommandType = CommandType.Text;

                //        try
                //        {
                //            using (var oracleDataReader = oracleCommand.ExecuteReader())
                //            {
                //                if (oracleDataReader.HasRows)
                //                {
                //                    salProfilings = new List<SalProfiling>();

                //                    while (oracleDataReader.Read())
                //                    {
                //                        var salProfiling = new SalProfiling
                //                        {
                //                            CifId = oracleDataReader["CIF_ID"]?.ToString(),
                //                            Foracid = oracleDataReader["FORACID"]?.ToString(),
                //                            LastName = oracleDataReader["LAST_NAME"]?.ToString(),
                //                            FirstName = oracleDataReader["FIRST_NAME"]?.ToString(),
                //                            Middlename = oracleDataReader["Middlename"]?.ToString(),
                //                            Bvn = oracleDataReader["BVN"]?.ToString(),
                //                            DateOfBirth = oracleDataReader["DATE_OF_BIRTH"]?.ToString(),
                //                            Average = "",//oracleDataReader["AVERAGE"]?.ToString(),
                //                            StrdDev = "",//oracleDataReader["STRD_DEV"]?.ToString(),
                //                            Cov = "",//oracleDataReader["COV"]?.ToString(),
                //                            ModeStat = oracleDataReader["MODE_STAT"]?.ToString(),
                //                            MaxVal = oracleDataReader["MAX_VAL"]?.ToString(),
                //                            MinVal = oracleDataReader["MIN_VAL"]?.ToString(),
                //                            MostFreqNarr = oracleDataReader["MOST_FREQ_NARRA"]?.ToString(),

                //                            MostFreqTranDate = oracleDataReader["MOST_FREQ_TRAN_DATE"]?.ToString(),
                //                            FirstMonth = oracleDataReader["FIRST_MONTH"]?.ToString(),
                //                            SecondMonth = oracleDataReader["SECOND_MONTH"]?.ToString(),
                //                            ThirdMonth = oracleDataReader["THIRD_MONTH"]?.ToString(),
                //                            FourthMonth = oracleDataReader["FOURTH_MONTH"]?.ToString(),
                //                            FifthMonth = oracleDataReader["FIFTH_MONTH"]?.ToString(),
                //                            SixthMonth = oracleDataReader["SIXTH_MONTH"]?.ToString(),
                //                            Src = oracleDataReader["SRC"]?.ToString(),
                //                            InsertedDate = oracleDataReader["INSERTED_DATE"]?.ToString(),

                //                            IsValidBvn = oracleDataReader["IS_VALID_BVN"]?.ToString(),
                //                            BvnCheckDate = oracleDataReader["BVN_CHECK_DATE"]?.ToString(),
                //                            IsValidCRMS = oracleDataReader["IS_VALID_CRMS"]?.ToString(),
                //                            CRMSCheckDate = oracleDataReader["CRMS_CHECK_DATE"]?.ToString(),
                //                            IsValidCRC = oracleDataReader["IS_VALID_CRC"]?.ToString(),
                //                            CRCSCheckDate = oracleDataReader["CRC_CHECK_DATE"]?.ToString(),
                //                        };

                //                        salProfilings.Add(salProfiling);
                //                    }
                //                }
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                //        }

                //    }

                //    oracleConnection.Close();
                //}

                //                            CifId = oracleDataReader["CIF_ID"]?.ToString(),
                //                            Foracid = oracleDataReader["FORACID"]?.ToString(),
                //                            LastName = oracleDataReader["LAST_NAME"]?.ToString(),
                //                            FirstName = oracleDataReader["FIRST_NAME"]?.ToString(),
                //                            Middlename = oracleDataReader["Middlename"]?.ToString(),
                //                            Bvn = oracleDataReader["BVN"]?.ToString(),
                //                            DateOfBirth = oracleDataReader["DATE_OF_BIRTH"]?.ToString(),
                //                            Average = "",//oracleDataReader["AVERAGE"]?.ToString(),
                //                            StrdDev = "",//oracleDataReader["STRD_DEV"]?.ToString(),
                //                            Cov = "",//oracleDataReader["COV"]?.ToString(),
                //                            ModeStat = oracleDataReader["MODE_STAT"]?.ToString(),
                //                            MaxVal = oracleDataReader["MAX_VAL"]?.ToString(),
                //                            MinVal = oracleDataReader["MIN_VAL"]?.ToString(),
                //                            MostFreqNarr = oracleDataReader["MOST_FREQ_NARRA"]?.ToString(),

                //                            MostFreqTranDate = oracleDataReader["MOST_FREQ_TRAN_DATE"]?.ToString(),
                //                            FirstMonth = oracleDataReader["FIRST_MONTH"]?.ToString(),
                //                            SecondMonth = oracleDataReader["SECOND_MONTH"]?.ToString(),
                //                            ThirdMonth = oracleDataReader["THIRD_MONTH"]?.ToString(),
                //                            FourthMonth = oracleDataReader["FOURTH_MONTH"]?.ToString(),
                //                            FifthMonth = oracleDataReader["FIFTH_MONTH"]?.ToString(),
                //                            SixthMonth = oracleDataReader["SIXTH_MONTH"]?.ToString(),
                //                            Src = oracleDataReader["SRC"]?.ToString(),
                //                            InsertedDate = oracleDataReader["INSERTED_DATE"]?.ToString(),

                //                            IsValidBvn = oracleDataReader["IS_VALID_BVN"]?.ToString(),
                //                            BvnCheckDate = oracleDataReader["BVN_CHECK_DATE"]?.ToString(),
                //                            IsValidCRMS = oracleDataReader["IS_VALID_CRMS"]?.ToString(),
                //                            CRMSCheckDate = oracleDataReader["CRMS_CHECK_DATE"]?.ToString(),
                //                            IsValidCRC = oracleDataReader["IS_VALID_CRC"]?.ToString(),
                //                            CRCSCheckDate = oracleDataReader["CRC_CHECK_DATE"]?.ToString(),

                var query = 
                    $"SELECT " +
                    $"CIF_ID as CifId, " +
                    $"FORACID as Foracid, " +
                    $"LAST_NAME as LastName, " +
                    $"FIRST_NAME as FirstName, " +
                    $"MIDDLENAME as Middlename, " +
                    $"BVN as Bvn, " +
                    $"DATE_OF_BIRTH as DateOfBirth, " +
                    $"'' as Average, " +
                    $"'' as StrdDev, " +
                    $"'' as Cov, " +
                    $"MODE_STAT as ModeStat, " +
                    $"MAX_VAL as MaxVal, " +
                    $"MIN_VAL as MinVal, " +
                    $"MOST_FREQ_NARRA as MostFreqNarr, " +
                    $"MOST_FREQ_TRAN_DATE as MostFreqTranDate, " +
                    $"FIRST_MONTH as FirstMonth, " +
                    $"SECOND_MONTH as SecondMonth, " +
                    $"THIRD_MONTH as ThirdMonth, " +
                    $"FOURTH_MONTH as FourthMonth, " +
                    $"FIFTH_MONTH as FifthMonth, " +
                    $"SIXTH_MONTH as SixthMonth, " +
                    $"SRC as Src, " +
                    $"INSERTED_DATE as InsertedDate, " +
                    $"IS_VALID_BVN as IsValidBvn, " +
                    $"BVN_CHECK_DATE as BvnCheckDate, " +
                    $"IS_VALID_CRMS as IsValidCRMS, " +
                    $"CRMS_CHECK_DATE as CRMSCheckDate, " +
                    $"IS_VALID_CRC as IsValidCRC, " +
                    $"CRC_CHECK_DATE as CRCSCheckDate, " +
                    $"ACCT_STATUS as AccountStatus " +
                    $"from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling order by INSERTED_DATE desc";

                using (var db = new Data(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection"), 1))
                {
                    salProfilings = (List<SalProfiling>)db.QueryList<SalProfiling>(query);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return salProfilings;
        }
        public static List<SalProfiling> GetAllFromSalProfiling(string query, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<SalProfiling> salProfilings = null;

            try
            {
                //using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection")))
                //{
                //    if (oracleConnection.State != ConnectionState.Open)
                //    {
                //        oracleConnection.Open();
                //    }

                //    using (var oracleCommand = oracleConnection.CreateCommand())
                //    {
                //        //SELECT * from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_BVN IS NOT NULL order by INSERTED_DATE desc
                //        //SELECT * from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRMS IS NOT NULL order by INSERTED_DATE desc
                //        //SELECT * from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRC IS NOT NULL order by INSERTED_DATE desc

                //        oracleCommand.CommandText = query;
                //        oracleCommand.CommandType = CommandType.Text;

                //        try
                //        {
                //            using (var oracleDataReader = oracleCommand.ExecuteReader())
                //            {
                //                if (oracleDataReader.HasRows)
                //                {
                //                    salProfilings = new List<SalProfiling>();

                //                    while (oracleDataReader.Read())
                //                    {
                //                        var salProfiling = new SalProfiling
                //                        {
                //                            CifId = oracleDataReader["CIF_ID"]?.ToString(),
                //                            Foracid = oracleDataReader["FORACID"]?.ToString(),
                //                            LastName = oracleDataReader["LAST_NAME"]?.ToString(),
                //                            FirstName = oracleDataReader["FIRST_NAME"]?.ToString(),
                //                            Middlename = oracleDataReader["Middlename"]?.ToString(),
                //                            Bvn = oracleDataReader["BVN"]?.ToString(),
                //                            DateOfBirth = oracleDataReader["DATE_OF_BIRTH"]?.ToString(),
                //                            Average = "",//oracleDataReader["AVERAGE"]?.ToString(),
                //                            StrdDev = "",//oracleDataReader["STRD_DEV"]?.ToString(),
                //                            Cov = "",//oracleDataReader["COV"]?.ToString(),
                //                            ModeStat = oracleDataReader["MODE_STAT"]?.ToString(),
                //                            MaxVal = oracleDataReader["MAX_VAL"]?.ToString(),
                //                            MinVal = oracleDataReader["MIN_VAL"]?.ToString(),
                //                            MostFreqNarr = oracleDataReader["MOST_FREQ_NARRA"]?.ToString(),

                //                            MostFreqTranDate = oracleDataReader["MOST_FREQ_TRAN_DATE"]?.ToString(),
                //                            FirstMonth = oracleDataReader["FIRST_MONTH"]?.ToString(),
                //                            SecondMonth = oracleDataReader["SECOND_MONTH"]?.ToString(),
                //                            ThirdMonth = oracleDataReader["THIRD_MONTH"]?.ToString(),
                //                            FourthMonth = oracleDataReader["FOURTH_MONTH"]?.ToString(),
                //                            FifthMonth = oracleDataReader["FIFTH_MONTH"]?.ToString(),
                //                            SixthMonth = oracleDataReader["SIXTH_MONTH"]?.ToString(),
                //                            Src = oracleDataReader["SRC"]?.ToString(),
                //                            InsertedDate = oracleDataReader["INSERTED_DATE"]?.ToString(),

                //                            IsValidBvn = oracleDataReader["IS_VALID_BVN"]?.ToString(),
                //                            BvnCheckDate = oracleDataReader["BVN_CHECK_DATE"]?.ToString(),
                //                            IsValidCRMS = oracleDataReader["IS_VALID_CRMS"]?.ToString(),
                //                            CRMSCheckDate = oracleDataReader["CRMS_CHECK_DATE"]?.ToString(),
                //                            IsValidCRC = oracleDataReader["IS_VALID_CRC"]?.ToString(),
                //                            CRCSCheckDate = oracleDataReader["CRC_CHECK_DATE"]?.ToString()
                //                        };

                //                        salProfilings.Add(salProfiling);
                //                    }
                //                }
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                //        }

                //    }

                //    oracleConnection.Close();
                //}

                var columns = $"CIF_ID as CifId, " +
                    $"FORACID as Foracid, " +
                    $"LAST_NAME as LastName, " +
                    $"FIRST_NAME as FirstName, " +
                    $"MIDDLENAME as Middlename, " +
                    $"BVN as Bvn, " +
                    $"DATE_OF_BIRTH as DateOfBirth, " +
                    $"'' as Average, " +
                    $"'' as StrdDev, " +
                    $"'' as Cov, " +
                    $"MODE_STAT as ModeStat, " +
                    $"MAX_VAL as MaxVal, " +
                    $"MIN_VAL as MinVal, " +
                    $"MOST_FREQ_NARRA as MostFreqNarr, " +
                    $"MOST_FREQ_TRAN_DATE as MostFreqTranDate, " +
                    $"FIRST_MONTH as FirstMonth, " +
                    $"SECOND_MONTH as SecondMonth, " +
                    $"THIRD_MONTH as ThirdMonth, " +
                    $"FOURTH_MONTH as FourthMonth, " +
                    $"FIFTH_MONTH as FifthMonth, " +
                    $"SIXTH_MONTH as SixthMonth, " +
                    $"SRC as Src, " +
                    $"INSERTED_DATE as InsertedDate, " +
                    $"IS_VALID_BVN as IsValidBvn, " +
                    $"BVN_CHECK_DATE as BvnCheckDate, " +
                    $"IS_VALID_CRMS as IsValidCRMS, " +
                    $"CRMS_CHECK_DATE as CRMSCheckDate, " +
                    $"IS_VALID_CRC as IsValidCRC, " +
                    $"CRC_CHECK_DATE as CRCSCheckDate, " +
                    $"ACCT_STATUS as AccountStatus ";

                query = query.Replace("[COLUMNS]", columns);

                using (var db = new Data(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection"), 1))
                {
                    salProfilings = (List<SalProfiling>)db.QueryList<SalProfiling>(query);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return salProfilings;
        }

        //
        public static long InsertSalProfiling(string connectionString, SalProfiling salProfiling, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var oracleConnection = new OracleConnection(connectionString))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = new OracleCommand($"INSERT into {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling" +
                        "(" +
                        "FORACID, " +
                        "CIF_ID, " +
                        "AVERAGE, " +
                        "MAX_VAL, " +
                        "MIN_VAL, " +
                        "FIRST_MONTH, " +
                        "SECOND_MONTH, " +
                        "THIRD_MONTH, " +
                        "FOURTH_MONTH, " +
                        "FIFTH_MONTH, " +
                        "SIXTH_MONTH, " +
                        "SRC, " +
                        "BVN, " +
                        "FIRST_NAME, " +
                        "MIDDLENAME, " +
                        "LAST_NAME, " +
                        "DATE_OF_BIRTH, " +
                        "INSERTED_DATE" +
                        ") " +
                        "VALUES" +
                        "(" +
                        ":Foracid, " +
                        ":CifId, " +
                        ":Average, " +
                        ":MaxVal, " +
                        ":MinVal, " +
                        ":FirstMonth, " +
                        ":SecondMonth, " +
                        ":ThirdMonth, " +
                        ":FourthMonth, " +
                        ":FifthMonth, " +
                        ":SixthMonth, " +
                        ":Src, " +
                        ":Bvn, " +
                        ":FirstName, " +
                        ":Middlename, " +
                        ":LastName, " +
                        ":DateOfBirth, " +
                        ":InsertedDate" +
                        ")", oracleConnection))
                    {
                        oracleCommand.Parameters.Add(new OracleParameter("Foracid", OracleDbType.Varchar2)).Value = salProfiling.Foracid;
                        oracleCommand.Parameters.Add(new OracleParameter("CifId", OracleDbType.Varchar2)).Value = salProfiling.CifId;
                        oracleCommand.Parameters.Add(new OracleParameter("Average", OracleDbType.Decimal)).Value = salProfiling.Average;
                        oracleCommand.Parameters.Add(new OracleParameter("MaxVal", OracleDbType.Decimal)).Value = salProfiling.MaxVal;
                        oracleCommand.Parameters.Add(new OracleParameter("MinVal", OracleDbType.Decimal)).Value = salProfiling.MinVal;
                        //oracleCommand.Parameters.Add(new OracleParameter("MostFreqTranDate", OracleDbType.Decimal)).Value = salProfiling.MostFreqTranDate;
                        oracleCommand.Parameters.Add(new OracleParameter("FirstMonth", OracleDbType.Decimal)).Value = salProfiling.FirstMonth;
                        oracleCommand.Parameters.Add(new OracleParameter("SecondMonth", OracleDbType.Decimal)).Value = salProfiling.SecondMonth;
                        oracleCommand.Parameters.Add(new OracleParameter("ThirdMonth", OracleDbType.Decimal)).Value = salProfiling.ThirdMonth;
                        oracleCommand.Parameters.Add(new OracleParameter("FourthMonth", OracleDbType.Decimal)).Value = salProfiling.FourthMonth;
                        oracleCommand.Parameters.Add(new OracleParameter("FifthMonth", OracleDbType.Decimal)).Value = salProfiling.FifthMonth;
                        oracleCommand.Parameters.Add(new OracleParameter("SixthMonth", OracleDbType.Decimal)).Value = salProfiling.SixthMonth;
                        oracleCommand.Parameters.Add(new OracleParameter("Src", OracleDbType.Varchar2)).Value = salProfiling.Src;
                        oracleCommand.Parameters.Add(new OracleParameter("Bvn", OracleDbType.Varchar2)).Value = salProfiling.Bvn;
                        oracleCommand.Parameters.Add(new OracleParameter("FirstName", OracleDbType.Varchar2)).Value = salProfiling.FirstName;
                        oracleCommand.Parameters.Add(new OracleParameter("Middlename", OracleDbType.Varchar2)).Value = salProfiling.Middlename;
                        oracleCommand.Parameters.Add(new OracleParameter("LastName", OracleDbType.Varchar2)).Value = salProfiling.LastName;
                        oracleCommand.Parameters.Add(new OracleParameter("DateOfBirth", OracleDbType.Varchar2)).Value = salProfiling.DateOfBirth;
                        oracleCommand.Parameters.Add(new OracleParameter("InsertedDate", OracleDbType.Varchar2)).Value = salProfiling.InsertedDate;

                        result = oracleCommand.ExecuteNonQuery();
                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long UpdateSalProfiling(string connectionString, SalProfiling salProfiling, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var oracleConnection = new OracleConnection(connectionString))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    var query = $"UPDATE {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling " +
                        "SET " +
                        "IS_VALID_CRMS = :IsValidCRMS, " +
                        "CRMS_CHECK_DATE = :CRMSCheckDate, " +
                        "IS_VALID_BVN = :IsValidBVN, " +
                        "BVN_CHECK_DATE = :BVNCheckDate, " +
                        "IS_VALID_CRC = :IsValidCRC, " +
                        "CRC_CHECK_DATE = :CRCCheckDate " +
                        "WHERE " +
                        "FORACID = :Foracid AND CIF_ID = :CifId";

                    //LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, query);
                    //LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, JsonConvert.SerializeObject(salProfiling));

                    using (var oracleCommand = new OracleCommand(query, oracleConnection))
                    {
                        oracleCommand.Parameters.Add(new OracleParameter("IsValidCRMS", OracleDbType.Varchar2)).Value = salProfiling.IsValidCRMS;
                        oracleCommand.Parameters.Add(new OracleParameter("CRMSCheckDate", OracleDbType.Varchar2)).Value = DateTime.Now.ToString("dd-MMM-yy");
                        oracleCommand.Parameters.Add(new OracleParameter("IsValidBVN", OracleDbType.Varchar2)).Value = salProfiling.IsValidBvn;
                        oracleCommand.Parameters.Add(new OracleParameter("BVNCheckDate", OracleDbType.Varchar2)).Value = DateTime.Now.ToString("dd-MMM-yy");
                        oracleCommand.Parameters.Add(new OracleParameter("IsValidCRC", OracleDbType.Varchar2)).Value = salProfiling.IsValidCRC;
                        oracleCommand.Parameters.Add(new OracleParameter("CRCCheckDate", OracleDbType.Varchar2)).Value = DateTime.Now.ToString("dd-MMM-yy");
                        oracleCommand.Parameters.Add(new OracleParameter("Foracid", OracleDbType.Varchar2)).Value = salProfiling.Foracid;
                        oracleCommand.Parameters.Add(new OracleParameter("CifId", OracleDbType.Varchar2)).Value = salProfiling.CifId;

                        result = oracleCommand.ExecuteNonQuery();
                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }

        public static SalProfiling GetFromSalProfilingWithAccountNumber(string connectionString, string accountNumber, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            SalProfiling salProfiling = null;

            try
            {
                using (var oracleConnection = new OracleConnection(connectionString))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = oracleConnection.CreateCommand())
                    {
                        var oracleParameters = new OracleParameter[]
                        {
                            new OracleParameter(":Foracid", accountNumber)
                        };

                        oracleCommand.CommandType = CommandType.Text;
                        oracleCommand.CommandText = $"SELECT * from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling WHERE FORACID = :Foracid";
                        oracleCommand.Parameters.AddRange(oracleParameters);

                        try
                        {
                            using (var oracleDataReader = oracleCommand.ExecuteReader())
                            {
                                if (oracleDataReader.HasRows)
                                {
                                    while (oracleDataReader.Read())
                                    {
                                        salProfiling = new SalProfiling
                                        {
                                            CifId = oracleDataReader["CIF_ID"]?.ToString(),
                                            Foracid = oracleDataReader["FORACID"]?.ToString(),
                                            LastName = oracleDataReader["LAST_NAME"]?.ToString(),
                                            FirstName = oracleDataReader["FIRST_NAME"]?.ToString(),
                                            Middlename = oracleDataReader["Middlename"]?.ToString(),
                                            Bvn = oracleDataReader["BVN"]?.ToString(),
                                            DateOfBirth = oracleDataReader["DATE_OF_BIRTH"]?.ToString(),
                                            Average = "",//oracleDataReader["AVERAGE"]?.ToString(),
                                            StrdDev = "",//oracleDataReader["STRD_DEV"]?.ToString(),
                                            Cov = "",//oracleDataReader["COV"]?.ToString(),
                                            ModeStat = oracleDataReader["MODE_STAT"]?.ToString(),
                                            MaxVal = oracleDataReader["MAX_VAL"]?.ToString(),
                                            MinVal = oracleDataReader["MIN_VAL"]?.ToString(),
                                            MostFreqNarr = oracleDataReader["MOST_FREQ_NARRA"]?.ToString(),

                                            MostFreqTranDate = oracleDataReader["MOST_FREQ_TRAN_DATE"]?.ToString(),
                                            FirstMonth = oracleDataReader["FIRST_MONTH"]?.ToString(),
                                            SecondMonth = oracleDataReader["SECOND_MONTH"]?.ToString(),
                                            ThirdMonth = oracleDataReader["THIRD_MONTH"]?.ToString(),
                                            FourthMonth = oracleDataReader["FOURTH_MONTH"]?.ToString(),
                                            FifthMonth = oracleDataReader["FIFTH_MONTH"]?.ToString(),
                                            SixthMonth = oracleDataReader["SIXTH_MONTH"]?.ToString(),
                                            Src = oracleDataReader["SRC"]?.ToString(),
                                            InsertedDate = oracleDataReader["INSERTED_DATE"]?.ToString(),

                                            IsValidBvn = oracleDataReader["IS_VALID_BVN"]?.ToString(),
                                            BvnCheckDate = oracleDataReader["BVN_CHECK_DATE"]?.ToString(),
                                            IsValidCRMS = oracleDataReader["IS_VALID_CRMS"]?.ToString(),
                                            CRMSCheckDate = oracleDataReader["CRMS_CHECK_DATE"]?.ToString(),
                                            IsValidCRC = oracleDataReader["IS_VALID_CRC"]?.ToString(),
                                            CRCSCheckDate = oracleDataReader["CRC_CHECK_DATE"]?.ToString(),
                                        };

                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                        }
                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return salProfiling;
        }        
        public static List<LoanExposure> GetAllFromLoanExposure(List<string> accountNumbers,string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<LoanExposure> loanExposures = null;

            try
            {
                var query = $"SELECT " +
                            $"SCHM_CODE as SchemeCode, " +
                            $"LOAN_ACCT as LoanAccount, " +
                            $"OP_ACCT as OperationalAccount, " +
                            $"ACCT_NAME as AccountName, " +
                            $"REF_DESC as ReferenceDescription, " +
                            $"PRINCIPAL as Principal, " +
                            $"PRIN_REPAYMENT as PrincipalRepayment, " +
                            $"INT_REPAYMENT as InterestRepayment, " +
                            $"INTEREST as Interest, " +
                            $"APPROVED_DATE as DateApproved, " +
                            $"DISB_DATE as DateDisbursed, " +
                            $"OUTANDING_BAL as OutstandingBalance, " +
                            $"FREQ as Frequency, " +
                            $"TENOR as Tenor, " +
                            $"OUST_TENOR as OutstandingTenor, " +
                            $"LOAN_SOURCE as LoanSource " +
                            $"from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.MIS_SAL_LOAN ";

                if (accountNumbers.Count > 0)
                {
                    query = $"{query} where OP_ACCT IN (?) ";

                    var joinString = "";

                    foreach (var accountNumber in accountNumbers)
                    {
                        joinString += $"'{accountNumber}',";
                    }

                    if (!string.IsNullOrEmpty(joinString))
                    {
                        joinString = joinString.Substring(0, joinString.Length - 1);
                    }

                    query = query.Replace("?", joinString);
                }

                query = $"{query} order by DISB_DATE desc";

                //using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection")))
                //{
                //    if (oracleConnection.State != ConnectionState.Open)
                //    {
                //        oracleConnection.Open();
                //    }

                //    using (var oracleCommand = oracleConnection.CreateCommand())
                //    {
                //        oracleCommand.CommandType = CommandType.Text;

                //        var query = $"SELECT * from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.MIS_SAL_LOAN ";

                //        if (accountNumbers.Count > 0)
                //        {
                //            query = $"{query} where OP_ACCT IN (?) ";

                //            var joinString = "";

                //            foreach (var accountNumber in accountNumbers)
                //            {
                //                joinString += $"'{accountNumber}',";
                //            }

                //            if (!string.IsNullOrEmpty(joinString))
                //            {
                //                joinString = joinString.Substring(0, joinString.Length - 1);
                //            }

                //            query = query.Replace("?", joinString);
                //        }

                //        query = $"{query} order by DISB_DATE desc";

                //        LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, query);

                //        oracleCommand.CommandText = query;

                //        //var parameterNames = accountNumbers.Select((s, i) => ":tag" + i.ToString()).ToArray();
                //        //var inClause = string.Join(", ", parameterNames);
                //        //for (int i = 0; i < parameterNames.Length; i++)
                //        //{
                //        //    oracleCommand.Parameters.Add(new OracleParameter(parameterNames[i], accountNumbers[i]));
                //        //}

                //        try
                //        {
                //            using (var oracleDataReader = oracleCommand.ExecuteReader())
                //            {
                //                if (oracleDataReader.HasRows)
                //                {
                //                    loanExposures = new List<LoanExposure>();

                //                    while (oracleDataReader.Read())
                //                    {
                //                        var loanExposure = new LoanExposure
                //                        {
                //                            SchemeCode = oracleDataReader["SCHM_CODE"]?.ToString(),
                //                            LoanAccount = oracleDataReader["LOAN_ACCT"]?.ToString(),
                //                            OperationalAccount = oracleDataReader["OP_ACCT"]?.ToString(),
                //                            AccountName = oracleDataReader["ACCT_NAME"]?.ToString(),
                //                            ReferenceDescription = oracleDataReader["REF_DESC"]?.ToString(),
                //                            Principal = oracleDataReader["PRINCIPAL"]?.ToString(),
                //                            PrincipalRepayment = oracleDataReader["PRIN_REPAYMENT"]?.ToString(),
                //                            InterestRepayment = oracleDataReader["INT_REPAYMENT"]?.ToString(),
                //                            Interest = oracleDataReader["INTEREST"]?.ToString(),
                //                            DateApproved = oracleDataReader["APPROVED_DATE"]?.ToString(),
                //                            DateDisbursed = oracleDataReader["DISB_DATE"]?.ToString(),
                //                            OutstandingBalance = oracleDataReader["OUTANDING_BAL"]?.ToString(),
                //                            Frequency = oracleDataReader["FREQ"]?.ToString(),
                //                            Tenor = oracleDataReader["TENOR"]?.ToString(),
                //                            OutstandingTenor = oracleDataReader["OUST_TENOR"]?.ToString(),
                //                            LoanSource = oracleDataReader["LOAN_SOURCE"]?.ToString()
                //                        };

                //                        loanExposures.Add(loanExposure);
                //                    }
                //                }
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                //        }

                //    }

                //    oracleConnection.Close();
                //}

                using (var db = new Data(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection"), 1))
                {
                    loanExposures = (List<LoanExposure>)db.QueryList<LoanExposure>(query);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return loanExposures;
        }
        public static SodaRac GetFromSodaRacWithAccountNumber(string accountNumber, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            SodaRac sodaRac = null;

            try
            {
                using (var oracleConnection = new OracleConnection(ConfigurationUtility.GetConnectionStringValue("RacDbConnection")))
                {
                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        oracleConnection.Open();
                    }

                    using (var oracleCommand = oracleConnection.CreateCommand())
                    {
                        var oracleParameters = new OracleParameter[]
                        {
                            new OracleParameter(":Foracid", accountNumber)
                        };

                        oracleCommand.CommandType = CommandType.Text;
                        oracleCommand.CommandText = $"SELECT * from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.soda_rac WHERE SALACCT_1 = :Foracid";
                        oracleCommand.Parameters.AddRange(oracleParameters);

                        try
                        {
                            using (var oracleDataReader = oracleCommand.ExecuteReader())
                            {
                                if (oracleDataReader.HasRows)
                                {
                                    while (oracleDataReader.Read())
                                    {
                                        sodaRac = new SodaRac
                                        {
                                            ProcessDate = oracleDataReader["PROCESS_DATE"]?.ToString(),
                                            CifId = oracleDataReader["CIF_ID"]?.ToString(),

                                            CustFirstName = oracleDataReader["CUST_FIRST_NAME"]?.ToString(),
                                            CustMiddleName = oracleDataReader["CUST_MIDDLE_NAME"]?.ToString(),
                                            CustLastName = oracleDataReader["CUST_LAST_NAME"]?.ToString(),

                                            Bvn = oracleDataReader["BVN"]?.ToString(),
                                            Gender = oracleDataReader["GENDER"]?.ToString(),
                                            Age = oracleDataReader["AGE"]?.ToString(),

                                            CrmEmail = oracleDataReader["CRM_EMAIL"]?.ToString(),
                                            AlertEmail = oracleDataReader["ALERT_EMAIL"]?.ToString(),
                                            CrmPhone = oracleDataReader["CRM_PHONE"]?.ToString(),

                                            AlertPhone = oracleDataReader["ALERT_PHONE"]?.ToString(),
                                            CrmAddress = oracleDataReader["CRM_ADDRESS"]?.ToString(),
                                            Occupation = oracleDataReader["OCCUPATION"]?.ToString(),

                                            AverageSalary = oracleDataReader["AVERAGE_SALARY"]?.ToString(),
                                            StaffStat = oracleDataReader["STAFF_STAT"]?.ToString(),
                                            SalAcct1 = oracleDataReader["SALACCT_1"]?.ToString(),

                                            SalAcct2 = oracleDataReader["SALACCT_2"]?.ToString(),
                                            SalAcct3 = oracleDataReader["SALACCT_3"]?.ToString(),
                                            LoanOutstandingBal = oracleDataReader["LOAN_OUSTANDING_BAL"]?.ToString(),

                                            Emi = oracleDataReader["EMI"]?.ToString(),
                                            EligAmt = oracleDataReader["ELIG_AMT"]?.ToString(),
                                            Rule1 = oracleDataReader["RULE1"]?.ToString(),

                                            Rule2 = oracleDataReader["RULE2"]?.ToString(),
                                            Rule3 = oracleDataReader["RULE3"]?.ToString(),
                                            Rule4 = oracleDataReader["RULE4"]?.ToString(),

                                            Rule5 = oracleDataReader["RULE5"]?.ToString(),
                                            Rule6 = oracleDataReader["RULE6"]?.ToString(),
                                            Rule7 = oracleDataReader["RULE7"]?.ToString(),

                                            Rule8 = oracleDataReader["RULE8"]?.ToString(),
                                            Rule9 = oracleDataReader["RULE9"]?.ToString(),
                                            Rule10 = oracleDataReader["RULE10"]?.ToString(),


                                            Rule11 = oracleDataReader["RULE11"]?.ToString(),
                                            Rule12 = oracleDataReader["RULE12"]?.ToString(),
                                            Rule13 = oracleDataReader["RULE13"]?.ToString(),

                                            Rule14 = oracleDataReader["RULE14"]?.ToString(),
                                            Rule15 = oracleDataReader["RULE15"]?.ToString(),
                                            OverallElig = oracleDataReader["OVERALL_ELIG"]?.ToString(),

                                            SalaryPayment1 = oracleDataReader["SALARYPAYMENT_1"]?.ToString(),
                                            SalaryPayment2 = oracleDataReader["SALARYPAYMENT_2"]?.ToString(),
                                            SalaryPayment3 = oracleDataReader["SALARYPAYMENT_3"]?.ToString(),

                                            SalaryPayment4 = oracleDataReader["SALARYPAYMENT_4"]?.ToString(),
                                            SalaryPayment5 = oracleDataReader["SALARYPAYMENT_5"]?.ToString(),
                                            SalaryPayment6 = oracleDataReader["SALARYPAYMENT_6"]?.ToString(),

                                            OtherAcctSavings = oracleDataReader["OTHERACCT_SAVINGS"]?.ToString(),
                                            OtherAcctCurrent = oracleDataReader["OTHERACCT_CURRENT"]?.ToString()
                                        };

                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
                        }

                    }

                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return sodaRac;
        }
        
        //
        public static long GetAllCountFromSalProfiling(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var result = (long)0;

            try
            {
                using (var db = new Data(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection"), 1))
                {
                    var queryResult = db.Query<dynamic>($"SELECT count(*) as TOTAL_COUNT from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling");
                    result = Convert.ToInt64(queryResult.TOTAL_COUNT);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long GetAllCountFromSalProfiling(string query, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var result = (long)0;

            try
            {
                using (var db = new Data(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection"), 1))
                {
                    var queryResult = db.Query<dynamic>(query);
                    result = Convert.ToInt64(queryResult.TOTAL_COUNT);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
    }
}
