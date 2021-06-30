using IDZ1_library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///Это класс теста для TIFFTest, в котором должны
    ///находиться все модульные тесты TIFFTest
    ///</summary>
    [TestClass()]
    public class TIFFTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Получает или устанавливает контекст теста, в котором предоставляются
        ///сведения о текущем тестовом запуске и обеспечивается его функциональность.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Дополнительные атрибуты теста
        // 
        //При написании тестов можно использовать следующие дополнительные атрибуты:
        //
        //ClassInitialize используется для выполнения кода до запуска первого теста в классе
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //ClassCleanup используется для выполнения кода после завершения работы всех тестов в классе
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //TestInitialize используется для выполнения кода перед запуском каждого теста
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //TestCleanup используется для выполнения кода после завершения каждого теста
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Тест для LoadFile
        ///</summary>
        [TestMethod()]
        public void LoadFileTest()
        {
            string ptf = "C:\\Users\\s36241\\Desktop\\тест\\0042_0102_10306_1_00330_08_GSA_d.tif"; // TODO: инициализация подходящего значения
            TIFF target = new TIFF(ptf); // TODO: инициализация подходящего значения
            target.ShowData();
            
            //target.LoadFile(pathtofile);
            Assert.Inconclusive("Невозможно проверить метод, не возвращающий значение.");
        }
    }
}
