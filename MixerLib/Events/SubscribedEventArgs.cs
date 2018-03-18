using System;

namespace MixerLib.Events
{
   public class SubscribedEventArgs : EventArgs
   {
      public string UserName { get; set; }
      public uint ChannelId { get; set; }
   }
}
