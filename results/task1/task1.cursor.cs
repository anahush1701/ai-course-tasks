//  TASK: Formulate a prompt for LLM so that it generates a correct unit test 
//      using xUnit, with clear checks for both conditions (isMember = true/false).

namespace Task1.Services;

public class OrderService
{
    public DiscountResult CalculateOrder(int totalAmount, bool isMember, int itemsCount)
    {
        if (totalAmount < 0)
            throw new ArgumentException("Amount must be non-negative", nameof(totalAmount));

        int discount;
        if (isMember && totalAmount > 1000)
            discount = 15;
        else if (isMember)
            discount = 10;
        else if (totalAmount > 5000)
            discount = 5;
        else
            discount = 0;

        var bonusPoints = (totalAmount / 100) * (isMember ? 2 : 1);

        return new DiscountResult
        {
            DiscountPercent = discount,
            FinalAmount = totalAmount * (100 - discount) / 100,
            BonusPoints = bonusPoints
        };
    }
}

namespace Task1.Tests.Services
{
    public class ShippingServiceTests
    {
        [Fact]
        public void CalculateShipping_ReturnsFree_WhenWeightUnderThreshold()
        {
            // Arrange
            var service = new ShippingService();
            decimal weight = 4.5m; // kg
            decimal expected = 0m;

            // Act
            var result = service.CalculateShipping(weight);

            // Assert
            result.Should().Be(expected, because: "for weight up to 5 kg delivery is free");
        }
    }

    public class OrderServiceTests
    {
        [Theory]
        [InlineData(1200, true, 2, 15, 1020, 24)]   // isMember, amount > 1000
        [InlineData(800, true, 1, 10, 720, 16)]      // isMember, amount <= 1000
        [InlineData(6000, false, 5, 5, 5700, 60)]    // not member, amount > 5000
        [InlineData(500, false, 1, 0, 500, 5)]       // not member, amount <= 5000
        public void CalculateOrder_ReturnsExpectedResult_ForVariousInputs(int totalAmount, bool isMember, int itemsCount, int expectedDiscount, int expectedFinalAmount, int expectedBonusPoints)
        {
            // Arrange
            var service = new OrderService();

            // Act
            var result = service.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            result.DiscountPercent.Should().Be(expectedDiscount, because: "discount should match scenario");
            result.FinalAmount.Should().Be(expectedFinalAmount, because: "final amount should reflect discount");
            result.BonusPoints.Should().Be(expectedBonusPoints, because: "bonus points should match scenario");
        }

        [Fact]
        public void CalculateOrder_ThrowsArgumentException_WhenAmountIsNegative()
        {
            // Arrange
            var service = new OrderService();
            int totalAmount = -1;
            bool isMember = true;
            int itemsCount = 1;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.CalculateOrder(totalAmount, isMember, itemsCount));
        }
    }
}
