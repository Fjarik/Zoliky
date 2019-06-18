using DataAccess.Managers;
using DataAccess.Models;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
	public sealed class Manager
	{
		public ZoliksEntities Db { get; private set; }

		public ClassManager Classes { get; private set; }
		public ConsentManager Consents { get; private set; }
		public CrashManager Crashes { get; private set; }
		public HashMananger Hashes { get; private set; }
		public ChangelogManager Changelogs { get; private set; }
		public ImageManager Images { get; private set; }
		public NewsManager News { get; private set; }
		public NotificationManager Notifications { get; private set; }
		public PriceManager Prices { get; private set; }
		public ProjectManager Projects { get; private set; }
		public RankManager Ranks { get; private set; }
		public TokenManager Tokens { get; private set; }
		public TransManager Transactions { get; private set; }
		public UnavailabilitiesManager Unavailabilities { get; private set; }
		public UserManager Users { get; private set; }
		public LoginsManager Logins { get; private set; }
		public WebEventManager Events { get; private set; }
		public ZolikManager Zoliky { get; private set; }


		public Manager()
		{
			Db = new ZoliksEntities();

			Classes = new ClassManager(Db, this);
			Consents = new ConsentManager(Db, this);
			Crashes = new CrashManager(Db, this);
			Hashes = new HashMananger(Db, this);
			Changelogs = new ChangelogManager(Db, this);
			Images = new ImageManager(Db, this);
			News = new NewsManager(Db, this);
			Notifications = new NotificationManager(Db, this);
			Prices = new PriceManager(Db, this);
			Projects = new ProjectManager(Db, this);
			Ranks = new RankManager(Db, this);
			Tokens = new TokenManager(Db, this);
			Transactions = new TransManager(Db, this);
			Unavailabilities = new UnavailabilitiesManager(Db, this);
			Users = new UserManager(Db, this);
			Logins = new LoginsManager(Db, this);
			Events = new WebEventManager(Db, this);
			Zoliky = new ZolikManager(Db, this);
		}

		public void RefreshEntites()
		{
			Db = new ZoliksEntities();
		}

	}

}
