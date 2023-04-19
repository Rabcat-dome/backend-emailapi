using NUnit.Framework;
using PTTDigital.Email.Data.Models;

namespace PTTDigital.Email.NUnitTest
{
    [TestFixture]
    public class EmailQueueTests
    {
        [SetUp]
        public void Setup()
        {
            //var mock = new ModelCacheKey();
            var queues = new List<EmailQueue>();
        }

        //ทดสอบการ InsertQueueService โดยการส่ง Parameter ลงไป จะต้องได้ Key ของ ulid ออกมา
        [Test]
        [TestCase()]
        public void MethodName_Scenario_Result()
        {
            //Arrange
            //var queues = new List<EmailQueue>()
            //{
            //    new EmailQueue()
            //    {
            //        Message = new Message()
            //        {
            //            EmailSubject = 
            //        }
            //    }
            //};

            //Act


            //Assert
            Assert.Pass();
            //Assert.That(true,Is.True);
        }

        //ทดสอบ report ของ EmailQueue Service โดยตรวจสอบว่าต้องมี Data ทุก Column
        [Test]
        public void MethodName_Scenario_Result2()
        {
            //Arrange


            //Act


            //Assert
            Assert.Pass();
            //Assert.That(true,Is.True);
        }
    }
}