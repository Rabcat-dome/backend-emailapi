namespace PTTDigital.Email.NUnitTest
{
    [TestFixture]
    public class EmailQueueTests
    {
        [SetUp]
        public void Setup()
        {
        }

        //ทดสอบการ InsertQueueService โดยการส่ง Parameter ลงไป จะต้องได้ Key ของ ulid ออกมา
        [Test]
        public void MethodName_Scenario_Result()
        {
            //Arrange


            //Act


            //Assert
            Assert.Pass();
            //Assert.That(true,Is.True);
        }

        //ทดสอบ report ของ EmailQueue Service โดยตรวจสอบว่าต้องมี Data ทุก Column
    }
}