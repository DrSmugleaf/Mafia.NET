using Mafia.NET.Players;
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
            var crimes = new Crimes(null!);

            for (var i = 0; i < 2; i++)
                foreach (var crime in Crimes.All)
                    crimes.Add(crime);

            Assert.That(crimes.AllCommitted().Count, Is.EqualTo(Crimes.All.Count));

            foreach (var crime in Crimes.All)
                Assert.That(crimes.AllCommitted(), Contains.Item(crime));
        }

        [Test]
        public void Each()
        {
            foreach (var key in Crimes.All)
            {
                var crimes = new Crimes(null!);
                crimes.Add(key);
                var committed = crimes.AllCommitted();

                Assert.That(committed.Count, Is.EqualTo(1));
                Assert.That(committed[0], Is.EqualTo(key));
            }
        }
    }
}