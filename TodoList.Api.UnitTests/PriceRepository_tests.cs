using Backend.DAL;
using Backend.Model;
using Backend.Pricing.BL;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace TodoList.Api.UnitTests
{
    public class PriceRepository_tests
    {


        private readonly IEnumerable<Price> _listOfPrices = new List<Price>
            {
                new Price
                {
                    ProductCode="A", Amount=1.25,
                    Pack = new Pack
                    {
                        Amount=3.0, Count=3
                    }
                },
                new Price
                {
                    ProductCode = "B",
                    Amount = 4.25,
                    Pack = new Pack
                    {
                        Amount = 0,
                        Count = 0
                    }
                },
                new Price
                {
                    ProductCode = "C",
                    Amount = 1,
                    Pack = new Pack
                    {
                        Amount = 5.0,
                        Count = 6
                    }
                },
                new Price
                {
                    ProductCode = "D",
                    Amount = 0.75,
                    Pack = new Pack
                    {
                        Amount = 0,
                        Count = 0
                    }
                }
            };

        [Fact]
        public async void PriceRepository_integration_has_data_test()
        {
            PriceUpdater<Price> PriceDataUpdater = new PriceUpdater<Price>();
            DataRepository<Price> rep = new DataRepository<Price>(PriceDataUpdater);

            var allDataTask = await rep.GetAllTask();
            Assert.True(allDataTask.Any());

            var allData = await rep.GetAllTask();
            Assert.True(allData.Any());

        }

        [Fact]
        public async void CalculationService_Sequence_InValid_Product_found_test()
        {
            //Arrange
            PriceUpdater<Price> PriceDataUpdater = new PriceUpdater<Price>();
            DataRepository<Price> rep = new DataRepository<Price>(PriceDataUpdater);

            Mock<IDataRepository<Price>> repoMock = new Mock<IDataRepository<Price>>();

            repoMock.Setup(m => m.GetAllTask()).Returns(Task.Run(() => _listOfPrices));

            var calculationService = new CalculationService(repoMock.Object);

            //Act
            var finalPriceResult = await calculationService.CalculatePrice("ABCX");



            //Asssert
            Assert.True(finalPriceResult.Result == 0);
            Assert.True(finalPriceResult.ComplexResult.ResultType == ResultType.BadRequest);
            Assert.True(finalPriceResult.ComplexResult.ResultType != ResultType.OK);
            Assert.Contains("Product with code = X does not exists in the system. Please, remove it from the sequence and try again.", finalPriceResult.ComplexResult.Message);
        }


        [Fact]
        public async void CalculationService_No_Prices_found_test()
        {
            //Arrange
            PriceUpdater<Price> PriceDataUpdater = new PriceUpdater<Price>();
            DataRepository<Price> rep = new DataRepository<Price>(PriceDataUpdater);

            Mock<IDataRepository<Price>> repoMock = new Mock<IDataRepository<Price>>();

            repoMock.Setup(m => m.GetAllTask()).Returns(Task.Run(() => new List<Price>() as IEnumerable<Price>));

            var calculationService = new CalculationService(repoMock.Object);

            //Act
            var finalPriceResult = await calculationService.CalculatePrice(new ProductScan { });



            //Asssert
            Assert.True(finalPriceResult.Result == 0);
            Assert.True(finalPriceResult.ComplexResult.ResultType == ResultType.NotFound);
            Assert.True(finalPriceResult.ComplexResult.ResultType != ResultType.OK);
            Assert.Contains("pricese data was found in the system", finalPriceResult.ComplexResult.Message);
        }

        [Fact]
        public async void CalculationService_No_Product_in_system_test()
        {
            //Arrange
            PriceUpdater<Price> PriceDataUpdater = new PriceUpdater<Price>();
            DataRepository<Price> rep = new DataRepository<Price>(PriceDataUpdater);

            Mock<IDataRepository<Price>> repoMock = new Mock<IDataRepository<Price>>();

            repoMock.Setup(m => m.GetAllTask()).Returns(Task.Run(() => _listOfPrices));

            var calculationService = new CalculationService(repoMock.Object);

            //Act
            var finalPriceResult = await calculationService.CalculatePrice(new ProductScan { CodesSingle = "X" });



            //Asssert
            Assert.True(finalPriceResult.Result == 0);
            Assert.True(finalPriceResult.ComplexResult.ResultType == ResultType.NotFound);
            Assert.True(finalPriceResult.ComplexResult.ResultType != ResultType.OK);
            Assert.Contains("Product with code = X was found in the system", finalPriceResult.ComplexResult.Message);
        }

        [Theory]
        [InlineData("ABCDABA", 13.25)]
        [InlineData("CCCCCCC", 6)]
        [InlineData("ABCD", 7.25)]
        public async void CalculationService_productScan_test(string initialScann, double expectedResult)
        {
            //Arrange
            PriceUpdater<Price> PriceDataUpdater = new PriceUpdater<Price>();
            DataRepository<Price> rep = new DataRepository<Price>(PriceDataUpdater);

            Mock<IDataRepository<Price>> repoMock = new Mock<IDataRepository<Price>>();

            repoMock.Setup(m => m.GetAllTask()).Returns(Task.Run(() => _listOfPrices));

            var calculationService = new CalculationService(repoMock.Object);


            var charsArray = initialScann.ToCharArray();
            List<ProductScan> productScans = new List<ProductScan>();

            for (int i = 0; i < charsArray.Length; i++)
            {
                if (i == 0)
                {
                    productScans.Add(new ProductScan { IsNewScan = true, CodesSingle = charsArray[i].ToString() });
                }
                else
                {
                    productScans.Add(new ProductScan { IsNewScan = false, CodesSingle = charsArray[i].ToString() });
                }

            }
            DataResult<double> finalPriceResult = new DataResult<double>(0, new ComplexResult());

            //Act
            foreach (var scannedProduct in productScans)
            {
                finalPriceResult = await calculationService.CalculatePrice(scannedProduct);
            }


            //Asssert
            Assert.True(finalPriceResult.Result != 0);
            Assert.True(finalPriceResult.ComplexResult.ResultType == ResultType.OK);
            Assert.True(finalPriceResult.ComplexResult.ResultType != ResultType.UnknownError);
            Assert.Equal(expectedResult, finalPriceResult.Result);



        }

        [Theory]
        [InlineData("ABCDABA", 13.25)]
        [InlineData("CCCCCCC", 6)]
        [InlineData("ABCD", 7.25)]
        public async void CalculationService_Scan_sequence_test(string initialScann, double expectedResult)
        {
            //Arrange
            PriceUpdater<Price> PriceDataUpdater = new PriceUpdater<Price>();
            DataRepository<Price> rep = new DataRepository<Price>(PriceDataUpdater);

            Mock<IDataRepository<Price>> repoMock = new Mock<IDataRepository<Price>>();

            repoMock.Setup(m => m.GetAllTask()).Returns(Task.Run(() => _listOfPrices));

            var calculationService = new CalculationService(repoMock.Object);



            //Act
            var res = await calculationService.CalculatePrice(initialScann);

            //Asssert
            Assert.True(res.Result != 0);
            Assert.True(res.ComplexResult.ResultType == ResultType.OK);
            Assert.True(res.ComplexResult.ResultType != ResultType.UnknownError);
            Assert.Equal(expectedResult, res.Result);

        }



    }
}
