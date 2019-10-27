using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHash.Base;

namespace SharpHash.Crypto.Tests
{
    [TestClass]
    public class RadioGatun32Tests : CryptoHashBaseTests
    {
        static RadioGatun32Tests()
        {
            hash = HashFactory.Crypto.CreateRadioGatun32();

            ExpectedHashOfEmptyData = "F30028B54AFAB6B3E55355D277711109A19BEDA7091067E9A492FB5ED9F20117";
            ExpectedHashOfDefaultData = "17B20CF19B3FC84FD2FFE084F07D4CD4DBBC50E41048D8259EB963B0A7B9C784";
            ExpectedHashOfOnetoNine = "D77629174F56D8451F73CBE80EC7A20EF2DD65C46A1480CD004CBAA96F3FA1FD";
            ExpectedHashOfabcde = "A593059B12513A1BD88A2D433F07B239BC14743AF0FF7294837B5DF756BF9C7A";
            ExpectedHashOfDefaultDataWithHMACWithLongKey = "CD48D590665EA2C066A0C26E2620D567C75090DE38045B88C53BFAE685D67886";
            ExpectedHashOfDefaultDataWithHMACWithShortKey = "72EB7D36180C1B1BBF88E062FEC7419DBB4849892623D332821C1B0D71D6D513";
        }
    }
}