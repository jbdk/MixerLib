using System;

namespace MixerLib.Events
{
   public class ResubscribedEventArgs : EventArgs
   {
      public string UserName { get; set; }
      public uint TotalMonths { get; set; }
      public DateTime Since { get; set; }
      public uint ChannelId { get; set; }
   }
}
