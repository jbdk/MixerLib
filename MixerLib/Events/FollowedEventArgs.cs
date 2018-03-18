using System;

namespace MixerLib.Events
{
   public class FollowedEventArgs : EventArgs
   {
      public bool IsFollowing { get; set; }
      public string UserName { get; set; }
      public uint ChannelId { get; set; }
   }
}
