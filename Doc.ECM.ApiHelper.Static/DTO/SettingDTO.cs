namespace Doc.ECM.APIHelper.DTO
{
    public class SettingDTO
    {
        //Regional Settings
        public string DefaultDateFormat { get; set; }
        public string DefaultDecimalSeparator { get; set; }
        public string DefaultThousandSeparator { get; set; }
        public int DefaultLanguageID { get; set; }
        public string DefaultTimezone { get; set; }

        //Security
        public bool EnablePasswordExpiration { get; set; }
        public int PasswordExpirationDays { get; set; }
        public bool EnableAccountInactivity { get; set; }
        public int AccountInactivityDays { get; set; }
        public bool EnableFailedLoginAttempts { get; set; }
        public int FailedLoginAttempts { get; set; }
        public bool EnableTokenExpiration { get; set; }
        public int TokenExpirationMinutes { get; set; }
        public bool EnablePasswordStrength { get; set; }
        public int PasswordStrengthMinLength { get; set; }
        public int PasswordStrengthMaxLength { get; set; }
        public int PasswordStrengthSpecialLength { get; set; }
        public int PasswordStrengthUpperLength { get; set; }
        public int PasswordStrengthNumberLength { get; set; }


        //Search
        public int MaxSearchResults { get; set; }
        public bool SearchResultsOrderAsc { get; set; }
        public int SearchResultsVisibleFields { get; set; }
        public bool EnableDocumentPreAuthorization { get; set; }

        //Email
        public string EmailAccount { get; set; }
        public string EmailPassword { get; set; }
        public string EmailServer { get; set; }
        public int EmailPort { get; set; }
        public bool EmailUseSSL { get; set; }
        public string SendGridApiKey { get; set; }
        public string Body { get; set; }

        //DocuSign
        public string DocuSignAccountID { get; set; }
        public string DocuSignApiURL { get; set; }
        public string DocuSignIntegrationKey { get; set; }
        public string DocuSignRSAKey { get; set; }

        //Two Factor authentication
        public string TwoFactorAuthenticationServiceId { get; set; }
        public string TwoFactorAuthenticationAuthApiKey { get; set; }
    }
}
