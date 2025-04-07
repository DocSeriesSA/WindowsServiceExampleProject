namespace Doc.ECM.APIHelper.DTO
{
    public class CommentDTO
    {
        public int CommentID { get; set; }
        public int ObjectID { get; set; }
        public string Autor { get; set; }
        public string Date { get; set; }
        public string Text { get; set; }
        public int PNum { get; set; } = 1;
        public bool CanEdit { get; set; }

        public CommentDTO()
        {
            PNum = 1;
            Text = "";
        }
    }
}
