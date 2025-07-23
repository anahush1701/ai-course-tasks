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

public class DiscountResult
{
    public int DiscountPercent { get; set; }
    public int FinalAmount { get; set; }
    public int BonusPoints { get; set; }
}

namespace Task1.Tests.Services
{
    public class ShippingServiceTests
    {
        // This test method is provided as an example of structure and naming.
        // It implies the existence of ShippingService, which is not provided.
        // It will be removed as per the prompt's instruction.
        /*
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
        */
    }

    public class OrderServiceTests
    {
        [Theory]
        // isMember = true scenarios
        [InlineData(1200, true, 3, 15, 1020, 24, "a member with total amount > 1000 should get 15% discount and double bonus points")]
        [InlineData(1000, true, 2, 10, 900, 20, "a member with total amount = 1000 should get 10% discount and double bonus points")]
        [InlineData(750, true, 2, 10, 675, 14, "a member with total amount < 1000 should get 10% discount and double bonus points")]
        [InlineData(0, true, 1, 10, 0, 0, "a member with total amount = 0 should get 10% discount and double bonus points")]

        // isMember = false scenarios
        [InlineData(6000, false, 5, 5, 5700, 60, "a non-member with total amount > 5000 should get 5% discount and single bonus points")]
        [InlineData(5000, false, 4, 0, 5000, 50, "a non-member with total amount = 5000 should get 0% discount and single bonus points")]
        [InlineData(3000, false, 1, 0, 3000, 30, "a non-member with total amount < 5000 should get 0% discount and single bonus points")]
        [InlineData(0, false, 1, 0, 0, 0, "a non-member with total amount = 0 should get 0% discount and single bonus points")]
        public void CalculateOrder_ReturnsCorrectDiscountAndBonusPoints(
            int totalAmount, bool isMember, int itemsCount,
            int expectedDiscountPercent, int expectedFinalAmount, int expectedBonusPoints,
            string becauseMessage)
        {
            // Arrange
            var service = new OrderService();
            var expectedResult = new DiscountResult
            {
                DiscountPercent = expectedDiscountPercent,
                FinalAmount = expectedFinalAmount,
                BonusPoints = expectedBonusPoints
            };

            // Act
            var actualResult = service.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult, because: becauseMessage);
        }

        [Fact]
        public void CalculateOrder_TotalAmountNegative_ThrowsArgumentException()
        {
            // Arrange
            var service = new OrderService();
            int totalAmount = -100;
            bool isMember = true;
            int itemsCount = 1;

            // Act & Assert
            Action act = () => service.CalculateOrder(totalAmount, isMember, itemsCount);

            act.Should().Throw<ArgumentException>()
               .WithMessage("Amount must be non-negative (Parameter 'totalAmount')");
        }
    }
}