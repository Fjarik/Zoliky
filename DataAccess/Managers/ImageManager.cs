using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Errors;
using DataAccess.Models;
using JetBrains.Annotations;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class ImageManager : IManager<Image>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		public ImageManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}


		public MActionResult<Image> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<Image>(StatusCode.NotValidID);
			}
			Image i = _ent.Images.Find(id);
			if (i == null) {
				return new MActionResult<Image>(StatusCode.NotFound);
			}
			return new MActionResult<Image>(StatusCode.OK, i);
		}

		[NotNull]
		public MActionResult<Image> Select(string hash)
		{
			if (string.IsNullOrWhiteSpace(hash)) {
				return new MActionResult<Image>(StatusCode.NotValidID);
			}
			Image i = _ent.Images.FirstOrDefault(x => x.Hash == hash);
			if (i == null) {
				return new MActionResult<Image>(StatusCode.NotFound);
			}
			return new MActionResult<Image>(StatusCode.OK, i);
		}

		[NotNull]
		public MActionResult<Image> Create(string name, string hash, string base64, string mime, int size, int ownerId)
		{
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(hash) || string.IsNullOrWhiteSpace(base64) || string.IsNullOrWhiteSpace(mime) || size < 1) {
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
			Image i1 = _ent.Images.Add(i);
			Save(null);
			return new MActionResult<Image>(StatusCode.OK, i1);
		}

		public int Save(Image content, bool throwException = true)
		{
			try {
				if (content != null) {
					_ent.Images.AddOrUpdate(content);
				}
				int changes = _ent.SaveChanges();
				return changes;
			} catch (DbEntityValidationException ex) {
				if (throwException) {
					throw new DbEntityValidationException(ex.GetExceptionMessage(), ex.EntityValidationErrors);
				}
				return 0;
			}
		}

	}
}
