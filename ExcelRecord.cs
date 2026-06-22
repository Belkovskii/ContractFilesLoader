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
        public string? RecordName { get; set; }
        public string? FullFileName1C { get; set; }
        public string? ApproverUser1C { get; set;}
        public string? FileType1C { get; set; }
        public string? CurrentFileModifiedBy { get;set; }
    }

}
