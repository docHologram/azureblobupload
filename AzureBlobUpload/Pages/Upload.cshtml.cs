using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;

namespace AzureBlobUpload.Pages
{
	public class UploadModel : PageModel
	{
		private readonly IOptions<StorageAccountInfo> _storageAccountInfOptions;

		public UploadModel(IOptions<StorageAccountInfo> storageAccountInfOptions)
			=> _storageAccountInfOptions = storageAccountInfOptions;

		[BindProperty]
		public IFormFile Upload { get; set; }

		[BindProperty]
		public List<CloudBlob> CarrierLogos { get; set; }

		public void OnGet()
		{
			var cloudStorageAccount = CloudStorageAccount.Parse(_storageAccountInfOptions.Value.ConnectionString);
			var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
			var cloudBlobContainer = cloudBlobClient.GetContainerReference(_storageAccountInfOptions.Value.ContainerName);

			CarrierLogos = cloudBlobContainer.ListBlobs()
				.Select(_ => (CloudBlob) _)
				.ToList();
		}

		public async Task OnPostAsync()
		{
			var cloudStorageAccount = CloudStorageAccount.Parse(_storageAccountInfOptions.Value.ConnectionString);
			var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
			var cloudBlobContainer = cloudBlobClient.GetContainerReference(_storageAccountInfOptions.Value.ContainerName);
			var fileName = Upload.FileName;
			var fileMimeType = Upload.ContentType;

			if (await cloudBlobContainer.CreateIfNotExistsAsync())
				await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

			var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
			cloudBlockBlob.Properties.ContentType = fileMimeType;

			await cloudBlockBlob.UploadFromStreamAsync(Upload.OpenReadStream());
			var uri = cloudBlockBlob.Uri.AbsoluteUri;

			OnGet();
		}
	}
}