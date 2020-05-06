using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;

namespace AzureBlobUpload.Pages
{
	public class FileInfo
	{
		public string Name { get; set; }
		public string Base64FileData { get; set; }
		public string ContentType { get; set; }
		public string Uri { get; set; }
	}

    public class EditFileModel : PageModel
    {
		private readonly IOptions<StorageAccountInfo> _storageAccountInfOptions;

		[BindProperty]
		public FileInfo FileInfo { get; set; }

		[BindProperty]
		public IFormFile Upload { get; set; }

		public EditFileModel(IOptions<StorageAccountInfo> storageAccountInfOptions) 
			=> _storageAccountInfOptions = storageAccountInfOptions;

		public async Task OnGetAsync(string fileName)
        {
			var cloudStorageAccount = CloudStorageAccount.Parse(_storageAccountInfOptions.Value.ConnectionString);  
			var blobClient = cloudStorageAccount.CreateCloudBlobClient();
			var container = blobClient.GetContainerReference(_storageAccountInfOptions.Value.ContainerName);
			var file = await container.GetBlobReferenceFromServerAsync(fileName);

			if (await file.ExistsAsync())
			{
				FileInfo = new FileInfo
				{
					ContentType = file.Properties.ContentType,
					Name = file.Name,
					Uri = file.Uri.AbsoluteUri
				};
				
				using (var memoryStream = new MemoryStream())
				{
					await file.DownloadToStreamAsync(memoryStream);
					var bytes = memoryStream.ToArray();
					FileInfo.Base64FileData = Convert.ToBase64String(bytes, 0, bytes.Length);
				}
			}
		}

		public async Task OnPostAsync()
		{
			var cloudStorageAccount = CloudStorageAccount.Parse(_storageAccountInfOptions.Value.ConnectionString);
			var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
			var cloudBlobContainer = cloudBlobClient.GetContainerReference(_storageAccountInfOptions.Value.ContainerName);
			var fileName = Upload.FileName;
			var fileMimeType = Upload.ContentType;

			var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
			cloudBlockBlob.Properties.ContentType = fileMimeType;

			await cloudBlockBlob.UploadFromStreamAsync(Upload.OpenReadStream());
			var uri = cloudBlockBlob.Uri.AbsoluteUri;
			await OnGetAsync(fileName);
		}
    }
}