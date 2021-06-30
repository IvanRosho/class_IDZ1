using IDZ1_library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject3
{
    
    
    /// <summary>
    ///Это класс теста для IDZ1Test, в котором должны
    ///находиться все модульные тесты IDZ1Test
    ///</summary>
    [TestClass()]
    public class IDZ1Test {


        private TestContext testContextInstance;

        /// <summary>
        ///Получает или устанавливает контекст теста, в котором предоставляются
        ///сведения о текущем тестовом запуске и обеспечивается его функциональность.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
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
        ///Тест для load_frame
        ///</summary>
        [TestMethod()]
        public void load_frameTest() {
            IDZ1 target = new IDZ1(); // TODO: инициализация подходящего значения
            target.loadfromfileidz("D:\\Расчеты\\ртуть\\ВИ-ОЭП1-ВД1_lazer_250_1.idz1");
            int k = (int)target.kadri; // TODO: инициализация подходящего значения
            int[,] expected = null; // TODO: инициализация подходящего значения
            int[,] actual;
            actual = target.load_frame(k);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Проверьте правильность этого метода теста.");
        }
    }
}
