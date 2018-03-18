namespace MixerLib
{
   internal interface IMixerClientInternal
   {
      IMixerFactory Factory { get; }
      IMixerConstellation Constellation { get; }
      IMixerChat Chat { get; }
   }
}
