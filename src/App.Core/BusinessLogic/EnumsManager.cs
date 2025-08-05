namespace App.Core.BusinessLogic
{
    public enum UserAccountAuthenticationType
    {
        LocalAccountAuthentication = 0,
        ActiveDirectoryAuthentication = 1
    }
    public enum AlertType
    {
        Information = 1,
        Success = 2,
        Warning = 3,
        Error = 4
    }
    public enum UserAccountStatus
    {
        Inactive = 0,
        Active = 1,
        LoggedIn = 2,
        LockedOut = 3,
        Dormant = 4,
        Deleted = 5
    }
    public enum MakerCheckerCategory
    {
        Role = 1,
        User = 2,
        RoleAccess = 3,
        Fintech = 4,
        FintechContactPerson = 5,
        AccountEmployerUpdate = 6,
        RacProfiling = 7
    }
    public enum MakerCheckerType
    {
        CreateNewRole = 11,
        EditExistingRole = 12,
        CreateNewUser = 21,
        EditExistingUser = 22,
        AssignRoleAccess = 31,
        CreateNewFintechProfile = 41,
        EditExistingFintechProfile = 42,
        CreateNewFintechContactPerson = 51,
        EditExistingFintechContactPerson = 52,
        RemoveExistingFintechContactPerson = 53,
        UpdateEmployerDetails = 61,
        ProfileIdentifiedSalaryAccount = 71
    }
    public enum MakerCheckerStatus
    {
        Initiated = 0,
        Approved = 1,
        Rejected = -1
    }
    public enum ReportAction
    {
        SpoolUserAccessList = 1,
        SpoolApplicationAuditList = 2,
        SpoolSettlementReport = 3,
        SpoolTransactionReport = 4,
        SpoolAppUsersReport = 5,
        SpoolEmployerProfiledSalaryAccountsReport = 6,
        SpoolIdentifiedSalaryAccountsReport = 7
    }
    public enum PersonType
    {
        BankUser = 0,
        FintechContactPerson = 1,
        BankRelationshipManager = 2
    }
    public enum EmailType
    {
        LoginNotification = 1,
        PendingItemsNotification = 2,
        ApprovalNotification = 3,
        RejectionNotification = 4,
        AccountCreationNotification = 5,
        AccountResetNotification = 6,
        ForgotPasswordNotification = 7
    }
    public enum FeeOperationName
    {
        Insert = 1,
        Update = 2
    }
    public enum FeeOperationStatus
    {
        Pending = 0,
        Existing = 1,
        NonExistent = 2,
        Inserted = 3,
        Updated = 4
    }
    public enum CRCResponseType
    {
        ResponseDataPacket = 1,
        ResponseNoHit = 2,
        ResponseSearchList = 3
    }
    public enum SalaryAccountsRacProfileStatus
    {
        SalaryPaymentHistoryCountLessOrEqualToZero = -4,
        UnableToGetBvnWithAccountNumber = -3,
        UnableToGetCustomerDetails = -2,
        Existent = -1,
        Pending = 0,
        Profiled = 1
    }
}