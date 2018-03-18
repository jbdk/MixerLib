using System;
using FluentAssertions;
using MixerLib.Helpers;
using Xunit;

namespace Test
{
   public class General : Base
   {
      public class EventTest
      {
         public event EventHandler Test;
      }

      [Fact]
      public void CanRaiseEventsUsingReflection()
      {
         int calls = 0;
         var sut = new EventTest();

         sut.Test += (e, s) => calls++;
         sut.Test += (e, s) => calls++;
         sut.Test += (e, s) => calls++;
         sut.Test += (e, s) => calls++;

         ReflectionHelper.RaiseEvent(sut, nameof(EventTest.Test), EventArgs.Empty);

         calls.Should().Be(4);
      }
   }
}
