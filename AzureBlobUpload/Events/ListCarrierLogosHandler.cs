using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureBlobUpload.Events
{
    public record ListCarrierLogosCommand() : IRequest<List<BlobItem>>;

    public class ListCarrierLogosHandler : IRequestHandler<ListCarrierLogosCommand, List<BlobItem>>
    {
        private readonly BlobContainerClient _blobContainerClient;

        public ListCarrierLogosHandler(BlobContainerClient blobContainerClient) 
        {
            _blobContainerClient = blobContainerClient;
        }

        public async Task<List<BlobItem>> Handle(ListCarrierLogosCommand command, CancellationToken cancellationToken)
        {
            var items = new List<BlobItem>();
            await foreach (BlobItem item in _blobContainerClient.GetBlobsAsync())
            {
                items.Add(item);
            }
            return items;
        }
    }
}
