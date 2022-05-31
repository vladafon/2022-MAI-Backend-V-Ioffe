using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProgramEngineering.Logic
{
    public class S3Repository : IDisposable
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _client;

        public S3Repository(IConfiguration configuration)
        {
            var accessKey = configuration["S3AccessKey"];
            var secretKey = configuration["S3SecretKey"];

            AmazonS3Config configsS3 = new AmazonS3Config
            {
                ServiceURL = "https://s3.yandexcloud.net"
            };

            _client = new AmazonS3Client(accessKey,secretKey,configsS3);

            _bucketName = configuration["S3BucketName"];
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<List<string>> GetFiles()
        {
            ListObjectsRequest listObjectsRequest = new ListObjectsRequest();
            listObjectsRequest.BucketName = _bucketName;
            var fileNames = new List<string>();
            ListObjectsResponse listObjectsResponse = await _client.ListObjectsAsync(listObjectsRequest);
            do
            {

                List<S3Object> s3Objects = listObjectsResponse.S3Objects;

                foreach (S3Object s3Object in s3Objects)
                {
                    fileNames.Add(s3Object.Key);
                }
                listObjectsRequest.Marker = listObjectsResponse.NextMarker;
                listObjectsResponse = await _client.ListObjectsAsync(listObjectsRequest);
            }
            while (listObjectsResponse.IsTruncated);

            return fileNames;
        }

        public async Task<byte[]> GetFile(string key)
        {
            MemoryStream ms = null;

            GetObjectRequest getObjectRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            using (var response = await _client.GetObjectAsync(getObjectRequest))
            {
                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    using (ms = new MemoryStream())
                    {
                        await response.ResponseStream.CopyToAsync(ms);
                    }
                }
            }

            if (ms is null || ms.ToArray().Length < 1)
                throw new FileNotFoundException(string.Format("The document '{0}' is not found", key));

            return ms.ToArray();
        }



        public async Task PutFile(Stream fileStream, string fileName)
        {
            var request = new PutObjectRequest()
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = fileStream
            };


            await _client.PutObjectAsync(request);
        }
    }
}
