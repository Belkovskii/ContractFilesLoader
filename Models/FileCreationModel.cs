namespace ContractLoader.Models
{
    public class FileCreationModel
    {
        public string? __id { get; set; }
        public DateTime __createdAt { get; set; }
        public DateTime __updatedAt { get; set; }
        public string? __name { get; set; }
        public string? originalName { get; set; }
        public int size { get; set; }
        public string? hash { get; set; }
        public int version { get; set; }
        public List<string>? __currentUserPermissions { get; set; }
        public List<string>? __subscribers { get; set; }
    }

    public class FileAttachment(FileCreationModel fileCreationModel)
    {
        public string? id { get; set; } = fileCreationModel.__id;
        public string? name { get; set; } = fileCreationModel.originalName;
        public string? hash { get; set; } = fileCreationModel.__id;
        public int size { get; set; } = fileCreationModel.size;
        public FileCreationModel file { get; set; } = fileCreationModel;
    }
}
