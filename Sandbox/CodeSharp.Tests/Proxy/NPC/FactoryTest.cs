using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CodeSharp.Proxy.NPC
{
    public class FactoryTest
    {
        [Test]
        public void SetBaseTypeChokesOnValueType()
        {
            var e = Assert.Throws<ArgumentException>(() => Factory.SetBaseType(typeof(ValueType), ""));
            Assert.That(e.ParamName, Is.EqualTo("baseType"));
        }

        [Test]
        public void SetBaseTypeChokesOnTypeDoesNotImplementINotifyPropertyChanged()
        {
            var e = Assert.Throws<ArgumentException>(() => Factory.SetBaseType(typeof(NotNotifyPropertyChanged), ""));
            Assert.That(e.ParamName, Is.EqualTo("baseType"));
        }

        [Test]
        public void SetBaseTypeChokesOnNullBaseType()
        {
            var e = Assert.Throws<ArgumentNullException>(() => Factory.SetBaseType(null, ""));
            Assert.That(e.ParamName, Is.EqualTo("baseType"));
        }

        public class NotNotifyPropertyChanged
        {
            public void OnPropertyChanged(string name) { }
        }

    }
}
