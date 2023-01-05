using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MediatR;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureBlobUpload.Events
{
    public record GetCarrierLogosCommand(string ConnectionString, string ContainerName) : IRequest<List<BlobItem>>;

    public class GetCarrierLogosHandler : IRequestHandler<GetCarrierLogosCommand, List<BlobItem>>
    {
        private readonly BlobServiceClient _blobServiceClient;

        public GetCarrierLogosHandler(BlobServiceClient blobServiceClient) 
        { 
            _blobServiceClient= blobServiceClient;
        }

        public async Task<List<BlobItem>> Handle(GetCarrierLogosCommand command, CancellationToken cancellationToken)
        {
            var container = _blobServiceClient.GetBlobContainerClient("carrierlogoblobs");

            var items = new List<BlobItem>();
            await foreach (BlobItem item in container.GetBlobsAsync())
            {
                items.Add(item);
            }
            return items;
        }
    }
}
