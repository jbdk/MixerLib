namespace MixerLib
{
	public static partial class API
	{
		/// <summary>
		/// A Group which a user can belong to can control features or access controls throughout Mixer.
		/// </summary>
		public class UserGroup : TimeStamped
		{
			/// <summary>The unique ID of the group.</summary>
			public uint Id { get; set; }

			/// <summary>he name of the group.</summary>
			/* Available roles are:
				User - A regular user.All Users have this.
				Banned - A user who has been banned from the channel will have this role.
				Pro - A user who has an active Mixer Pro subscription will have this role.
				VerifiedPartner - A channel who is marked as Verified but does not have a Subscribe button will have this role.
				Partner - A channel that has a Subscribe button will have this role.
				Subscriber - A user who has an active subscription for the partnered channel involved in this request will have this role.
				ChannelEditor - A user marked as a Channel Editor will be able to change that channel's title, game, and other channel properties.
				Mod - A user will have this role if they are a moderator in the channel involved in this request.
				GlobalMod - A user will have this role if they are a global moderator on Mixer.
				Staff - A User will have this role if they are Mixer Staff.
				Founder - A User will have this role if they are a Mixer Founder.
				Owner - A user will have this role if they are the owner of the channel involved in this request.
			*/
			public string Name { get; set; }
		}
	}
}
