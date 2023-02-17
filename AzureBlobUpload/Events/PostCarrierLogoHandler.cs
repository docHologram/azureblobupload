using MediatR;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AzureBlobUpload.Events
{
    public record PostCarrierLogoCommand(IFormFile File) : IRequest<BlobContentInfo>;

    public class PostCarrierLogoHandler : IRequestHandler<PostCarrierLogoCommand, BlobContentInfo>
    {
        private readonly BlobContainerClient _blobContainerClient;

        public PostCarrierLogoHandler(BlobContainerClient blobContainerClient)
        {
            _blobContainerClient = blobContainerClient;
        }

        public async Task<BlobContentInfo> Handle(PostCarrierLogoCommand command, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(command.File.FileName);
            return await blobClient.UploadAsync(command.File.FileName);
        }
    }
}
