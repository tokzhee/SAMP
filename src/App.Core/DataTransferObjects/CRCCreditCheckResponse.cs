using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DataTransferObjects
{
    public class CRCCreditCheckResponse
    {
        public ConsumerNoHitResponse ConsumerNoHitResponse { get; set; }
        public ConsumerHitResponse ConsumerHitResponse { get; set; }
        public ConsumerSearchResultResponse ConsumerSearchResultResponse { get; set; }
    }
    public class ConsumerNoHitResponse
    {
        public BODY BODY { get; set; }
        public HEADER HEADER { get; set; }
        public string REQUESTID { get; set; }
    }
    public class ConsumerHitResponse
    {
        public BODY BODY { get; set; }
        public HEADER HEADER { get; set; }
        public string REQUESTID { get; set; }
    }
    public class ConsumerSearchResultResponse
    {
        public BODY BODY { get; set; }
        public HEADER HEADER { get; set; }
        public string REFERENCENO { get; set; }
        public string REQUESTID { get; set; }
    }
    public class BODY
    {
        public NOHIT NOHIT { get; set; }
        public object AddressHistory { get; set; }
        public object Amount_OD_BucketCURR1 { get; set; }
        public CONSUMERRELATION CONSUMER_RELATION { get; set; }
        public CREDITMICROSUMMARY CREDIT_MICRO_SUMMARY { get; set; }
        public CREDITNANOSUMMARY CREDIT_NANO_SUMMARY { get; set; }
        public object CREDIT_SCORE_DETAILS { get; set; }
        public object ClassificationInsType { get; set; }
        public object ClassificationProdType { get; set; }
        public object ClosedAccounts { get; set; }
        public object ConsCommDetails { get; set; }
        public object ConsumerMergerDetails { get; set; }
        public object ContactHistory { get; set; }
        public object CreditDisputeDetails { get; set; }
        public object CreditFacilityHistory24 { get; set; }
        public object CreditProfileOverview { get; set; }
        public object CreditProfileSummaryCURR1 { get; set; }
        public object CreditProfileSummaryCURR2 { get; set; }
        public object CreditProfileSummaryCURR3 { get; set; }
        public object CreditProfileSummaryCURR4 { get; set; }
        public object CreditProfileSummaryCURR5 { get; set; }
        public object DMMDisputeSection { get; set; }
        public object DODishonoredChequeDetails { get; set; }
        public object DOJointHolderDetails { get; set; }
        public object DOLitigationDetails { get; set; }
        public object DisclaimerDetails { get; set; }
        public object EmploymentHistory { get; set; }
        public object GuaranteedLoanDetails { get; set; }
        public object InquiryHistoryDetails { get; set; }
        public object Inquiry_Product { get; set; }
        public object LegendDetails { get; set; }
        public MFCREDITMICROSUMMARY MFCREDIT_MICRO_SUMMARY { get; set; }
        public MFCREDITNANOSUMMARY MFCREDIT_NANO_SUMMARY { get; set; }
        public MGCREDITMICROSUMMARY MGCREDIT_MICRO_SUMMARY { get; set; }
        public MGCREDITNANOSUMMARY MGCREDIT_NANO_SUMMARY { get; set; }
        public object MIC_CONSUMER_PROFILE { get; set; }
        public NANOCONSUMERPROFILE NANO_CONSUMER_PROFILE { get; set; }
        public object RelatedToDetails { get; set; }
        public object ReportDetail { get; set; }
        public object ReportDetailAcc { get; set; }
        public object ReportDetailBVN { get; set; }
        public object ReportDetailMob { get; set; }
        public object ReportDetailsSIR { get; set; }
        public object SecurityDetails { get; set; }
        public object SummaryOfPerformance { get; set; }
    }
    public class NOHIT
    {
        public DATAPACKET DATAPACKET { get; set; }
    }
    public class DATAPACKET
    {
        public ConsDisclaimer ConsDisclaimer { get; set; }
        public NoHitCONSUMERPROFILE NoHit_CONSUMER_PROFILE { get; set; }
        public NoHitSUBJECTDETAILS NoHit_SUBJECT_DETAILS { get; set; }
    }
    public class ConsDisclaimer
    {
        public GetDisclaimerContent GetDisclaimerContent { get; set; }
    }
    public class GetDisclaimerContent
    {
        public string Item1 { get; set; }
    }
    public class NoHitCONSUMERPROFILE
    {
    }
    public class NoHitSUBJECTDETAILS
    {
        public SUBJECTDETAILS SUBJECT_DETAILS { get; set; }
    }
    public class SUBJECTDETAILS
    {
    }
    public class CONSUMERRELATION
    {
    }
    public class CREDITMICROSUMMARY
    {
        public SUMMARY SUMMARY { get; set; }
    }
    public class CREDITNANOSUMMARY
    {
        public SUMMARY SUMMARY { get; set; }
    }
    public class SUMMARY
    {
        public string HAS_CREDITFACILITIES { get; set; }
        public string LAST_REPORTED_DATE { get; set; }
        public string NO_OF_DELINQCREDITFACILITIES { get; set; }
    }
    public class MFCREDITMICROSUMMARY
    {
        public SUMMARY SUMMARY { get; set; }
    }
    public class MFCREDITNANOSUMMARY
    {
        public SUMMARY SUMMARY { get; set; }
    }
    public class MGCREDITMICROSUMMARY
    {
        public SUMMARY SUMMARY { get; set; }
    }
    public class MGCREDITNANOSUMMARY
    {
        public SUMMARY SUMMARY { get; set; }
    }

    public class NANOCONSUMERPROFILE
    {
        public CONSUMERDETAILS CONSUMER_DETAILS { get; set; }
    }
    public class CONSUMERDETAILS
    {
        public string CITIZENSHIP { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string FIRST_NAME { get; set; }
        public string GENDER { get; set; }
        public List<IDENTIFICATION> IDENTIFICATION { get; set; }
        public object IDENTIFICATION_DETAILS { get; set; }
        public string LAST_NAME { get; set; }
        public string NAME { get; set; }
        public string RUID { get; set; }
    }
    public class IDENTIFICATION
    {
        public object EXP_DATE { get; set; }
        public bool EXP_DATESpecified { get; set; }
        public string ID_DISPLAY_NAME { get; set; }
        public string ID_VALUE { get; set; }
        public string RUID { get; set; }
        public string SOURCE_ID { get; set; }
    }
    public class HEADER
    {
        public REPORTHEADER REPORTHEADER { get; set; }
        public RESPONSETYPE RESPONSETYPE { get; set; }
        public SEARCHCRITERIA SEARCHCRITERIA { get; set; }
        public SEARCHRESULTLIST SEARCHRESULTLIST { get; set; }
    }
    public class REPORTHEADER
    {
        public string MAILTO { get; set; }
        public string PRODUCTNAME { get; set; }
        public REASON REASON { get; set; }
        public string REPORTDATE { get; set; }
        public string REPORTORDERNUMBER { get; set; }
        public string USERID { get; set; }
    }
    public class REASON
    {
    }
    public class RESPONSETYPE
    {
        public string CODE { get; set; }
        public string DESCRIPTION { get; set; }
    }
    public class SEARCHCRITERIA
    {
        public object BRANCHCODE { get; set; }
        public string BVN_NO { get; set; }
        public object CFACCOUNTNUMBER { get; set; }
        public object DATEOFBIRTH { get; set; }
        public object GENDER { get; set; }
        public object NAME { get; set; }
        public object TELEPHONE_NO { get; set; }
    }
    public class SEARCHRESULTLIST
    {
        public SEARCHRESULTITEM SEARCHRESULTITEM { get; set; }
    }
    public class SEARCHRESULTITEM
    {
        public ADDRESSES ADDRESSES { get; set; }
        public string BUREAUID { get; set; }
        public string CONFIDENCESCORE { get; set; }
        public IDENTIFIERS IDENTIFIERS { get; set; }
        public string NAME { get; set; }
        public SURROGATES SURROGATES { get; set; }
    }
    public class ADDRESSES
    {
    }

    public class IDENTIFIERS
    {
    }

    public class SURROGATES
    {
    }
    
}


