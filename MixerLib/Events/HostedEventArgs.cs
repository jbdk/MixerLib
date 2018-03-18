using System;

namespace MixerLib.Events
{
   public class HostedEventArgs : EventArgs
   {
      public bool IsHosting { get; set; }
      public string HosterName { get; set; }
      public uint CurrentViewers { get; set; }
      public uint ChannelId { get; set; }
   }
}
