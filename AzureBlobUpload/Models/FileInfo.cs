namespace AzureBlobUpload.Models
{
    public class FileInfo
    {
        public string Name { get; set; }
        public string Base64FileData { get; set; }
        public string ContentType { get; set; }
        public string Uri { get; set; }
    }
}
