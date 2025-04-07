using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc.ECM.APIHelper.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public int? LanguageID { get; set; }
        public string TimeZoneID { get; set; }
        public int? OrganizationID { get; set; }
        public bool? IsAdmin { get; set; }
        public bool IsActivated { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MobileNumber { get; set; }
        public string Civility { get; set; }
        public string Initials { get; set; }
        public string Job { get; set; }
        public string WelcomeMessage { get; set; }
        public string Email { get; set; }
        public string MetricDateFormat { get; set; }
        public string MetricDecimalSep { get; set; }
        public string MetricThousandSep { get; set; }
        public string OrganizationName { get; set; }
        public string ImgURL { get; set; }
        public List<UserSignatureDTO> Signatures { get; set; }
        public bool EntableTwoFactorAuthentication { get; set; }
        public string SignatureImagePath { get; set; }
        public string SignatureCertificatePath { get; set; }
        public string SwissIDAccountID { get; set; }

        public string CultureCode
        {
            get
            {
                switch (LanguageID)
                {
                    case 1:
                        return "de";
                    case 2:
                        return "en";
                    case 3:
                        return "es";
                    case 4:
                        return "fr";
                    case 5:
                        return "it";
                    case 6:
                        return "ro";
                    case 7:
                        return "pr";
                    default:
                        break;
                }

                return "fr";
            }
        }

        public AttachmentsDTO SignatureImage { get; set; }
        public AttachmentsDTO SignatureCertificate { get; set; }
    }

    public class UserSignatureDTO
    {
        public string Description { get; set; }
        public int Id { get; set; }
        public bool IsDefault { get; set; }
        public AttachmentsDTO SignatureImage { get; set; }
        public string SignatureImagePath { get; set; }

        public bool SignatureImageHasChanged { get; set; }

        public bool IncludeFrame { get; set; }
        public bool IncludeUserFullName { get; set; }
        public bool IncludeTimeStamp { get; set; }
        public double? ImagePositionX { get; set; }
        public double? ImagePositionY { get; set; }
        public double? ImageSizeX { get; set; }
        public double? ImageSizeY { get; set; }
    }
    public class ReadUsersResponseDTO
    {
        public List<UserDTO> Data { get; set; }
    }
}
