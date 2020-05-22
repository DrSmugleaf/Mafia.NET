﻿using Mafia.NET.Localization;
using Mafia.NET.Players.Roles;
using NUnit.Framework;

namespace Mafia.NET.Tests.Players.Roles
{
    [TestFixture]
    [TestOf(typeof(Crimes))]
    public class CrimesTest
    {
        [Test]
        public void All()
        {
            var crimes = new Crimes(null);
            
            for (var i = 0; i < 2; i++)
            {
                foreach (var crime in Crimes.All)
                    crimes.AddKey(crime);
            }

            Assert.That(crimes.AllCommitted().Count, Is.EqualTo(Crimes.All.Count));

            foreach (var crime in Crimes.All)
                Assert.That(crimes.AllCommitted(), Contains.Item(crime));
        }

        [Test]
        public void None()
        {
            var crimes = new Crimes(null);
            Assert.That(crimes.AllCommitted(), Is.Empty);
            Assert.That(crimes.Crime(), Is.EqualTo(Crimes.NoCrime));
        }

        [Test]
        public void Each([Values] CrimeKey key)
        {
            var crimes = new Crimes(null);
            crimes.Add(key);
            var committed = crimes.AllCommitted();
            
            Assert.That(committed.Count, Is.EqualTo(1));
            Assert.That(committed[0], Is.EqualTo((Key) key));
        }
    }
}