using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using AzureBlobUpload.Models;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace AzureBlobUpload.Events
{
    public record GetCarrierLogoCommand(string FileName) : IRequest<FileInfo>;

    public class GetCarrierLogoHandler : IRequestHandler<GetCarrierLogoCommand, FileInfo>
    {
        private readonly BlobContainerClient _blobContainerClient;

        public GetCarrierLogoHandler(BlobContainerClient blobContainerClient)
        {
            _blobContainerClient = blobContainerClient;
        }

        public async Task<FileInfo> Handle(GetCarrierLogoCommand command, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(command.FileName);

            var fileInfo = new FileInfo
            {
                Name = command.FileName,
                Uri = blobClient.Uri.AbsoluteUri
            };

            using var memoryStream = new System.IO.MemoryStream();
            using var result = await blobClient.DownloadToAsync(memoryStream, cancellationToken);

            if (!result.IsError)
            {
                fileInfo.ContentType = result.Headers.ContentType;
                fileInfo.Base64FileData = Convert.ToBase64String(memoryStream.ToArray());
            }

            return fileInfo;
        }
    }
}
