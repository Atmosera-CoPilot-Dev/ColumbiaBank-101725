namespace TransformerLib.Tests;

public class ByteFactoryTests
{
  [Test]
  public void Echo_ReturnsSameValue()
  {
    var factory = new ByteFactory();
    Assert.That(factory.Echo(42), Is.EqualTo(42));
  }
}
