using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs.Models;
using MediatR;
using AzureBlobUpload.Events;


namespace AzureBlobUpload.Pages
{
	public class UploadModel : PageModel
	{
		private readonly IMediator _mediator;
		private readonly ILogger _logger;

		public UploadModel(IMediator mediator, ILogger logger)
		{
			_mediator = mediator;
			_logger = logger;
        }

		[BindProperty]
		public IFormFile Upload { get; set; }

		[BindProperty]
		public List<BlobItem> CarrierLogos { get; set; }

		public async Task OnGetAsync()
		{
			try
			{
				CarrierLogos = await _mediator.Send(new ListCarrierLogosCommand());
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
				await _mediator.Send(new PostCarrierLogoCommand(Upload));
				await OnGetAsync();
			}
            catch (Exception ex)
            {
                _logger.LogError("There was an error saving carrier logo: {0}", ex.Message);
            }
        }
	}
}