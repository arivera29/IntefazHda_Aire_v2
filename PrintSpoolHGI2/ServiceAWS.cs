using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSpoolHGI2
{
    public class ServiceAWS
    {
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static string awsAccessKey = "AKIAX2LZWGZ3OGR56VVU";
        private static string awsSecretKey = "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";
        private static string awsBucketName = "hgi2cmbucket";
        public static void UploadFileAWS(string filePath, string keyName)
        {
            try
            {
                TransferUtility transferUtility = new TransferUtility((IAmazonS3)new AmazonS3Client(awsAccessKey, awsSecretKey, ServiceAWS.bucketRegion));
                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
                FileStream fileStream = System.IO.File.OpenRead(filePath);
                request.BucketName = awsBucketName;
                request.Key = keyName;
                request.Headers["Content-Type"] = "application/pdf";
                request.InputStream = (Stream)fileStream;
                transferUtility.Upload(request);
                if (!System.IO.File.Exists(filePath))
                    return;
                System.IO.File.Delete(filePath);
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", (object)ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", (object)ex.Message);
            }
        }

        public static void UploadFileAWSStream(byte[] filePath, string keyName)
        {
            try
            {
                
                AmazonS3Client amazonS3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, ServiceAWS.bucketRegion);
                MemoryStream memoryStream = new MemoryStream(filePath);
                new TransferUtility((IAmazonS3)amazonS3Client).Upload(new TransferUtilityUploadRequest()
                {
                    BucketName = awsBucketName,
                    Key = keyName,
                    Headers = {
            ["Content-Type"] = "application/pdf"
          },
                    InputStream = (Stream)memoryStream
                });
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", (object)ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", (object)ex.Message);
            }
        }

        public static byte[] ReadObjectDataHgi(string fileName)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                using (AmazonS3Client amazonS3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, ServiceAWS.bucketRegion))
                {
                    GetObjectRequest request = new GetObjectRequest()
                    {
                        BucketName = awsBucketName,
                        Key = fileName
                    };
                    using (GetObjectResponse getObjectResponse = amazonS3Client.GetObject(request))
                    {
                        using (Stream responseStream = getObjectResponse.ResponseStream)
                        {
                            responseStream.CopyTo((Stream)memoryStream);
                            memoryStream.Position = 0L;
                        }
                    }
                }
                byte[] numArray = new byte[0];
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeleteFile(string KeyName)
        {
            IAmazonS3 amazonS3 = (IAmazonS3)new AmazonS3Client(awsAccessKey, awsSecretKey, ServiceAWS.bucketRegion);
            DeleteObjectRequest request = new DeleteObjectRequest()
            {
                BucketName = awsBucketName,
                Key = KeyName
            };
            Console.WriteLine("Deleting an object");
            amazonS3.DeleteObject(request);
        }

    }
}
