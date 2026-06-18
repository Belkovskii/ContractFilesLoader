namespace ContractLoader
{
    public struct ExcelRecord
    {
        public string? FileGuid { get; set; }
        public string? DocumentGuid { get; set; }
        public string? PathToFile { get; set; }
        public string? FileModifiedDate { get; set; }
        public string? FileModifiedUser { get; set; }
        public string? LoadResult { get; set; }
        public string? Author { get; set; }
        public string? CreationDate { get; set; }
    }

}
