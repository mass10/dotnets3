using System;

namespace dotnets3
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				// コマンドライン引数の解析
				var profileName = "";
				var bucketName = "";
				var key = "";

				var currentSection = "";
				foreach (var arg in args)
				{
					// System.Console.WriteLine("[DEBUG] arg: " + arg);
					if (arg.StartsWith("--"))
					{
						currentSection = arg.Substring(2);
						//System.Console.WriteLine("[DEBUG] currentSection: " + currentSection);
					}
					else
					{
						switch (currentSection)
						{
							case "profile":
								// プロファイル名
								System.Console.WriteLine("[DEBUG] profile: " + arg);
								profileName = arg;
								break;
							case "bucket":
								// バケット名
								System.Console.WriteLine("[DEBUG] bucket: " + arg);
								bucketName = arg;
								break;
							case "key":
								// キー
								System.Console.WriteLine("[DEBUG] key: " + arg);
								key = arg;
								break;
							default:
								System.Console.WriteLine("[ERROR] 不明なオプション: " + arg);
								break;
						}
					}
				}

				new Application(profileName).Run();
			}
			catch (Exception e)
			{
				System.Console.WriteLine("[ERROR] 実行時エラー！" + e);
			}
		}
	}

	class Application
	{
		private readonly string profileName;

		public Application(string profileName)
		{
			this.profileName = profileName;
		}

		public void Run()
		{
			var service = new S3Service(this.profileName);

			// list buckets
			var buckets = service.ListBuckets(profileName);
			foreach (var bucket in buckets)
			{
				System.Console.WriteLine("[DEBUG] bucket: " + bucket.BucketName + ", " + bucket.CreationDate);
			}
		}
	}

	class S3Service
	{
		private readonly string profileName;

		private readonly Amazon.S3.AmazonS3Client client;

		public S3Service(string profileName)
		{
			this.profileName = profileName;

			Amazon.AWSConfigs.AWSProfileName = profileName;

			this.client = new Amazon.S3.AmazonS3Client(Amazon.RegionEndpoint.APNortheast1);
		}

		public System.Collections.Generic.IList<Amazon.S3.Model.S3Bucket> ListBuckets(string profile)
		{
			var request = new Amazon.S3.Model.ListBucketsRequest { };
			var response = this.client.ListBucketsAsync(request).Result;
			return response.Buckets;
		}

		public object GetObject(string bucketName, string key)
		{
			var request = new Amazon.S3.Model.GetObjectRequest { BucketName = bucketName, Key = key };
			using (var response = client.GetObjectAsync(request).Result)
			{
				System.Console.WriteLine("[DEBUG] StatusCode: [" + response.HttpStatusCode + "]");
				if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
					return null;
			}
			return null;
		}
	}
}
