using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using AzureBlobUpload.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace AzureBlobUpload.Pages
{

	public class EditFileModel : PageModel
	{
		private readonly BlobContainerClient _blobContainerClient;

		public EditFileModel(BlobContainerClient blobContainerClient)
			=> _blobContainerClient = blobContainerClient;

		[BindProperty]
		public FileInfo FileInfo { get; set; }

		[BindProperty]
		public IFormFile Upload { get; set; }

		public async Task OnGetAsync(string fileName)
		{
			var blobClient = _blobContainerClient.GetBlobClient(fileName);

            FileInfo = new FileInfo
            {
                Name = fileName,
                Uri = blobClient.Uri.AbsoluteUri
            };

            using var memoryStream = new System.IO.MemoryStream();
            using var result = await blobClient.DownloadToAsync(memoryStream);

            if (!result.IsError)
			{
				FileInfo.ContentType = result.Headers.ContentType;
				FileInfo.Base64FileData = Convert.ToBase64String(memoryStream.ToArray());
			}
            
        }

		public async Task OnPostAsync()
		{
			var fileName = Upload.FileName;

            var blobClient = _blobContainerClient.GetBlobClient(fileName);
			using var result = blobClient.UploadAsync(fileName);

			await OnGetAsync(fileName);
		}
	}
}