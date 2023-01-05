using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs.Models;
using MediatR;
using AzureBlobUpload.Events;


namespace AzureBlobUpload.Pages
{
	public class UploadModel : PageModel
	{
		private readonly IOptions<StorageAccountInfo> _storageAccountInfOptions;
		private readonly IMediator _mediator;
		private readonly ILogger _logger;

		public UploadModel(IOptions<StorageAccountInfo> storageAccountInfOptions, IMediator mediator, ILogger logger)
		{
            _storageAccountInfOptions = storageAccountInfOptions;
			_mediator = mediator;
			_logger = logger;
        }

		[BindProperty]
		public IFormFile Upload { get; set; }

		[BindProperty]
		public List<BlobItem> CarrierLogos { get; set; }

		public async void OnGetAsync()
		{
			try
			{
				var request = new GetCarrierLogosCommand(_storageAccountInfOptions.Value.ConnectionString, _storageAccountInfOptions.Value.ContainerName);
				CarrierLogos = await _mediator.Send(request);
			}
			catch (Exception ex)
			{
				_logger.LogError("There was an error retrieving carrier logos: {0}", ex.Message);
			}
        }

		public async Task OnPostAsync()
		{
			try
			{
				var request = new PostCarrierLogoCommand(Upload);
				var response = await _mediator.Send(request);
			}
            catch (Exception ex)
            {
                _logger.LogError("There was an error saving carrier logo: {0}", ex.Message);
            }
        }
	}
}