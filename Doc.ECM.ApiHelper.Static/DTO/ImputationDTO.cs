using System.Collections.Generic;

namespace Doc.ECM.APIHelper.DTO
{
    public class ImputationDTO
    {
        public int Id { get; set; }
        public int ObjectID { get; set; }
        public string AllVersionsIds { get; set; }
        public int UserID { get; set; }
        public string UserFullName { get; set; }
        public List<ImputationFieldDTO> Fields { get; set; }
        public string CreationDate { get; set; }
    }

    public class ImputationFieldDTO
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }
}
