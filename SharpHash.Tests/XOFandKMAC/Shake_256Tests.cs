using SharpHash.Base;
using SharpHash.Interfaces;
using SharpHash.Utils;
using SharpHash.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SharpHash.XOFandKMAC.Tests
{
    [TestClass]
    public class Shake_256Tests
    {
        protected IHash hash = HashFactory.XOF.CreateShake_256(512);

        protected string ExpectedHashOfEmptyData = "46B9DD2B0BA88D13233B3FEB743EEB243FCD52EA62B81B82B50C27646ED5762FD75DC4DDD8C0F200CB05019D67B592F6FC821C49479AB48640292EACB3B7C4BE";
        protected string ExpectedHashOfDefaultData = "922279516284A34F384ADA776D3606FBEC97875E716E6EA0FFCF9372AAB696BEEFAB7C34CC5D1C926CEAD58FD4D6DB597C8620782541D7D7B47498FE4AF4B7A4";
        protected string ExpectedHashOfOnetoNine = "24347B9C4B6DA2FC9CDE08C87F33EDD2E603C8DCD6840E6B3920F62B1DD69D7BC4655A9E6F0EE6255940380DCD1488DBCA3E796AE58A2234CC31CD61DFD1EB56";
        protected string ExpectedHashOfabcde = "98AD79D7ED29F585AD1AFFBC2BB5B5F244917F97CEA8B5424FDC6F7377A22042FD410C95237B587A0A13B10062034E1E3BF6B5766291CCC1F4C44229371991ED";
        protected string ExpectedVeryLongShakeOfEmptyString =
            "46B9DD2B0BA88D13233B3FEB743EEB243FCD52EA62B81B82B50C27646ED5762FD75DC4DDD8C0F200CB05019D67B592F6FC821C49479AB48640292EACB3B7C4BE141E96616FB13957692CC7EDD0B45AE3DC07223C8E92937BEF84BC0EAB862853349EC755"
          + "46F58FB7C2775C38462C5010D846C185C15111E595522A6BCD16CF86F3D122109E3B1FDD943B6AEC468A2D621A7C06C6A957C62B54DAFC3BE87567D677231395F6147293B68CEAB7"
          + "A9E0C58D864E8EFDE4E1B9A46CBE854713672F5CAAAE314ED9083DAB4B099F8E300F01B8650F1F4B1D8FCF3F3CB53FB8E9EB2EA203BDC970F50AE55428A91F7F53AC266B28419C3778A15FD248D339EDE785FB7F5A1AAA96D313EACC890936C173CDCD0FA"
          + "B882C45755FEB3AED96D477FF96390BF9A66D1368B208E21F7C10D04A3DBD4E360633E5DB4B602601C14CEA737DB3DCF722632CC77851CBDDE2AAF0A33A07B373445DF490CC8FC1E4160FF118378F11F0477DE055A81A9EDA57A4A2CFB0C83929D31091"
          + "2F729EC6CFA36C6AC6A75837143045D791CC85EFF5B21932F23861BCF23A52B5DA67EAF7BAAE0F5FB1369DB78F3AC45F8C4AC5671D85735CDDDB09D2B1E34A1FC066FF4A162CB263D6541274AE2FCC865F618ABE27C124CD8B074CCD516301B91875824D09958F341"
          + "EF274BDAB0BAE316339894304E35877B0C28A9B1FD166C796B9CC258A064A8F57E27F2A5B8D548A728C9444ECB879ADC19DE0C1B8587DE3E73E15D3CE2DB7C9FA7B58FFC0E87251773FAF3E8F3"
          + "E3CF1D4DFA723AFD4DA9097CB3C866ACBE" +
          "FAB2C4E85E1918990FF93E0656B5F75B08729C60E6A9D7352B9EFD2E33E3D1BA6E6D89EDFA671266ECE6BE7BB5AC948B737E41590ABE138CE1869C08680162F08863D174E77E07A9DDB33B57DE04C443A5BD77C42036871AAE7893362B"
          + "27015B84B4139F0E313579B4EF5F6B6426563D7195B8C5B84736B14266160342C4093F8ABEA48371BA94CC06DCB6B8A8E7BCE6354F9BABC949A5F18F8C9F0AAEFE0B8BECAD386F078CA41CACF2E3D17F4"
          + "EC21FED0E3B682435AD5B665C25D7B61B379E86824C2B22D5A54835F8B04D4C0B29667BAEB0C3258809EE698DBC03536A1C936C811F6E6F69210F5632080064923FDF9CF405301E45A3F96E3F57C55C4E0B538EFE8942F6B601AC49EA635F70"
          + "E4BA39E5FCE513CFB672945BB92E17F7D222EAB2AA29BE89FC3FF24BC6B6D7A3D307CE7B1731E7DF59690D0530D7F2F5BB9ED37D180169A6C1BB022252AB8CC6860E3CF1F1414C90A19350B526E3741E500717769CDD09D268CC3F88B5D521C70AA8BBE631FBF08905A0A833D2005830717"
          + "ADBA3233DD591BC505C7B13A9D5672AD4BE10C744AC33D9E92A23BDEE6E14D470EE7DC142FE4EFF4182A49BEEEC8E4";

        [TestMethod]
        public void TestEmptyString()
        {
            TestHelper.TestActualAndExpectedData(TestConstants.EmptyData,
                ExpectedHashOfEmptyData, hash);
        }

        [TestMethod]
        public void TestDefaultData()
        {
            TestHelper.TestActualAndExpectedData(TestConstants.DefaultData,
                ExpectedHashOfDefaultData, hash);
        }

        [TestMethod]
        public void TestOnetoNine()
        {
            TestHelper.TestActualAndExpectedData(TestConstants.OnetoNine,
                ExpectedHashOfOnetoNine, hash);
        }

        [TestMethod]
        public void TestBytesabcde()
        {
            TestHelper.TestActualAndExpectedData(TestConstants.Bytesabcde,
                ExpectedHashOfabcde, hash);
        }

        [TestMethod]
        public void TestEmptyStream()
        {
            TestHelper.TestEmptyStream(ExpectedHashOfEmptyData, hash);
        }

        [TestMethod]
        public void TestIncrementalHash()
        {
            TestHelper.TestIncrementalHash(TestConstants.DefaultData,
                ExpectedHashOfDefaultData, hash);
        }

        [TestMethod]
        public void TestHashCloneIsCorrect()
        {
            TestHelper.TestHashCloneIsCorrect(hash);
        }

        [TestMethod]
        public void TestHashCloneIsUnique()
        {
            TestHelper.TestHashCloneIsUnique(hash);
        }

        [TestMethod]
        public void TestHMACCloneIsCorrect()
        {
            TestHelper.TestHMACCloneIsCorrect(hash);
        }

        [TestMethod]
        public void TestVeryLongShakeOfEmptyString()
        {
            IHash VeryLongShake_256 = HashFactory.XOF.CreateShake_256(8000);

            string ActualString = VeryLongShake_256.ComputeString(
                TestConstants.EmptyData).ToString();

            string ExpectedString = ExpectedVeryLongShakeOfEmptyString;

            Assert.AreEqual(ExpectedString, ActualString,
                String.Format("Expected {0} but got {1}.",
                ExpectedString, ActualString));
        }

        [TestMethod]
        public void TestVeryLongShakeOfEmptyStringWithStreamingOutput()
        {
            IXOF VeryLongShake_256;
            byte[] TempResult, ExpectedChunk, ActualChunk;

            byte[] Expected = Converters.ConvertHexStringToBytes(ExpectedVeryLongShakeOfEmptyString);

            TempResult = new byte[1000];
            VeryLongShake_256 = HashFactory.XOF.CreateShake_256(8000) as IXOF;
            VeryLongShake_256.Initialize();
            VeryLongShake_256.TransformString(TestConstants.EmptyData);

            VeryLongShake_256.DoOutput(ref TempResult, 0, 250);

            ActualChunk = new byte[250];
            Utils.Utils.memcopy(ref ActualChunk, TempResult, 250, 0);

            ExpectedChunk = new byte[250];
            Utils.Utils.memcopy(ref ExpectedChunk, Expected, 250, 0);

            Assert.IsTrue(TestHelper.Compare(ExpectedChunk, ActualChunk), 
                "Shake256 Streaming Test 1 Mismatch");

            VeryLongShake_256.DoOutput(ref TempResult, 250, 250);
            
            Utils.Utils.memcopy(ref ActualChunk, TempResult, 250, 250);
            Utils.Utils.memcopy(ref ExpectedChunk, Expected, 250, 250);

            Assert.IsTrue(TestHelper.Compare(ExpectedChunk, ActualChunk),
                 "Shake256 Streaming Test 2 Mismatch");

            VeryLongShake_256.DoOutput(ref TempResult, 500, 250);

            Utils.Utils.memcopy(ref ActualChunk, TempResult, 250, 500);
            Utils.Utils.memcopy(ref ExpectedChunk, Expected, 250, 500);

            Assert.IsTrue(TestHelper.Compare(ExpectedChunk, ActualChunk),
                 "Shake256 Streaming Test 3 Mismatch");

            VeryLongShake_256.DoOutput(ref TempResult, 750, 250);

            Utils.Utils.memcopy(ref ActualChunk, TempResult, 250, 750);
            Utils.Utils.memcopy(ref ExpectedChunk, Expected, 250, 750);

            Assert.IsTrue(TestHelper.Compare(ExpectedChunk, ActualChunk),
                 "Shake256 Streaming Test 4 Mismatch");

            string ActualString = Converters.ConvertBytesToHexString(TempResult, false);
            string ExpectedString = ExpectedVeryLongShakeOfEmptyString;

            Assert.AreEqual(ExpectedString, ActualString, 
                String.Format("Expected {0} but got {1}.", 
                ExpectedString, ActualString));

            // Verify that Initialization Works
            VeryLongShake_256.Initialize();

            VeryLongShake_256.DoOutput(ref TempResult, 0, 250);

            Utils.Utils.memcopy(ref ActualChunk, TempResult, 250, 0);
            Utils.Utils.memcopy(ref ExpectedChunk, Expected, 250, 0);
            
            Assert.IsTrue(TestHelper.Compare(ExpectedChunk, ActualChunk),
                "Shake256 Streaming Initialization Test Fail");

        }

    }
}