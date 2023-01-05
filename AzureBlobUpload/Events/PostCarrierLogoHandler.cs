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
        private readonly BlobServiceClient _blobServiceClient;

        public PostCarrierLogoHandler(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<BlobContentInfo> Handle(PostCarrierLogoCommand command, CancellationToken cancellationToken)
        {
            var container = _blobServiceClient.GetBlobContainerClient("carrierlogoblobs");
            var client = container.GetBlobClient(command.File.FileName);
            var uploaded = command.File.OpenReadStream();
            return await client.UploadAsync(uploaded, cancellationToken);
        }
    }
}
