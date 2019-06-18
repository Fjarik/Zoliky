using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared;

namespace DataAccess.Managers.New
{
	public class ImageManager : Manager<Image>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public ImageManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public ImageManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Overrides
		///

#region Overrides

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static ImageManager Create(IdentityFactoryOptions<ImageManager> options, IOwinContext context)
		{
			return new ImageManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<Image>> SelectAsync(string hash)
		{
			if (string.IsNullOrWhiteSpace(hash)) {
				return new MActionResult<Image>(StatusCode.NotValidID);
			}
			Image i = await _ctx.Images.AsNoTracking().FirstOrDefaultAsync(x => x.Hash == hash);
			if (i == null) {
				return new MActionResult<Image>(StatusCode.NotFound);
			}
			return new MActionResult<Image>(StatusCode.OK, i);
		}

		public async Task<MActionResult<Image>> CreateAsync(int ownerId, byte[] bytes, 
															string mime, string name = "Profile photo")
		{
			if (bytes.Length < 1) {
				return new MActionResult<Image>(StatusCode.InvalidInput);
			}
			string hash;
			using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider()) {
				hash = Convert.ToBase64String(sha1.ComputeHash(bytes));
			}
			var base64 = Convert.ToBase64String(bytes);
			return await CreateAsync(ownerId, hash, base64, mime, bytes.Length, name);
		}

		private async Task<MActionResult<Image>> CreateAsync(int ownerId, string hash, 
															string base64, string mime, 
															int size, string name = "Profile photo")
		{
			if (ownerId < 1) {
				return  new MActionResult<Image>(StatusCode.NotValidID);
			}

			if (Methods.AreNullOrWhiteSpace(name, hash, base64, mime) || size < 1) {
				return new MActionResult<Image>(StatusCode.InvalidInput);
			}

			Image i = new Image() {
				Name = name,
				Hash = hash,
				OwnerID = ownerId,
				Uploaded = DateTime.Now,
				Base64 = base64,
				MIME = mime,
				Size = size,
			};
			return await this.CreateAsync(i);
		}

		public async Task<int> GetSizeAsync(int imageId)
		{
			if (imageId < 1) {
				return 0;
			}
			var size = (await _ctx.Images.FindAsync(imageId))?.Size;
			if (size == null) {
				return 0;
			}
			return (int) size;
		}

#endregion

#endregion
	}
}